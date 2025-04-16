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

#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SocialPlatforms;

using GooglePlayGames.Api;
using GooglePlayGames.Api.Events;
using GooglePlayGames.Api.Nearby;
using GooglePlayGames.Api.SavedGame;
using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

namespace GooglePlayGames {

    public sealed class PlayGamesPlatform : ISocialPlatform {

        private static volatile PlayGamesPlatform       s_instance;
        private static volatile bool                    s_nearbyPending;
        private static volatile INearbyConnectionClient s_nearbyClient;

        private readonly IPlayGamesClient           m_client               = null;
        private          string                     m_defaultLeaderboardId = null;
        private readonly Dictionary<string, string> m_idMap                = new();

        public static bool DebugLogEnabled
        {
            get => Logger.DebugLogEnabled;
            set => Logger.DebugLogEnabled = value;
        }

        public static PlayGamesPlatform Instance
        {
            get {
                if (s_instance == null) {
                    Logger.d("Initializing the PlayGamesPlatform instance.");
                    s_instance = new PlayGamesPlatform(PlayGamesClientFactory.GetPlatformPlayGamesClient());
                }
                return s_instance;
            }
        }

        public static INearbyConnectionClient Nearby
        {
            get {
                if (s_nearbyClient == null && !s_nearbyPending) {
                    s_nearbyPending = true;
                    InitializeNearby(null);
                }
                return s_nearbyClient;
            }
        }

        public static PlayGamesPlatform Activate()
        {
            Logger.d("Activating PlayGamesPlatform.");
            Social.Active = Instance;
            Logger.d("PlayGamesPlatform activated: " + Social.Active);
            return Instance;
        }

