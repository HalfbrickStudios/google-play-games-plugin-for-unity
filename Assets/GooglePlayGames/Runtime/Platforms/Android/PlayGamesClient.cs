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

using GooglePlayGames.Android.Java.Extensions;
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
using APS   = GooglePlayGames.Api.PlayerStats;
using ARA   = GooglePlayGames.Api.RecallAccess;
using ARS   = GooglePlayGames.Api.ResponseStatus;
using ASGRS = GooglePlayGames.Api.SavedGame.SavedGameRequestStatus;
using ASIS  = GooglePlayGames.Api.SignInStatus;
using ASPC  = GooglePlayGames.Api.ScorePageCursor;
using AUS   = GooglePlayGames.Api.UiStatus;

using GPGHO = GooglePlayGames.Utils.PlayGamesHelperObject;

using APGCAS = GooglePlayGames.Android.PlayGamesClient.AuthState;

using JLSBI = GooglePlayGames.Android.Java.LeaderboardScoreBuffer.Instance;
using JLSsI = GooglePlayGames.Android.Java.LeaderboardsClient.LeaderboardScores.Instance;
using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JPG   = GooglePlayGames.Android.Java.PlayGames;
using JPGS  = GooglePlayGames.Android.Java.PlayGamesSdk;

namespace GooglePlayGames.Android {

    public sealed class PlayGamesClient : AIPGC {

        internal enum AuthState {
            Authenticated   = 1,
            Unauthenticated = 0,
        }

        private volatile APGCAS m_authState = APGCAS.Unauthenticated;
        private readonly object m_authStateLock = new();
        private volatile AIEC m_eventsClient = null;
        private UIUP[] m_friends = new UIUP[0];
        private readonly int m_friendsMaxResults = 200;
        private readonly object m_gameServicesLock = new();
        private ALFS m_lastLoadFriendsStatus = ALFS.Unknown;
        private readonly int m_leaderboardMaxResults = 25;
        private volatile AISGC m_savedGameClient = null;
        private volatile AP m_user = null;
        private UAJO m_friendsResolutionException = null;

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
            callback = Utility.ToUiAction(callback);

            lock (m_authStateLock) {
                if (m_authState == APGCAS.Authenticated) {
                    Logger.d("Already authenticated.");
                    callback.Invoke(ASIS.Success);
                    return;
                }
            }

            using var jClient = JPG.JGetGamesSignInClient();
            using var jTask = isAutoSignIn ? jClient.JIsAuthenticated() : jClient.JSignIn();
            jTask.JAddOnSuccessListener(jResponse => {
                SignInOnResult(jResponse.IsAuthenticated(), callback);
            }).JAddOnFailureListener(jException => {
                Logger.e("Authentication failed - " + jException.JToString());
                callback.Invoke(ASIS.InternalError);
            });
        }

        private ALSD CreateLeaderboardScoreData(string id, ALC collection, ALTS span, ARS status, JLSsI jScores)
        {
            var result = Utility.ToAndroidLeaderboardScoreData(jScores, id, status, collection, span);
            using var jLeaderboard = jScores.JGetLeaderboard();
            using var jVariants = jLeaderboard.JGetVariants();
            using var jVariant = jVariants.JGet(0);
            if (jVariant.HasPlayerInfo()) {
                result.PlayerScore = Utility.ToAndroidPlayerGameScore(jVariant, id, m_user.Id);
            }
            result.ApproximateCount = (ulong)jVariant.GetNumScores();
            return result;
        }

        private void LoadAllFriends(int pageSize, bool reload, bool more, Action<bool> callback)
        {
            callback = Utility.ToUiAction(callback);

            LoadFriendsPaginated(pageSize, more, reload, result => {
                m_lastLoadFriendsStatus = result;
                switch (result) {
                    case ALFS.Completed:
                        callback.Invoke(true);
                        break;
                    case ALFS.LoadMore:
                        LoadAllFriends(pageSize, reload: false, more: true, callback);
                        break;
                    case ALFS.ResolutionRequired:
                    case ALFS.InternalError:
                    case ALFS.NotAuthorized:
                        callback.Invoke(false);
                        break;
                    default:
                        Logger.d("There was an error when loading friends." + result);
                        callback.Invoke(false);
                        break;
                }
            });
        }

