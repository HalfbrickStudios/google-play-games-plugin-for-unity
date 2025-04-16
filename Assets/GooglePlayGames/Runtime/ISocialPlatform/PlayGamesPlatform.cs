// <copyright file="PlayGamesPlatform.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc. All Rights Reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

using GPGA  = GooglePlayGames.PlayGamesAchievement;
using GPGP  = GooglePlayGames.PlayGamesPlatform;
using GPGL  = GooglePlayGames.PlayGamesLeaderboard;
using GPGLU = GooglePlayGames.PlayGamesLocalUser;
using GPGCF = GooglePlayGames.PlayGamesClientFactory;

using UIA  = UnityEngine.SocialPlatforms.IAchievement;
using UIAD = UnityEngine.SocialPlatforms.IAchievementDescription;
using UIL  = UnityEngine.SocialPlatforms.ILeaderboard;
using UILU = UnityEngine.SocialPlatforms.ILocalUser;
using UIS  = UnityEngine.SocialPlatforms.IScore;
using UISP = UnityEngine.SocialPlatforms.ISocialPlatform;
using UIUP = UnityEngine.SocialPlatforms.IUserProfile;
using UUS  = UnityEngine.SocialPlatforms.UserScope;

using ACSC  = GooglePlayGames.Api.CommonStatusCodes;
using ADNCC = GooglePlayGames.Api.Nearby.DummyNearbyConnectionClient;
using AFLVS = GooglePlayGames.Api.FriendsListVisibilityStatus;
using AIEC  = GooglePlayGames.Api.Events.IEventsClient;
using AINCC = GooglePlayGames.Api.Nearby.INearbyConnectionClient;
using AIPGC = GooglePlayGames.Api.IPlayGamesClient;
using AISGC = GooglePlayGames.Api.SavedGame.ISavedGameClient;
using ALC   = GooglePlayGames.Api.LeaderboardCollection;
using ALFS  = GooglePlayGames.Api.LoadFriendsStatus;
using ALS   = GooglePlayGames.Api.LeaderboardStart;
using ALSD  = GooglePlayGames.Api.LeaderboardScoreData;
using ALTS  = GooglePlayGames.Api.LeaderboardTimeSpan;
using APS   = GooglePlayGames.Api.PlayerStats;
using ARA   = GooglePlayGames.Api.RecallAccess;
using ARS   = GooglePlayGames.Api.ResponseStatus;
using ASIS  = GooglePlayGames.Api.SignInStatus;
using ASPC  = GooglePlayGames.Api.ScorePageCursor;
using AUS   = GooglePlayGames.Api.UiStatus;

namespace GooglePlayGames {

    public sealed class PlayGamesPlatform : UISP {

        private static volatile GPGP  s_instance;
        private static volatile bool  s_nearbyPending;
        private static volatile AINCC s_nearbyClient;

        private readonly AIPGC                      m_client               = null;
        private          string                     m_defaultLeaderboardId = null;
        private readonly Dictionary<string, string> m_idMap                = new();

        public static bool DebugLogEnabled
        {
            get => Logger.DebugLogEnabled;
            set => Logger.DebugLogEnabled = value;
        }

        public static GPGP Instance
        {
            get {
                if (s_instance == null) {
                    Logger.d("Initializing the PlayGamesPlatform instance.");
                    s_instance = new GPGP(GPGCF.GetPlatformPlayGamesClient());
                }
                return s_instance;
            }
        }

        public static AINCC Nearby
        {
            get {
                if (s_nearbyClient == null && !s_nearbyPending) {
                    s_nearbyPending = true;
                    InitializeNearby(null);
                }
                return s_nearbyClient;
            }
        }

        public static GPGP Activate()
        {
            Logger.d("Activating PlayGamesPlatform.");
            Social.Active = Instance;
            Logger.d("PlayGamesPlatform activated: " + Social.Active);
            return Instance;
        }

