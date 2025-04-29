// <copyright file="NativeClient.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.  All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

#if UNITY_ANDROID

using System;
using System.Linq;

using GooglePlayGames.Android.Java;
using GooglePlayGames.Android.Java.Extensions;
using GooglePlayGames.Config;
using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

using UAJO = UnityEngine.AndroidJavaObject;
using UIUP = UnityEngine.SocialPlatforms.IUserProfile;

using AA    = GooglePlayGames.Api.Achievement;
using ACSC  = GooglePlayGames.Api.CommonStatusCodes;
using AFLVS = GooglePlayGames.Api.FriendsListVisibilityStatus;
using AIEC  = GooglePlayGames.Api.Events.IEventsClient;
using AIPGC = GooglePlayGames.Api.IPlayGamesClient;
using AISGC = GooglePlayGames.Api.SavedGame.ISavedGameClient;
using ALC   = GooglePlayGames.Api.LeaderboardCollection;
using ALFS  = GooglePlayGames.Api.LoadFriendsStatus;
using ALS   = GooglePlayGames.Api.LeaderboardStart;
using ALSD  = GooglePlayGames.Api.LeaderboardScoreData;
using ALTS  = GooglePlayGames.Api.LeaderboardTimeSpan;
using AP    = GooglePlayGames.Api.Player;
using APP   = GooglePlayGames.Api.PlayerProfile;
using APS   = GooglePlayGames.Api.PlayerStats;
using ARA   = GooglePlayGames.Api.RecallAccess;
using ARS   = GooglePlayGames.Api.ResponseStatus;
using ASGRS = GooglePlayGames.Api.SavedGame.SavedGameRequestStatus;
using ASIS  = GooglePlayGames.Api.SignInStatus;
using ASPC  = GooglePlayGames.Api.ScorePageCursor;
using AUS   = GooglePlayGames.Api.UiStatus;

using GPGHO = GooglePlayGames.Utils.PlayGamesHelperObject;

using APGC   = GooglePlayGames.Android.PlayGamesClient;
using APGCAS = GooglePlayGames.Android.PlayGamesClient.AuthState;

using JAE   = GooglePlayGames.Android.Java.ApiException;
using JLSBI = GooglePlayGames.Android.Java.LeaderboardScoreBuffer.Instance;
using JLSsI = GooglePlayGames.Android.Java.LeaderboardsClient.LeaderboardScores.Instance;
using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JPG   = GooglePlayGames.Android.Java.PlayGames;
using JPGS  = GooglePlayGames.Android.Java.PlayGamesSdk;
using JRAE  = GooglePlayGames.Android.Java.ResolvableApiException;

namespace GooglePlayGames.Android {

    public sealed class PlayGamesClient : AIPGC {

        internal enum AuthState {
            Authenticated   = 1,
            Unauthenticated = 0,
        }

        private volatile APGCAS m_authState                  = APGCAS.Unauthenticated;
        private readonly object m_authStateLock              = new();
        private volatile AIEC   m_eventsClient               = null;
        private          APP[]  m_friends                    = new APP[0];
        private readonly int    m_friendsMaxResults          = 200;
        private          UAJO   m_friendsResolutionException = null;
        private readonly object m_gameServicesLock           = new();
        private          ALFS   m_lastLoadFriendsStatus      = ALFS.Unknown;
        private readonly int    m_leaderboardMaxResults      = 25;
        private volatile AISGC  m_savedGameClient            = null;
        private volatile AP     m_user                       = null;

        private static bool IsApiException(JOI exception)
        {
            var name = exception.JGetClass().GetName();
            return name == "com.google.android.gms.common.api.ApiException";
        }

        internal PlayGamesClient()
        {
            GPGHO.CreateObject();
            JPGS.Initialize();
        }

        private void Authenticate(bool isAutoSignIn, Action<ASIS> callback)
        {
            const string method = "PlayGamesClient.Authenticate(bool, Action<SignInStatus>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            lock (m_authStateLock) {
                if (m_authState == APGCAS.Authenticated) {
                    Logger.d("AND: User is already authenticated; returning a success code");
                    callback.Invoke(ASIS.Success);
                    return;
                }
            }