        private void LoadFriendsPaginated(int size, bool more, bool reload, Action<ALFS> callback)
        {
            m_friendsResolutionException = null;
            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetPlayersClient();
            using var jTask   = more ? jClient.JLoadMoreFriends(size) : jClient.JLoadFriends(size, reload);
            jTask.JAddOnSuccessListener(jData => {
                using (var jPlayers = jData.JGet()) {
                    using (var jBundle = jPlayers.JGetMetadata()) {
                        var cursor = jBundle?.GetString("next_page_token");
                        m_lastLoadFriendsStatus = cursor != null ? ALFS.LoadMore : ALFS.Completed;
                    }
                    m_friends = Utility.ToAndroidPlayerProfile(jPlayers).ToArray();
                }
                callback.Invoke(m_lastLoadFriendsStatus);
            }).JAddOnFailureListener(jException => {
                // HelperFragmentClass.Instance.IsResolutionRequired(exception, resolutionRequired => {
                //     if (resolutionRequired) {
                //         m_friendsResolutionException = exception.Call<UAJO>("getResolution");
                //         m_lastLoadFriendsStatus = LoadFriendsStatus.ResolutionRequired;
                //         m_friends = new IUserProfile[0];
                //         InvokeCallbackOnGameThread(callback, LoadFriendsStatus.ResolutionRequired);
                //     } else {
                //         m_friendsResolutionException = null;
                //         if (IsApiException(exception)) {
                //             var casted = exception as ApiExceptionObject;
                //             var statusCode = casted.GetStatusCode();
                //             if (statusCode == /* GamesClientStatusCodes.NETWORK_ERROR_NO_DATA */ 26504) {
                //                 m_lastLoadFriendsStatus = LoadFriendsStatus.NetworkError;
                //                 InvokeCallbackOnGameThread(callback, LoadFriendsStatus.NetworkError);
                //                 return;
                //             }
                //         }
                // 
                //         m_lastLoadFriendsStatus = LoadFriendsStatus.InternalError;
                //         Logger.e("LoadFriends failed: " + jException.JToString());
                //         InvokeCallbackOnGameThread(callback, LoadFriendsStatus.InternalError);
                //     }
                // });
            });
        }

        private void SignInOnResult(bool isAuthenticated, Action<ASIS> callback)
        {
            callback = Utility.ToUiAction(callback);

            if (!isAuthenticated) {
                lock (m_authStateLock) {
                    Logger.e("Returning an error code.");
                    callback(ASIS.Canceled);
                }
                return;
            }

            using var jTask = JPG.JGetPlayersClient().JGetCurrentPlayer();
            jTask.JAddOnCompleteListener(jIt => {
                if (jIt.JIsSuccessful()) {
                    using (var jData = jIt.JGetResult()) {
                        using var jPlayer = jData.JGet();
                        m_user = Utility.ToAndroidPlayer(jPlayer);
                    }
                    lock (m_gameServicesLock) {
                        m_eventsClient = new EventsClient();
                        m_savedGameClient = new SavedGameClient(this);
                    }
                    m_authState = APGCAS.Authenticated;
                    callback.Invoke(ASIS.Success);
                    Logger.d("Authentication succeeded");
                    LoadAchievements(ignore => { });
                } else {
                    if (jIt.JIsCanceled()) {
                        callback.Invoke(ASIS.Canceled);
                        return;
                    }
                    using var jException = jIt.JGetException();
                    Logger.e("Authentication failed - " + jException.JToString());
                    callback.Invoke(ASIS.InternalError);
                }
            });
        }

        #region IPlayGamesClient implementation

        public void AskForLoadFriendsResolution(Action<AUS> callback)
        {
            callback = Utility.ToUiAction(callback);

            if (m_friendsResolutionException != null) {
                // HelperFragmentClass.AskForLoadFriendsResolution(m_friendsResolutionException, callback);
                return;
            }

            Logger.d("The developer asked for access to the friends list but there is no intent to trigger the UI. " +
                     "This may be because the user has granted access already or the game has not called loadFriends() before.");
            using var jClient = JPG.JGetPlayersClient();
            using var jTask   = jClient.JLoadFriends(size: 1, reload: false);
            jTask.JAddOnSuccessListener(jData => {
                callback.Invoke(AUS.Valid);
            }).JAddOnFailureListener(jException => {
                // HelperFragmentClass.IsResolutionRequired(exception, resolutionRequired => {
                //     if (resolutionRequired) {
                //         m_friendsResolutionException = exception.Call<AndroidJavaObject>("getResolution");
                //         // HelperFragmentClass.AskForLoadFriendsResolution(m_friendsResolutionException, AsOnGameThreadCallback(callback));
                //         return;
                //     }
                //     if (IsApiException(exception)) {
                //         var casted = exception as AEO;
                //         var statusCode = casted.GetStatusCode();
                //         if (statusCode == /* GamesClientStatusCodes.NETWORK_ERROR_NO_DATA */ 26504) {
                //             InvokeCallbackOnGameThread(callback, UIStatus.NetworkError);
                //             return;
                //         }
                //     }
                //     Logger.e("LoadFriends failed: " + jException.JToString());
                //     InvokeCallbackOnGameThread(callback, UIStatus.InternalError);
                // });
            });
        }