        public static void InitializeNearby(Action<INearbyConnectionClient> callback)
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
                s_nearbyClient = new DummyNearbyConnectionClient();
                callback?.Invoke(s_nearbyClient);
#endif
            } else if (callback != null) {
                Logger.d("Nearby Already initialized: calling callback directly");
                callback.Invoke(s_nearbyClient);
            } else {
                Logger.d("Nearby Already initialized");
            }
        }

        private static void InvokeCallbackOnGameThread<T>(Action<T> callback, T data)
        {
            if (callback == null) return;
            PlayGamesHelperObject.RunOnGameThread(() => callback(data));
        }

        private static int ProgressToSteps(double progress, int totalSteps)
        {
            return (progress >= 100.0) ? totalSteps : (int)(progress * totalSteps / 100.0);
        }

        private static Action<T> ToOnGameThread<T>(Action<T> toConvert)
        {
            if (toConvert == null) return delegate { };
            return (val) => PlayGamesHelperObject.RunOnGameThread(() => toConvert(val));
        }

        private PlayGamesPlatform()
        {
            Logger.d("Creating new PlayGamesPlatform");
            LocalUser = new PlayGamesLocalUser(this);
        }

        internal PlayGamesPlatform(IPlayGamesClient client)
        {
            m_client  = Misc.CheckNotNull(client);
            LocalUser = new PlayGamesLocalUser(this);
        }

        public ILocalUser LocalUser { get; }

        public IEventsClient    Events    => m_client.GetEventsClient();
        public ISavedGameClient SavedGame => m_client.GetSavedGameClient();

        public void AddIdMapping(string fromId, string toId) => m_idMap[fromId] = toId;

        public void AskForLoadFriendsResolution(Action<UiStatus> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("AskForLoadFriendsResolution can only be called after authentication.");
                InvokeCallbackOnGameThread(callback, UiStatus.NotAuthorized);
                return;
            }
            Logger.d("AskForLoadFriendsResolution callback is " + callback);
            m_client.AskForLoadFriendsResolution(callback);
        }

        public void Authenticate(Action<SignInStatus> callback) => m_client.Authenticate(callback);

        internal IUserProfile[] GetFriends()
        {
            if (!IsAuthenticated()) {
                Logger.d("Cannot get friends when not authenticated!");
                return new IUserProfile[0];
            }
            return m_client.GetFriends();
        }

        public void GetFriendsListVisibility(bool forceReload, Action<FriendsListVisibilityStatus> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("GetFriendsListVisibility can only be called after authentication.");
                InvokeCallbackOnGameThread(callback, FriendsListVisibilityStatus.NotAuthorized);
                return;
            }
            Logger.d("GetFriendsListVisibility, callback is " + callback);
            m_client.GetFriendsListVisibility(forceReload, callback);
        }

        public LoadFriendsStatus GetLastLoadFriendsStatus()
        {
            if (!IsAuthenticated()) {
                Logger.e("GetLastLoadFriendsStatus can only be called after authentication.");
                return LoadFriendsStatus.NotAuthorized;
            }
            return m_client.GetLastLoadFriendsStatus();
        }

        public void GetPlayerStats(Action<CommonStatusCodes, PlayerStats> callback)
        {
            if (m_client != null && m_client.IsAuthenticated()) {
                m_client.GetPlayerStats(callback);
            } else {
                Logger.e("GetPlayerStats can only be called after authentication.");
                callback(CommonStatusCodes.SignInRequired, new PlayerStats());
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

        public void IncrementAchievement(string achievementID, int steps, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("IncrementAchievement can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d($"IncrementAchievement: {achievementID}, steps " + steps);
            achievementID = MapId(achievementID);
            m_client.IncrementAchievement(achievementID, steps, callback);
        }

        public bool IsAuthenticated() => m_client != null && m_client.IsAuthenticated();

        public void LoadFriends(int pageSize, bool forceReload, Action<LoadFriendsStatus> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadFriends can only be called after authentication.");
                InvokeCallbackOnGameThread(callback, LoadFriendsStatus.NotAuthorized);
                return;
            }
            m_client.LoadFriends(pageSize, forceReload, callback);
        }

        public void LoadMoreFriends(int pageSize, Action<LoadFriendsStatus> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadMoreFriends can only be called after authentication.");
                InvokeCallbackOnGameThread(callback, LoadFriendsStatus.NotAuthorized);
                return;
            }
            m_client.LoadMoreFriends(pageSize, callback);
        }

        public void LoadMoreScores(ScorePageToken token, int rowCount, Action<LeaderboardScoreData> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadMoreScores can only be called after authentication.");
                callback(new LeaderboardScoreData(token.LeaderboardId, ResponseStatus.NotAuthorized));
                return;
            }
            m_client.LoadMoreScores(token, rowCount, callback);
        }

        public void LoadScores(ILeaderboard board, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadScores can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }

            var timeSpan = LeaderboardTimeSpan.AllTime;
            switch (board.timeScope) {
                case TimeScope.AllTime:
                    timeSpan = LeaderboardTimeSpan.AllTime;
                    break;
                case TimeScope.Week:
                    timeSpan = LeaderboardTimeSpan.Weekly;
                    break;
                case TimeScope.Today:
                    timeSpan = LeaderboardTimeSpan.Daily;
                    break;
            }

            ((PlayGamesLeaderboard) board).IsLoading = true;
            Logger.d($"LoadScores, board={board} callback is " + callback);

            var start      = LeaderboardStart.PlayerCentered;
            var rowCount   = board.range.count > 0 ? board.range.count : m_client.LeaderboardMaxResults();
            var collection = board.userScope == UserScope.FriendsOnly ? LeaderboardCollection.Social : LeaderboardCollection.Public;
            m_client.LoadScores(board.id, start, rowCount, collection, timeSpan, LoadScoresHandler(board, callback));
        }

        private Action<LeaderboardScoreData> LoadScoresHandler(ILeaderboard board, Action<bool> callback)
        {
            return (scoreData) => LoadScoresHandler(board, scoreData, callback);
        }

        private void LoadScoresHandler(ILeaderboard board, LeaderboardScoreData scoreData, Action<bool> callback)
        {
            LoadScoresHandler((PlayGamesLeaderboard)board, scoreData, callback);
        }

        private void LoadScoresHandler(PlayGamesLeaderboard board, LeaderboardScoreData scoreData, Action<bool> callback)
        {
            var ok = board.SetFromData(scoreData);
            if (ok && !board.HasAllScores() && scoreData.NextPageToken != null) {
                var rowCount = board.Range.count - board.ScoreCount;
                m_client.LoadMoreScores(scoreData.NextPageToken, rowCount, (nextScoreData) => LoadScoresHandler(board, nextScoreData, callback));
            } else {
                callback(ok);
            }
        }

        public void ManuallyAuthenticate(Action<SignInStatus> callback) => m_client.ManuallyAuthenticate(callback);

        private string MapId(string id)
        {
            if (id == null) return null;
            if (m_idMap.ContainsKey(id)) {
                var result = m_idMap[id];
                Logger.d($"Mapping alias {id} to ID " + result);
                return result;
            }
            return id;
        }

        public void RequestRecallAccess(Action<RecallAccess> callback)
        {
            Misc.CheckNotNull(callback);
            m_client.RequestRecallAccessToken(callback);
        }

        public void ReportScore(long score, string board, string metadata, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ReportScore can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d($"ReportScore: score={score}, board={board}, metadata=" + metadata);
            var leaderboardId = MapId(board);
            m_client.SubmitScore(leaderboardId, score, metadata, callback);
        }

        public void RequestServerSideAccess(bool forceRefreshToken, Action<string> callback)
        {
            Misc.CheckNotNull(callback);
            if (!IsAuthenticated()) {
                Logger.e("RequestServerSideAccess() can only be called after authentication.");
                InvokeCallbackOnGameThread(callback, null);
                return;
            }
            m_client.RequestServerSideAccess(forceRefreshToken, callback);
        }

        public void RevealAchievement(string achievementID, Action<bool> callback = null)
        {
            if (!IsAuthenticated()) {
                Logger.e("RevealAchievement can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d("RevealAchievement: " + achievementID);
            achievementID = MapId(achievementID);
            m_client.RevealAchievement(achievementID, callback);
        }

        public void SetDefaultLeaderboardForUI(string lbid)
        {
            Logger.d("SetDefaultLeaderboardForUI: " + lbid);
            if (lbid != null) lbid = MapId(lbid);
            m_defaultLeaderboardId = lbid;
        }

        public void SetStepsAtLeast(string achievementID, int steps, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("SetStepsAtLeast can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d($"SetStepsAtLeast: {achievementID}, steps " + steps);
            achievementID = MapId(achievementID);
            m_client.SetStepsAtLeast(achievementID, steps, callback);
        }

        public void ShowAchievementsUI(Action<UiStatus> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ShowAchievementsUI can only be called after authentication.");
                return;
            }
            Logger.d("ShowAchievementsUI callback is " + callback);
            m_client.ShowAchievementsUI(callback);
        }

        public void ShowCompareProfileWithAlternativeNameHintsUI(string userId, string otherPlayerInGameName, string currentPlayerInGameName, Action<UiStatus> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ShowCompareProfileWithAlternativeNameHintsUI can only be called after authentication.");
                InvokeCallbackOnGameThread(callback, UiStatus.NotAuthorized);
                return;
            }
            Logger.d($"ShowCompareProfileWithAlternativeNameHintsUI, userId={userId} callback is " + callback);
            m_client.ShowCompareProfileWithAlternativeNameHintsUI(userId, otherPlayerInGameName, currentPlayerInGameName, callback);
        }

        public void ShowLeaderboardUI(string leaderboardId)
        {
            if (leaderboardId != null) leaderboardId = MapId(leaderboardId);
            ShowLeaderboardUI(leaderboardId, LeaderboardTimeSpan.AllTime, null);
        }

        public void ShowLeaderboardUI(string leaderboardId, Action<UiStatus> callback)
        {
            var start = LeaderboardTimeSpan.AllTime;
            ShowLeaderboardUI(leaderboardId, start, callback);
        }

        public void ShowLeaderboardUI(string leaderboardId, LeaderboardTimeSpan span, Action<UiStatus> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ShowLeaderboardUI can only be called after authentication.");
                callback?.Invoke(UiStatus.NotAuthorized);
                return;
            }
            Logger.d($"ShowLeaderboardUI, lbId={leaderboardId} callback is " + callback);
            m_client.ShowLeaderboardUI(leaderboardId, span, callback);
        }

        public void UnlockAchievement(string achievementID, Action<bool> callback = null)
        {
            if (!IsAuthenticated()) {
                Logger.e("UnlockAchievement can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d("UnlockAchievement: " + achievementID);
            achievementID = MapId(achievementID);
            m_client.UnlockAchievement(achievementID, callback);
        }

        #region ISocialPlatform implementation

        [Obsolete("Use LocalUser instead")]
        public ILocalUser localUser => LocalUser;

        public void Authenticate(ILocalUser unused, Action<bool> callback) => Authenticate(it => callback(it == SignInStatus.Success));

        public void Authenticate(ILocalUser unused, Action<bool, string> callback) => Authenticate(it => callback(it == SignInStatus.Success, it.ToString()));

        public IAchievement CreateAchievement() => new PlayGamesAchievement();

        public ILeaderboard CreateLeaderboard() => new PlayGamesLeaderboard(m_defaultLeaderboardId);

        public bool GetLoading(ILeaderboard board) => board != null && board.loading;

        public void LoadAchievements(Action<IAchievement[]> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadAchievements can only be called after authentication.");
                callback.Invoke(null);
                return;
            }
            m_client.LoadAchievements(ach => {
                var data = ach.Select(it => new PlayGamesAchievement(it)).ToArray<IAchievement>();
                callback.Invoke(data);
            });
        }

        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadAchievementDescriptions can only be called after authentication.");
                callback?.Invoke(null);
                return;
            }
            m_client.LoadAchievements(ach => {
                var data = ach.Select(it => new PlayGamesAchievement(it)).ToArray<IAchievementDescription>();
                callback.Invoke(data);
            });
        }

        public void LoadFriends(ILocalUser user, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadScores can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            m_client.LoadFriends(callback);
        }

        public void LoadScores(string leaderboardId, Action<IScore[]> callback)
        {
            var start      = LeaderboardStart.PlayerCentered;
            var rowCount   = m_client.LeaderboardMaxResults();
            var collection = LeaderboardCollection.Public;
            var timeSpan   = LeaderboardTimeSpan.AllTime;
            LoadScores(leaderboardId, start, rowCount, collection, timeSpan, (scoreData) => callback(scoreData.Scores));
        }

        public void LoadScores(string leaderboardId, LeaderboardStart start, int rowCount, LeaderboardCollection collection, LeaderboardTimeSpan timeSpan, Action<LeaderboardScoreData> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("LoadScores can only be called after authentication.");
                callback(new LeaderboardScoreData(leaderboardId, ResponseStatus.NotAuthorized));
                return;
            }
            m_client.LoadScores(leaderboardId, start, rowCount, collection, timeSpan, callback);
        }

        public void LoadUsers(string[] userIds, Action<IUserProfile[]> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("GetUserId() can only be called after authentication.");
                callback(new IUserProfile[0]);
                return;
            }
            m_client.LoadUsers(userIds, callback);
        }

        public void ReportProgress(string achievementID, double progress, Action<bool> callback)
        {
            callback = ToOnGameThread(callback);
            if (!IsAuthenticated()) {
                Logger.e("ReportProgress can only be called after authentication.");
                callback.Invoke(false);
                return;
            }
            Logger.d($"ReportProgress, {achievementID}, " + progress);
            achievementID = MapId(achievementID);
            if (progress < 0.000001) {
                Logger.d("Progress 0.00 interpreted as request to reveal.");
                m_client.RevealAchievement(achievementID, callback);
                return;
            }
            m_client.LoadAchievements(ach => {
                for (int i = 0; i < ach.Length; i++) {
                    if (ach[i].Id == achievementID) {
                        if (ach[i].IsIncremental) {
                            Logger.d($"Progress {progress} interpreted as incremental target (approximate).");
                            if (progress >= 0.0 && progress <= 1.0) {
                                Logger.w($"Progress {progress} is less than or equal to 1. " +
                                          "You might be trying to use values in the range of [0,1], while values are expected to be within the range [0,100]. " +
                                          "If you are using the latter, you can safely ignore this message.");
                            }
                            m_client.SetStepsAtLeast(achievementID, ProgressToSteps(progress, ach[i].TotalSteps), callback);
                        } else {
                            if (progress >= 100) {
                                Logger.d($"Progress {progress} interpreted as UNLOCK.");
                                m_client.UnlockAchievement(achievementID, callback);
                            } else {
                                Logger.d($"Progress {progress} not enough to unlock non-incremental achievement.");
                                callback.Invoke(false);
                            }
                        }
                        return;
                    }
                }
                Logger.e("Unable to locate achievement " + achievementID);
                callback.Invoke(false);
            });
        }

        public void ReportScore(long score, string board, Action<bool> callback)
        {
            if (!IsAuthenticated()) {
                Logger.e("ReportScore can only be called after authentication.");
                callback?.Invoke(false);
                return;
            }
            Logger.d($"ReportScore: score={score}, board=" + board);
            var leaderboardId = MapId(board);
            m_client.SubmitScore(leaderboardId, score, callback);
        }

        public void ShowAchievementsUI() => ShowAchievementsUI(null);

        public void ShowLeaderboardUI()
        {
            Logger.d("ShowLeaderboardUI with default ID");
            ShowLeaderboardUI(MapId(m_defaultLeaderboardId), null);
        }

        #endregion ISocialPlatform implementation

    }

}

#endif