            using var jClient = JPG.JGetGamesSignInClient();
            using var jTask   = isAutoSignIn ? jClient.JIsAuthenticated() : jClient.JSignIn();
            jTask.JAddOnSuccessListener(jResponse => {
                Logger.t($"AND: Success {method}");
                OnSignInResult(jResponse.IsAuthenticated(), callback);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.w($"AND: {jException.JToString()}");
                callback.Invoke(ASIS.InternalError);
            });
        }

        private ALSD CreateLeaderboardScoreData(string id, ALC collection, ALTS span, ARS status, JLSsI jScores)
        {
                  var result       = Utility.ToAndroidLeaderboardScoreData(jScores, id, status, collection, span);
            using var jLeaderboard = jScores.JGetLeaderboard();
            using var jVariants    = jLeaderboard.JGetVariants();
            using var jVariant     = jVariants.JGet(0);
            if (jVariant.HasPlayerInfo()) {
                result.PlayerScore = Utility.ToAndroidPlayerGameScore(jVariant, id, m_user.Id);
            }
            result.ApproximateCount = (ulong)jVariant.GetNumScores();
            return result;
        }

        private void LoadAllFriends(int pageSize, bool reload, bool more, Action<bool> callback)
        {
            const string method = "PlayGamesClient.LoadAllFriends(int, bool, bool, Action<bool>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            LoadFriendsPaginated(pageSize, more, reload, result => {
                m_lastLoadFriendsStatus = result;
                switch (result) {
                    case ALFS.Completed:
                        Logger.d("AND: Friends list loaded successfully");
                        callback.Invoke(true);
                        break;
                    case ALFS.LoadMore:
                        Logger.d("AND: Friends list loaded partially");
                        LoadAllFriends(pageSize, reload: false, more: true, callback);
                        break;
                    case ALFS.ResolutionRequired:
                    case ALFS.InternalError:
                    case ALFS.NotAuthorized:
                        Logger.d("AND: Friends list failed to load");
                        callback.Invoke(false);
                        break;
                    default:
                        Logger.d("AND: Friends list failed to load (unreachable code?)");
                        callback.Invoke(false);
                        break;
                }
            });
        }

        private void LoadFriendsPaginated(int size, bool more, bool reload, Action<ALFS> callback)
        {
            const string method = "PlayGamesClient.LoadFriendsPaginated(int, bool, bool, Action<LoadFriendsStatus>)";
            Logger.t($"AND: Calling {method}");

            m_friendsResolutionException = null;
            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetPlayersClient();
            using var jTask   = more ? jClient.JLoadMoreFriends(size) : jClient.JLoadFriends(size, reload);
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                using (var jPlayers = jData.JGet()) {
                    using (var jBundle = jPlayers.JGetMetadata()) {
                        var cursor = jBundle?.GetString("next_page_token");
                        m_lastLoadFriendsStatus = cursor != null ? ALFS.LoadMore : ALFS.Completed;
                    }
                    m_friends = Utility.ToAndroidPlayerProfile(jPlayers).ToArray();
                }
                callback.Invoke(m_lastLoadFriendsStatus);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                HelperFragment.IsResolutionRequired(jException, resolutionRequired =>
                {
                    if (resolutionRequired)
                    {
                        using var jResolvable = JRAE.WrapInstance(jException);
                        m_friendsResolutionException = jResolvable.JGetResolution();
                        m_lastLoadFriendsStatus = ALFS.ResolutionRequired;
                        m_friends = new APP[0];
                        callback.Invoke(ALFS.ResolutionRequired);
                    }
                    else
                    {
                        m_friendsResolutionException = null;
                        if (IsApiException(jException))
                        {
                            using var jWrapped = JAE.WrapInstance(jException);
                            var statusCode = jWrapped.GetStatusCode();
                            if (statusCode == /* GamesClientStatusCodes.NETWORK_ERROR_NO_DATA */ 26504)
                            {
                                m_lastLoadFriendsStatus = ALFS.NetworkError;
                                callback.Invoke(ALFS.NetworkError);
                                return;
                            }
                        }

                        m_lastLoadFriendsStatus = ALFS.InternalError;
                        Logger.e("LoadFriends failed: " + jException.JToString());
                        callback.Invoke(ALFS.InternalError);
                    }
                });
            });
        }

        private void OnSignInResult(bool isAuthenticated, Action<ASIS> callback)
        {
            const string method = "PlayGamesClient.OnSignInResult(bool, Action<SignInStatus>)";
            Logger.t($"AND: Calling {method}");

            if (!isAuthenticated) {
                lock (m_authStateLock) {
                    Logger.d("AND: Sign in result is NOT authenticated; returning a cancelation error");
                    callback.Invoke(ASIS.Canceled);
                }
                return;
            }

            using var jClient = JPG.JGetPlayersClient();
            using var jTask   = jClient.JGetCurrentPlayer();
            jTask.JAddOnCompleteListener(jIt => {
                Logger.t($"AND: Completed {method}");
                if (jIt.JIsSuccessful()) {
                    using (var jPlayer = jIt.JGetResult()) {
                        m_user = Utility.ToAndroidPlayer(jPlayer);
                    }
                    lock (m_gameServicesLock) {
                        m_eventsClient = new EventsClient();
                        m_savedGameClient = new SavedGameClient(this);
                    }
                    m_authState = APGCAS.Authenticated;
                    Logger.d("AND: Sign in result is authenticated; returning a success code");
                    callback.Invoke(ASIS.Success);
                    LoadAchievements(ignore => { });
                } else {
                    if (jIt.JIsCanceled()) {
                        Logger.d("AND: Sign in result is canceled; returning a cancelation error");
                        callback.Invoke(ASIS.Canceled);
                        return;
                    }
                    Logger.d("AND: Sign in result is NOT authenticated; returning an internal error");
                    using (var jException = jIt.JGetException()) {
                        Logger.e("AND: " + jException.JToString());
                    }
                    callback.Invoke(ASIS.InternalError);
                }
            });
        }

        #region IPlayGamesClient implementation

        public void AskForLoadFriendsResolution(Action<AUS> callback)
        {
            const string method = "PlayGamesClient.AskForLoadFriendsResolution(Action<UiStatus>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (m_friendsResolutionException != null) {
                HelperFragment.AskForLoadFriendsResolution(m_friendsResolutionException, callback);
                return;
            }

            Logger.d("AND: The developer asked for access to the friends list but there is no intent to trigger the UI.\n" +
                     "AND: This may be because the user has granted access already or the game has not called loadFriends() before.");
            using var jClient = JPG.JGetPlayersClient();
            using var jTask   = jClient.JLoadFriends(size: 1, reload: false);
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                callback.Invoke(AUS.Valid);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                HelperFragment.IsResolutionRequired(jException, required => {
                    if (required) {
                        using var jResolvable = JRAE.WrapInstance(jException);
                        m_friendsResolutionException = jResolvable.JGetResolution();
                        HelperFragment.AskForLoadFriendsResolution(m_friendsResolutionException, callback);
                        return;
                    }
                    if (IsApiException(jException)) {
                        using var jWrapped = JAE.WrapInstance(jException);
                        var statusCode = jWrapped.GetStatusCode();
                        // TODO: Port GamesClientStatusCodes
                        if (statusCode == /* GamesClientStatusCodes.NETWORK_ERROR_NO_DATA */ 26504) {
                            callback.Invoke(AUS.NetworkError);
                            return;
                        }
                    }
                    Logger.e("LoadFriends failed: " + jException.JToString());
                    callback.Invoke(AUS.InternalError);
                });
            });
        }

        public void Authenticate(Action<ASIS> callback)
        {
            const string method = "PlayGamesClient.Authenticate(Action<SignInStatus>)";
            Logger.t($"AND: Calling {method}");

            Authenticate(true, callback);
        }

        public AIEC GetEventsClient()
        {
            const string method = "PlayGamesClient.GetEventsClient()";
            Logger.t($"AND: Calling {method}");

            lock (m_gameServicesLock) {
                return m_eventsClient;
            }
        }

        public UIUP[] GetFriends()
        {
            const string method = "PlayGamesClient.GetFriends()";
            Logger.t($"AND: Calling {method}");

            return m_friends;
        }

        public void GetFriendsListVisibility(bool reload, Action<AFLVS> callback)
        {
            const string method = "PlayGamesClient.GetFriendsListVisibility(bool, Action<FriendsListVisibilityStatus>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetPlayersClient();
            using var jTask   = jClient.JGetCurrentPlayer(reload);
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                AFLVS status;
                using (var jPlayer = jData.JGet())
                using (var jInfo = jPlayer.JGetCurrentPlayerInfo()) {
                    status = jInfo.GetFriendsListVisibilityStatus();
                }
                callback.Invoke(status);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                callback.Invoke(AFLVS.NetworkError);
            });
        }

        public ALFS GetLastLoadFriendsStatus()
        {
            const string method = "PlayGamesClient.GetLastLoadFriendsStatus()";
            Logger.t($"AND: Calling {method}");

            return m_lastLoadFriendsStatus;
        }

        public void GetPlayerStats(Action<ACSC, APS> callback)
        {
            const string method = "PlayGamesClient.GetPlayerStats(Action<CommonStatusCodes, PlayerStats>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetPlayerStatsClient();
            using var jTask   = jClient.JLoadPlayerStats(reload: false);
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                var stats = null as APS;
                using (var jStats = jData.JGet()) {
                    stats = Utility.ToAndroidPlayerStats(jStats);
                }
                callback.Invoke(ACSC.Success, stats);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                var statusCode = IsAuthenticated() ? ACSC.InternalError : ACSC.SignInRequired;
                callback.Invoke(statusCode, new APS());
            });
        }

        public AISGC GetSavedGameClient()
        {
            const string method = "PlayGamesClient.GetSavedGameClient()";
            Logger.t($"AND: Calling {method}");

            lock (m_gameServicesLock) {
                return m_savedGameClient;
            }
        }

        public string GetUserDisplayName()
        {
            const string method = "PlayGamesClient.GetUserDisplayName()";
            Logger.t($"AND: Calling {method}");

            return m_user?.UserName;
        }

        public string GetUserId()
        {
            const string method = "PlayGamesClient.GetUserId()";
            Logger.t($"AND: Calling {method}");

            return m_user?.Id;
        }

        public string GetUserImageUrl()
        {
            const string method = "PlayGamesClient.GetUserImageUrl()";
            Logger.t($"AND: Calling {method}");

            return m_user?.AvatarUrl;
        }

        public void IncrementAchievement(string id, int steps, Action<bool> callback)
        {
            const string method = "PlayGamesClient.IncrementAchievement(string, int, Action<bool>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(false);
                return;
            }

            using (var jClient = JPG.JGetAchievementsClient()) {
                jClient.Increment(id, steps);
            }
            callback.Invoke(true);
        }

        public bool IsAuthenticated()
        {
            const string method = "PlayGamesClient.IsAuthenticated()";
            Logger.t($"AND: Calling {method}");

            lock (m_authStateLock) {
                return m_authState == APGCAS.Authenticated;
            }
        }

        public int LeaderboardMaxResults()
        {
            const string method = "PlayGamesClient.LeaderboardMaxResults()";
            Logger.t($"AND: Calling {method}");

            return m_leaderboardMaxResults;
        }

        public void LoadAchievements(Action<AA[]> callback)
        {
            const string method = "PlayGamesClient.LoadAchievements(Action<Achievement[]>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetAchievementsClient();
            using var jTask   = jClient.JLoad(reload: false);
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                var achievements = null as AA[];
                using (var jAchievements = jData.JGet()) {
                    achievements = Utility.ToAndroidAchievement(jAchievements).ToArray();
                }
                callback.Invoke(achievements);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                callback.Invoke(new AA[0]);
            });
        }

        public void LoadFriends(Action<bool> callback)
        {
            const string method = "PlayGamesClient.LoadFriends(Action<bool>)";
            Logger.t($"AND: Calling {method}");

            LoadAllFriends(m_friendsMaxResults, reload: false, more: false, callback);
        }

        public void LoadFriends(int size, bool reload, Action<ALFS> callback)
        {
            const string method = "PlayGamesClient.LoadFriends(int, bool, Action<LoadFriendsStatus>)";
            Logger.t($"AND: Calling {method}");

            LoadFriendsPaginated(size, more: false, reload, callback);
        }

        public void LoadMoreFriends(int size, Action<ALFS> callback)
        {
            const string method = "PlayGamesClient.LoadMoreFriends(int, Action<LoadFriendsStatus>)";
            Logger.t($"AND: Calling {method}");

            LoadFriendsPaginated(size, more: true, reload: false, callback);
        }

        public void LoadMoreScores(ASPC token, int rows, Action<ALSD> callback)
        {
            const string method = "PlayGamesClient.LoadMoreScores(ASPC, int, Action<LeaderboardScoreData>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            using var jClient       = JPG.JGetLeaderboardsClient();
            using var jLeaderboards = (JLSBI)token.Token;
                  var  direction    = Utility.ToJavaPageDirection(token.Direction);
            using var jTask         = jClient.JLoadMoreScores(jLeaderboards, rows, direction);
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                var leaderboard = null as ALSD;
                using (var jScores = jData.JGet()) {
                    var status = jData.GetResponseStatus();
                    leaderboard = CreateLeaderboardScoreData(token.LeaderboardId, token.Collection, token.TimeSpan, status, jScores);
                }
                callback.Invoke(leaderboard);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                HelperFragment.IsResolutionRequired(jException, required => {
                    if (required) {
                        using var jResolvable = JRAE.WrapInstance(jException);
                        m_friendsResolutionException = jResolvable.JGetResolution();
                        callback.Invoke(new ALSD(token.LeaderboardId, ARS.ResolutionRequired));
                    } else {
                        m_friendsResolutionException = null;
                    }
                });
                callback.Invoke(new ALSD(token.LeaderboardId, ARS.InternalError));
            });
        }

        public void LoadScores(string id, ALS start, int rows, ALC collection, ALTS span, Action<ALSD> callback)
        {
            const string method = "PlayGamesClient.LoadScores(string, LeaderboardStart, int, LeaderboardCollection, LeaderboardTimeSpan, Action<LeaderboardScoreData>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            using var jClient     = JPG.JGetLeaderboardsClient();
                  var jSpan       = Utility.ToJavaLeaderboardVariantTimeSpan(span);
                  var jCollection = Utility.ToJavaLeaderboardVariantCollection(collection);
            using var jtask       = start == ALS.TopScores
                                        ? jClient.JLoadTopScores(id, jSpan, jCollection, rows)
                                        : jClient.JLoadPlayerCenteredScores(id, jSpan, jCollection, rows);
            jtask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                var data = null as ALSD;
                using (var jScores = jData.JGet()) {
                    data = CreateLeaderboardScoreData(id, collection, span, jData.GetResponseStatus(), jScores);
                }
                callback.Invoke(data);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                HelperFragment.IsResolutionRequired(jException, resolutionRequired =>
                {
                    if (resolutionRequired)
                    {
                        using var jResolvable = JRAE.WrapInstance(jException);
                        m_friendsResolutionException = jResolvable.JGetResolution();
                        callback.Invoke(new ALSD(id, ARS.ResolutionRequired));
                    }
                    else
                    {
                        m_friendsResolutionException = null;
                    }
                });
                callback.Invoke(new ALSD(id, ARS.InternalError));
            });
        }

        public void LoadUsers(string[] userIds, Action<UIUP[]> callback)
        {
            const string method = "PlayGamesClient.LoadUsers(string[], Action<IUserProfile[]>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(new UIUP[0]);
                return;
            }

            using var jClient = JPG.JGetPlayersClient();
            var @lock = new object();
            var count = userIds.Length;
            var acc = 0;
            var users = new UIUP[count];
            for (var i = 0; i < count; i += 1) {
                using var jTask = jClient.JLoadPlayer(userIds[i]);
                jTask.JAddOnSuccessListener(jData => {
                    Logger.t($"AND: Success number {i}/{count} of {method}");
                    using (var jPlayer = jData.JGet()) {
                        var id = jPlayer.GetPlayerId();
                        for (var j = 0; j < count; j += 1) {
                            if (id == userIds[j]) {
                                users[j] = Utility.ToAndroidPlayer(jPlayer);
                                break;
                            }
                        }
                    }
                    lock (@lock) {
                        acc += 1;
                        if (acc == count) callback.Invoke(users);
                    }
                }).JAddOnFailureListener(jException => {
                    Logger.t($"AND: Failure number {i}/{count} of {method}");
                    Logger.d("AND: " + jException.JToString());
                    lock (@lock) {
                        acc += 1;
                        if (acc == count) callback.Invoke(users);
                    }
                });
            }
        }

        public void ManuallyAuthenticate(Action<ASIS> callback)
        {
            const string method = "PlayGamesClient.ManuallyAuthenticate(Action<SignInStatus>)";
            Logger.t($"AND: Calling {method}");

            Authenticate(false, callback);
        }

        public void RequestRecallAccessToken(Action<ARA> callback)
        {
            const string method = "PlayGamesClient.RequestRecallAccessToken(Action<RecallAccess>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetRecallClient();
            using var jTask   = jClient.JRequestRecallAccess();
            jTask.JAddOnSuccessListener(jAccess => {
                Logger.t($"AND: Success {method}");
                var id = jAccess.GetSessionId();
                callback.Invoke(new ARA(id));
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                callback.Invoke(null);
            });
        }

        public void RequestServerSideAccess(bool reload, Action<string> callback)
        {
            const string method = "PlayGamesClient.RequestServerSideAccess(bool, Action<string>)";
            Logger.t($"AND: Calling {method}");

            if (!GameInformation.HasInstance) {
                throw new NullReferenceException("The GameInformation instance is not available");
            } else if (!GameInformation.Instance.HasWebId) {
                Logger.w("AND: The GameInformation instance is missing the Web Client Id; make sure it's configured");
            }

            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetGamesSignInClient();
            using var jTask   = jClient.JRequestServerSideAccess(reload: true, webId: GameInformation.Instance.WebId);
            jTask.JAddOnSuccessListener(token => {
                Logger.t($"AND: Success {method}");
                callback.Invoke(token);
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.d("AND: " + jException.JToString());
                callback.Invoke(null);
            });
        }

        public void RevealAchievement(string achId, Action<bool> callback)
        {
            const string method = "PlayGamesClient.RevealAchievement(string, Action<bool>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(false);
                return;
            }

            using var jClient = JPG.JGetAchievementsClient();
            jClient.Reveal(achId);
            callback.Invoke(true);
        }

        public void SetStepsAtLeast(string achId, int steps, Action<bool> callback)
        {
            const string method = "PlayGamesClient.SetStepsAtLeast(string, int, Action<bool>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(false);
                return;
            }

            using var jClient = JPG.JGetAchievementsClient();
            jClient.SetSteps(achId, steps);
            callback.Invoke(true);
        }

        public void ShowAchievementsUI(Action<AUS> callback)
        {
            const string method = "PlayGamesClient.ShowAchievementsUI(Action<UiStatus>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(AUS.NotAuthorized);
                return;
            }

            HelperFragment.ShowAchievementsUi(callback);
        }

        public void ShowCompareProfileWithAlternativeNameHintsUI(string userId, string comparandUserName, string userName, Action<AUS> callback)
        {
            const string method = "PlayGamesClient.ShowCompareProfileWithAlternativeNameHintsUI(string, string, string, Action<UiStatus>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            HelperFragment.ShowCompareProfileWithAlternativeNameHintsUi(userId, comparandUserName, userName, callback);
        }

        public void ShowLeaderboardUI(string id, ALTS span, Action<AUS> callback)
        {
            const string method = "PlayGamesClient.ShowLeaderboardUI(string, LeaderboardTimeSpan, Action<UiStatus>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(AUS.NotAuthorized);
                return;
            }

            if (id == null) {
                HelperFragment.ShowAllLeaderboardsUi(callback);
            } else {
                HelperFragment.ShowLeaderboardUi(id, span, callback);
            }
        }

        public void SubmitScore(string id, long score, string metadata, Action<bool> callback)
        {
            const string method = "PlayGamesClient.SubmitScore(string, long, string, Action<bool>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(false);
                return;
            }

            using var jClient = JPG.JGetLeaderboardsClient();
            jClient.SubmitScore(id, score, metadata);
            callback.Invoke(true);
        }

        public void SubmitScore(string leaderboardId, long score, Action<bool> callback)
        {
            const string method = "PlayGamesClient.SubmitScore(string, long, Action<bool>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(false);
                return;
            }

            using var jClient = JPG.JGetLeaderboardsClient();
            jClient.SubmitScore(leaderboardId, score);
            callback.Invoke(true);
        }

        public void UnlockAchievement(string achId, Action<bool> callback)
        {
            const string method = "PlayGamesClient.UnlockAchievement(string, Action<bool>)";
            Logger.t($"AND: Calling {method}");

            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(false);
                return;
            }

            using var jClient = JPG.JGetAchievementsClient();
            jClient.Unlock(achId);
            callback.Invoke(true);
        }

        #endregion IPlayGamesClient implementation

        #region Object implementation

        private bool m_stringify = false;

        public override string ToString()
        {
            if (m_stringify) return $"PlayGamesClient(...)";
            try {
                m_stringify = true;
                return $"PlayGamesClient(authState: {m_authState}, authStateLock: {m_authStateLock}, eventsClient: {m_eventsClient}, friends: IUserProfile[{m_friends.Length}], friendsMaxResults: {m_friendsMaxResults}, friendsResolutionException: {m_friendsResolutionException}, gameServicesLock: {m_gameServicesLock}, lastLoadFriendsStatus: {m_lastLoadFriendsStatus}, leaderboardMaxResults: {m_leaderboardMaxResults}, savedGameClient: {m_savedGameClient}, user: {m_user})";
            } finally {
                m_stringify = false;
            }
        }

        public override int GetHashCode()
        {
            var hash1 = HashCode.Combine(m_authState,        m_authStateLock,         m_eventsClient,          m_friends,         m_friendsMaxResults, m_friendsResolutionException);
            var hash2 = HashCode.Combine(m_gameServicesLock, m_lastLoadFriendsStatus, m_leaderboardMaxResults, m_savedGameClient, m_user);
            return HashCode.Combine(hash1, hash2);
        }

        public override bool Equals(object other)
        {
            if (other is not APGC it) return false;
            return Utility.Equals(m_authState,                  it.m_authState)                  &&
                   Utility.Equals(m_authStateLock,              it.m_authStateLock)              &&
                   Utility.Equals(m_eventsClient,               it.m_eventsClient)               &&
                   Utility.Equals(m_friends,                    it.m_friends)                    &&
                   Utility.Equals(m_friendsMaxResults,          it.m_friendsMaxResults)          &&
                   Utility.Equals(m_friendsResolutionException, it.m_friendsResolutionException) &&
                   Utility.Equals(m_gameServicesLock,           it.m_gameServicesLock)           &&
                   Utility.Equals(m_lastLoadFriendsStatus,      it.m_lastLoadFriendsStatus)      &&
                   Utility.Equals(m_leaderboardMaxResults,      it.m_leaderboardMaxResults)      &&
                   Utility.Equals(m_savedGameClient,            it.m_savedGameClient)            &&
                   Utility.Equals(m_user,                       it.m_user);
        }

        #endregion Object implementation

    }

}

namespace GooglePlayGames.Android.Java.Extensions {

    internal static class PlayGamesClientExtensions {

        public static ASGRS GetSavedGameRequestStatus(this PlayGamesClient self) => Utility.ToAndroidSavedGameRequestStatus(self.IsAuthenticated());

    }

}

#endif