        public static void InitializeNearby(Action<AINCC> callback)
        {
            Logger.d("Calling InitializeNearby!");
            if (s_nearbyClient == null) {
#if UNITY_ANDROID && !UNITY_EDITOR
                NearbyConnectionClientFactory.Create(client => {
                    Logger.d("Nearby Client Created!!");
                    s_nearbyClient = client;
                    if (callback != null) {
                        callback.Invoke(client);
                    } else {
                        Logger.d("Initialize Nearby callback is null");
                    }
                });
#else
                s_nearbyClient = new ADNCC();
                callback?.Invoke(s_nearbyClient);
#endif
            } else {
                Logger.d("Nearby Already initialized: calling callback directly");
                callback?.Invoke(s_nearbyClient);
            }
        }

        private PlayGamesPlatform()
        {
            Logger.d("Creating new PlayGamesPlatform");
            LocalUser = new GPGLU(this);
        }

        internal PlayGamesPlatform(AIPGC client)
        {
            m_client  = Misc.CheckNotNull(client);
            LocalUser = new GPGLU(this);
        }

        public UILU LocalUser { get; }

        public AIEC  Events    => m_client.GetEventsClient();
        public AISGC SavedGame => m_client.GetSavedGameClient();

        public void MapId(string fromId, string toId) => m_idMap[fromId] = toId;

        public void AskForLoadFriendsResolution(Action<AUS> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("AskForLoadFriendsResolution can only be called after authentication.");
                Utility.RunUiAction(callback, AUS.NotAuthorized);
                return;
            }
            Logger.d("AskForLoadFriendsResolution callback is " + callback);
            m_client.AskForLoadFriendsResolution(callback);
        }

        public void Authenticate(Action<ASIS> callback) => m_client.Authenticate(callback);

        internal UIUP[] GetFriends()
        {
            if (!IsAuthenticated()) {
                Logger.d("Cannot get friends when not authenticated!");
                return new UIUP[0];
            }
            return m_client.GetFriends();
        }