        public void Authenticate(Action<ASIS> callback) => Authenticate(true, callback);

        public AIEC GetEventsClient()
        {
            lock (m_gameServicesLock) {
                return m_eventsClient;
            }
        }

        public UIUP[] GetFriends() => m_friends;

        public void GetFriendsListVisibility(bool reload, Action<AFLVS> callback)
        {
            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetPlayersClient();
            using var jTask   = jClient.JGetCurrentPlayer(reload);
            jTask.JAddOnSuccessListener(jData => {
                using var jPlayer = jData.JGet();
                using var jInfo = jPlayer.JGetCurrentPlayerInfo();
                callback.Invoke(jInfo.GetFriendsListVisibilityStatus());
            }).JAddOnFailureListener(jException => {
                callback.Invoke(AFLVS.NetworkError);
            });
        }

        public ALFS GetLastLoadFriendsStatus() => m_lastLoadFriendsStatus;

        public void GetPlayerStats(Action<ACSC, APS> callback)
        {
            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetPlayerStatsClient();
            using var jTask   = jClient.JLoadPlayerStats(reload: false);
            jTask.JAddOnSuccessListener(jData => {
                var stats = null as APS;
                using (var jStats = jData.JGet()) {
                    stats = Utility.ToAndroidPlayerStats(jStats);
                }
                callback.Invoke(ACSC.Success, stats);
            }).JAddOnFailureListener(jException => {
                Logger.e("GetPlayerStats failed: " + jException.JToString());
                var statusCode = IsAuthenticated() ? ACSC.InternalError : ACSC.SignInRequired;
                callback.Invoke(statusCode, new APS());
            });
        }

        public AISGC GetSavedGameClient()
        {
            lock (m_gameServicesLock) {
                return m_savedGameClient;
            }
        }

        public string GetUserDisplayName() => m_user?.UserName;

        public string GetUserId() => m_user?.Id;

        public string GetUserImageUrl() => m_user?.AvatarURL;

        public void IncrementAchievement(string id, int steps, Action<bool> callback)
        {
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
            lock (m_authStateLock) {
                return m_authState == APGCAS.Authenticated;
            }
        }

        public int LeaderboardMaxResults() => m_leaderboardMaxResults;

        public void LoadAchievements(Action<AA[]> callback)
        {
            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetAchievementsClient();
            using var jTask   = jClient.JLoad(reload: false);
            jTask.JAddOnSuccessListener(jData => {
                var achievements = null as AA[];
                using (var jAchievements = jData.JGet()) {
                    achievements = Utility.ToAndroidAchievement(jAchievements).ToArray();
                }
                callback.Invoke(achievements);
            }).JAddOnFailureListener(jException => {
                Logger.e("LoadAchievements failed: " + jException.JToString());
                callback.Invoke(new AA[0]);
            });
        }

        public void LoadFriends(Action<bool> callback) => LoadAllFriends(m_friendsMaxResults, reload: false, more: false, callback);

        public void LoadFriends(int size, bool reload, Action<ALFS> callback) => LoadFriendsPaginated(size, more: false, reload, callback);

        public void LoadMoreFriends(int size, Action<ALFS> callback) => LoadFriendsPaginated(size, more: true, reload: false, callback);

        public void LoadMoreScores(ASPC token, int rows, Action<ALSD> callback)
        {
            callback = Utility.ToUiAction(callback);

            using var jClient       = JPG.JGetLeaderboardsClient();
            using var jLeaderboards = (JLSBI)token.LeaderboardScoreBuffer;
                  var  direction    = Utility.ToJavaPageDirection(token.Direction);
            using var jTask         = jClient.JLoadMoreScores(jLeaderboards, rows, direction);
            jTask.JAddOnSuccessListener(jData => {
                var leaderboard = null as ALSD;
                using (var jScores = jData.JGet()) {
                    var status = jData.GetResponseStatus();
                    leaderboard = CreateLeaderboardScoreData(token.LeaderboardId, token.Collection, token.TimeSpan, status, jScores);
                }
                callback.Invoke(leaderboard);
            }).JAddOnFailureListener(jException => {
                // HelperFragmentClass.IsResolutionRequired(exception, resolutionRequired => {
                //     if (resolutionRequired) {
                //         m_friendsResolutionException = exception.Call<UAJO>("getResolution");
                //         InvokeCallbackOnGameThread(callback, new LeaderboardScoreData(token.LeaderboardId, ResponseStatus.ResolutionRequired));
                //     } else {
                //         m_friendsResolutionException = null;
                //     }
                // });
                Logger.e("LoadMoreScores failed: " + jException.JToString());
                callback.Invoke(new ALSD(token.LeaderboardId, ARS.InternalError));
            });
        }