        public void GetFriendsListVisibility(bool forceReload, Action<AFLVS> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("GetFriendsListVisibility can only be called after authentication.");
                Utility.RunUiAction(callback, AFLVS.NotAuthorized);
                return;
            }
            Logger.d("GetFriendsListVisibility, callback is " + callback);
            m_client.GetFriendsListVisibility(forceReload, callback);
        }

        public ALFS GetLastLoadFriendsStatus()
        {
            if (!IsAuthenticated()) {
                Logger.e("GetLastLoadFriendsStatus can only be called after authentication.");
                return ALFS.NotAuthorized;
            }
            return m_client.GetLastLoadFriendsStatus();
        }

        public void GetPlayerStats(Action<ACSC, APS> callback)
        {
            if (m_client != null && m_client.IsAuthenticated()) {
                m_client.GetPlayerStats(callback);
            } else {
                Logger.e("GetPlayerStats can only be called after authentication.");
                callback(ACSC.SignInRequired, new APS());
            }
        }

        public string GetUserDisplayName()
        {
            if (!IsAuthenticated()) {
                Logger.e("GetUserDisplayName can only be called after authentication.");
                return string.Empty;
            }
            return m_client.GetUserDisplayName();
        }

        public string GetUserId()
        {
            if (!IsAuthenticated()) {
                Logger.e("GetUserId() can only be called after authentication.");
                return "0";
            }
            return m_client.GetUserId();
        }

        public string GetUserImageUrl()
        {
            if (!IsAuthenticated()) {
                Logger.e("GetUserImageUrl can only be called after authentication.");
                return null;
            }
            return m_client.GetUserImageUrl();
        }

        public void IncrementAchievement(string id, int steps, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("IncrementAchievement can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d($"IncrementAchievement: {id}, steps " + steps);
            id = GetMapId(id);
            m_client.IncrementAchievement(id, steps, callback);
        }

        public bool IsAuthenticated() => m_client != null && m_client.IsAuthenticated();

        public void LoadFriends(int size, bool reload, Action<ALFS> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadFriends can only be called after authentication.");
                Utility.RunUiAction(callback, ALFS.NotAuthorized);
                return;
            }
            m_client.LoadFriends(size, reload, callback);
        }

        public void LoadMoreFriends(int size, Action<ALFS> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadMoreFriends can only be called after authentication.");
                Utility.RunUiAction(callback, ALFS.NotAuthorized);
                return;
            }
            m_client.LoadMoreFriends(size, callback);
        }

        public void LoadMoreScores(ASPC cursor, int rowCount, Action<ALSD> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadMoreScores can only be called after authentication.");
                callback(new ALSD(cursor.LeaderboardId, ARS.NotAuthorized));
                return;
            }
            m_client.LoadMoreScores(cursor, rowCount, callback);
        }

        public void LoadScores(UIL leaderboard, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadScores can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            var board = (GPGL)leaderboard;
            var span  = Utility.ToAndroidLeaderboardTimeSpan(board.TimeScope);
            board.IsLoading = true;
            Logger.d($"LoadScores, board={board} callback is " + callback);

            var start      = ALS.PlayerCentered;
            var size       = board.Range.count > 0 ? board.Range.count : m_client.LeaderboardMaxResults();
            var collection = board.UserScope == UUS.FriendsOnly ? ALC.Social : ALC.Public;
            m_client.LoadScores(board.Id, start, size, collection, span, CreateLoadScoresHandler(board, callback));
        }

        private Action<ALSD> CreateLoadScoresHandler(UIL leaderboard, Action<bool> callback)
        {
            return (data) => UnityLoadScoresHandler(leaderboard, data, callback);
        }

        private void UnityLoadScoresHandler(UIL leaderboard, ALSD data, Action<bool> callback)
        {
            GoogleLoadScoresHandler((GPGL)leaderboard, data, callback);
        }

        private void GoogleLoadScoresHandler(GPGL leaderboard, ALSD data, Action<bool> callback)
        {
            var ok = leaderboard.ImportLeaderboardScoreData(data);
            if (ok && !leaderboard.HasAllScores() && data.NextPageCursor != null) {
                var size = leaderboard.Range.count - leaderboard.ScoreCount;
                m_client.LoadMoreScores(data.NextPageCursor, size, (it) => GoogleLoadScoresHandler(leaderboard, data: it, callback));
            } else {
                callback?.Invoke(ok);
            }
        }

        public void ManuallyAuthenticate(Action<ASIS> callback) => m_client.ManuallyAuthenticate(callback);

        private string GetMapId(string id)
        {
            if (id == null) return null;
            if (m_idMap.ContainsKey(id)) {
                var result = m_idMap[id];
                Logger.d($"Mapping alias {id} to ID " + result);
                return result;
            }
            return id;
        }

        public void RequestRecallAccess(Action<ARA> callback)
        {
            Misc.CheckNotNull(callback);
            m_client.RequestRecallAccessToken(callback);
        }

        public void ReportScore(long score, string leaderboardId, string metadata, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ReportScore can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d($"ReportScore: score={score}, board={leaderboardId}, metadata=" + metadata);
            m_client.SubmitScore(GetMapId(leaderboardId), score, metadata, callback);
        }

        public void RequestServerSideAccess(bool refresh, Action<string> callback)
        {
            Misc.CheckNotNull(callback);
            if (!IsAuthenticated()) {
                Logger.e("RequestServerSideAccess() can only be called after authentication.");
                Utility.RunUiAction(callback, null);
                return;
            }
            m_client.RequestServerSideAccess(refresh, callback);
        }

        public void RevealAchievement(string id, Action<bool> callback = null)
        {
            if (!IsAuthenticated()) {
                Logger.e("RevealAchievement can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d("RevealAchievement: " + id);
            m_client.RevealAchievement(GetMapId(id), callback);
        }

        public void SetDefaultLeaderboardForUi(string id)
        {
            Logger.d("SetDefaultLeaderboardForUi: " + id);
            m_defaultLeaderboardId = GetMapId(id);
        }

        public void SetStepsAtLeast(string achievementId, int steps, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("SetStepsAtLeast can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d($"SetStepsAtLeast: {achievementId}, steps " + steps);
            m_client.SetStepsAtLeast(GetMapId(achievementId), steps, callback);
        }

        public void ShowAchievementsUi(Action<AUS> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ShowAchievementsUi can only be called after authentication.");
                return;
            }
            Logger.d("ShowAchievementsUi callback is " + callback);
            m_client.ShowAchievementsUI(callback);
        }

        public void ShowCompareProfileWithAlternativeNameHintsUi(string userId, string comparandUserName, string userName, Action<AUS> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ShowCompareProfileWithAlternativeNameHintsUi can only be called after authentication.");
                Utility.RunUiAction(callback, AUS.NotAuthorized);
                return;
            }
            Logger.d($"ShowCompareProfileWithAlternativeNameHintsUi, userId={userId} callback is " + callback);
            m_client.ShowCompareProfileWithAlternativeNameHintsUI(userId, comparandUserName, userName, callback);
        }

        public void ShowLeaderboardUi(string id) => ShowLeaderboardUi(GetMapId(id), ALTS.AllTime, null);

        public void ShowLeaderboardUi(string id, Action<AUS> callback) => ShowLeaderboardUi(id, ALTS.AllTime, callback);

        public void ShowLeaderboardUi(string id, ALTS span, Action<AUS> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ShowLeaderboardUI can only be called after authentication.");
                callback?.Invoke(AUS.NotAuthorized);
                return;
            }
            Logger.d($"ShowLeaderboardUI, lbId={id} callback is " + callback);
            m_client.ShowLeaderboardUI(id, span, callback);
        }

        public void UnlockAchievement(string id, Action<bool> callback = null)
        {
            if (!IsAuthenticated()) {
                Logger.e("UnlockAchievement can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d("UnlockAchievement: " + id);
            m_client.UnlockAchievement(GetMapId(id), callback);
        }

        #region Backward compatibility layer

        [Obsolete("Use MapId(string, string) instead")]
        public void AddIdMapping(string fromId, string toId) => MapId(fromId, toId);

        [Obsolete("Use SetDefaultLeaderboardForUi(string) instead")]
        public void SetDefaultLeaderboardForUI(string id) => SetDefaultLeaderboardForUi(id);

        [Obsolete("Use ShowAchievementsUi() instead")]
        public void ShowAchievementsUI() => ShowAchievementsUi();

        [Obsolete("Use ShowAchievementsUi(Action<UiStatus>) instead")]
        public void ShowAchievementsUI(Action<AUS> callback) => ShowAchievementsUi(callback);

        [Obsolete("Use ShowCompareProfileWithAlternativeNameHintsUi(string, string, string, Action<UiStatus>) instead")]
        public void ShowCompareProfileWithAlternativeNameHintsUI(string userId, string comparandUserName, string userName, Action<AUS> callback)
        {
            ShowCompareProfileWithAlternativeNameHintsUi(userId, comparandUserName, userName, callback);
        }

        [Obsolete("Use ShowLeaderboardUi(string) instead")]
        public void ShowLeaderboardUI(string id) => ShowLeaderboardUi(id);

        [Obsolete("Use ShowLeaderboardUi(string, Action<UiStatus>) instead")]
        public void ShowLeaderboardUI(string id, Action<AUS> callback) => ShowLeaderboardUi(id, callback);

        [Obsolete("Use ShowLeaderboardUi(string, LeaderboardTimeSpan, Action<UiStatus>) instead")]
        public void ShowLeaderboardUI(string id, ALTS span, Action<AUS> callback) => ShowLeaderboardUi(id, span, callback);

        [Obsolete("Use ShowLeaderboardUi() instead")]
        public void ShowLeaderboardUI() => ShowLeaderboardUi();

        #endregion Backward compatibility layer

        #region ISocialPlatform implementation

        [Obsolete("Use LocalUser instead")]
        public UILU localUser => LocalUser;

        public void Authenticate(UILU _, Action<bool> callback) => Authenticate(it => callback(it == ASIS.Success));

        public void Authenticate(UILU _, Action<bool, string> callback) => Authenticate(it => callback(it == ASIS.Success, it.ToString()));

        public UIA CreateAchievement() => new GPGA();

        public UIL CreateLeaderboard() => new GPGL(m_defaultLeaderboardId);

        public bool GetLoading(UIL board) => board != null && board.loading;

        public void LoadAchievements(Action<UIA[]> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadAchievements can only be called after authentication.");
                callback?.Invoke(null);
                return;
            }
            m_client.LoadAchievements(ach => {
                var data = ach.Select(it => new GPGA(it)).ToArray<UIA>();
                callback?.Invoke(data);
            });
        }

        public void LoadAchievementDescriptions(Action<UIAD[]> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadAchievementDescriptions can only be called after authentication.");
                callback?.Invoke(null);
                return;
            }
            m_client.LoadAchievements(ach => {
                var data = ach.Select(it => new GPGA(it)).ToArray<UIAD>();
                callback?.Invoke(data);
            });
        }

        public void LoadFriends(UILU user, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadScores can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            m_client.LoadFriends(callback);
        }

        public void LoadScores(string leaderboardId, Action<UIS[]> callback)
        {
            var start      = ALS.PlayerCentered;
            var size       = m_client.LeaderboardMaxResults();
            var collection = ALC.Public;
            var span       = ALTS.AllTime;
            LoadScores(leaderboardId, start, size, collection, span, (data) => callback(data.Scores));
        }

        public void LoadScores(string leaderboardId, ALS start, int size, ALC collection, ALTS span, Action<ALSD> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadScores can only be called after authentication.");
                callback?.Invoke(new ALSD(leaderboardId, ARS.NotAuthorized));
                return;
            }
            m_client.LoadScores(leaderboardId, start, size, collection, span, callback);
        }

        public void LoadUsers(string[] ids, Action<UIUP[]> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("GetUserId() can only be called after authentication.");
                callback?.Invoke(new UIUP[0]);
                return;
            }
            m_client.LoadUsers(ids, callback);
        }

        public void ReportProgress(string achievementId, double progress, Action<bool> callback)
        {
            callback = Utility.ToUiAction(callback);

            if (!IsAuthenticated()) {
                Logger.e("ReportProgress can only be called after authentication.");
                callback.Invoke(false);
                return;
            }

            Logger.d($"ReportProgress, {achievementId}, " + progress);
            achievementId = GetMapId(achievementId);

            if (progress < 0.000001) {
                Logger.d("Progress 0.00 interpreted as request to reveal.");
                m_client.RevealAchievement(achievementId, callback);
                return;
            }

            m_client.LoadAchievements(achievement => {
                for (var i = 0; i < achievement.Length; i += 1) {
                    if (achievement[i].Id == achievementId) {
                        if (achievement[i].IsIncremental) {
                            Logger.d($"Progress {progress} interpreted as incremental target (approximate).");
                            if (progress >= 0.0 && progress <= 1.0) {
                                Logger.w($"Progress {progress} is less than or equal to 1. " +
                                          "You might be trying to use values in the range of [0,1], while values are expected to be within the range [0,100]. " +
                                          "If you are using the latter, you can safely ignore this message.");
                            }
                            var steps = Utility.ToAndroidAchievementSteps(progress, achievement[i].TotalSteps);
                            m_client.SetStepsAtLeast(achievementId, steps, callback);
                        } else {
                            if (progress >= 100) {
                                Logger.d($"Progress {progress} interpreted as UNLOCK.");
                                m_client.UnlockAchievement(achievementId, callback);
                            } else {
                                Logger.d($"Progress {progress} not enough to unlock non-incremental achievement.");
                                callback.Invoke(false);
                            }
                        }
                        return;
                    }
                }
                Logger.e("Unable to locate achievement " + achievementId);
                callback.Invoke(false);
            });
        }

        public void ReportScore(long score, string leaderboardId, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ReportScore can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d($"ReportScore: score={score}, board=" + leaderboardId);
            m_client.SubmitScore(GetMapId(leaderboardId), score, callback);
        }

        public void ShowAchievementsUi() => ShowAchievementsUi(null);

        public void ShowLeaderboardUi()
        {
            Logger.d("ShowLeaderboardUI with default identifier");
            ShowLeaderboardUi(GetMapId(m_defaultLeaderboardId), null);
        }

        #endregion ISocialPlatform implementation

    }

}