        public void LoadScores(string id, ALS start, int rows, ALC collection, ALTS span, Action<ALSD> callback)
        {
            callback = Utility.ToUiAction(callback);

            using var jClient     = JPG.JGetLeaderboardsClient();
                  var jSpan       = Utility.ToJavaLeaderboardVariantTimeSpan(span);
                  var jCollection = Utility.ToJavaLeaderboardVariantCollection(collection);
            using var jtask       = start == ALS.TopScores
                                        ? jClient.JLoadTopScores(id, jSpan, jCollection, rows)
                                        : jClient.JLoadPlayerCenteredScores(id, jSpan, jCollection, rows);
            jtask.JAddOnSuccessListener(jData => {
                var data = null as ALSD;
                using (var jScores = jData.JGet()) {
                    data = CreateLeaderboardScoreData(id, collection, span, jData.GetResponseStatus(), jScores);
                }
                callback.Invoke(data);
            }).JAddOnFailureListener(jException => {
                // HelperFragmentClass.IsResolutionRequired(exception, resolutionRequired => {
                //     if (resolutionRequired) {
                //         m_friendsResolutionException = exception.Call<UAJO>("getResolution");
                //         InvokeCallbackOnGameThread(callback, new LeaderboardScoreData(id, ResponseStatus.ResolutionRequired));
                //     } else {
                //         m_friendsResolutionException = null;
                //     }
                // });
                Logger.e("LoadScores failed: " + jException.JToString());
                callback.Invoke(new ALSD(id, ARS.InternalError));
            });
        }

        public void LoadUsers(string[] userIds, Action<UIUP[]> callback)
        {
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
                    Logger.e("LoadUsers failed for index " + i + " with: " + jException.JToString());
                    lock (@lock) {
                        acc += 1;
                        if (acc == count) callback.Invoke(users);
                    }
                });
            }
        }

        public void ManuallyAuthenticate(Action<ASIS> callback) => Authenticate(false, callback);

        public void RequestRecallAccessToken(Action<ARA> callback)
        {
            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetRecallClient();
            using var jTask   = jClient.JRequestRecallAccess();
            jTask.JAddOnSuccessListener(jAccess => {
                var id = jAccess.GetSessionId();
                callback.Invoke(new ARA(id));
            }).JAddOnFailureListener(jException => {
                Logger.e("Requesting Recall access task failed - " + jException.JToString());
                callback.Invoke(null);
            });
        }

        public void RequestServerSideAccess(bool reload, Action<string> callback)
        {
            callback = Utility.ToUiAction(callback);

            using var jClient = JPG.JGetGamesSignInClient();
            using var jTask   = jClient.JRequestServerSideAccess(reload: true, webId: string.Empty);
            jTask.JAddOnSuccessListener(callback).JAddOnFailureListener(jException => {
                Logger.e("Requesting server side access task failed - " + jException.JToString());
                callback.Invoke(null);
            });
        }

        public void RevealAchievement(string achId, Action<bool> callback)
        {
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
            callback = Utility.ToUiAction(callback);
            if (!IsAuthenticated()) {
                callback.Invoke(AUS.NotAuthorized);
                return;
            }
            // HelperFragmentClass.ShowAchievementsUI(callback);
        }

        public void ShowCompareProfileWithAlternativeNameHintsUI(string playerId, string otherPlayerInGameName, string currentPlayerInGameName, Action<AUS> callback)
        {
            callback = Utility.ToUiAction(callback);
            // HelperFragmentClass.ShowCompareProfileWithAlternativeNameHintsUI(playerId, otherPlayerInGameName, currentPlayerInGameName, AsOnGameThreadCallback(callback));
        }

        public void ShowLeaderboardUI(string leaderboardId, ALTS span, Action<AUS> callback)
        {
            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                callback.Invoke(AUS.NotAuthorized);
                return;
            }

            if (leaderboardId == null) {
                // HelperFragmentClass.ShowAllLeaderboardsUI(callback);
            } else {
                // HelperFragmentClass.ShowLeaderboardUI(leaderboardId, span, callback);
            }
        }

        public void SubmitScore(string id, long score, string metadata, Action<bool> callback)
        {
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

    }

}

namespace GooglePlayGames.Android.Java.Extensions {

    internal static class PlayGamesClientExtensions {

        public static ASGRS GetSavedGameRequestStatus(this PlayGamesClient self) => Utility.ToAndroidSavedGameRequestStatus(self.IsAuthenticated());

    }

}

#endif