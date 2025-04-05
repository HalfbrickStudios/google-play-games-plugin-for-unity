// <copyright file="DummyClient.cs" company="Google Inc.">
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

using UnityEngine.SocialPlatforms;

using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi {

    public sealed class DummyClient : IPlayGamesClient {

        public void Authenticate(Action<SignInStatus> callback)
        {
            LogUsage();
            callback?.Invoke(SignInStatus.Canceled);
        }

        public void ManuallyAuthenticate(Action<SignInStatus> callback)
        {
            LogUsage();
            callback?.Invoke(SignInStatus.Canceled);
        }

        public bool IsAuthenticated()
        {
            LogUsage();
            return false;
        }

        public void RequestServerSideAccess(bool forceRefreshToken, Action<string> callback)
        {
            LogUsage();
            callback?.Invoke(null);
        }

        public void RequestRecallAccessToken(Action<RecallAccess> callback)
        {
            LogUsage();
            callback?.Invoke(null);
        }

        public string GetUserId()
        {
            LogUsage();
            return "DummyID";
        }

        public void GetPlayerStats(Action<CommonStatusCodes, PlayerStats> callback)
        {
            LogUsage();
            callback?.Invoke(CommonStatusCodes.ApiNotConnected, new PlayerStats());
        }

        public string GetUserDisplayName()
        {
            LogUsage();
            return "Player";
        }

        public string GetUserImageUrl()
        {
            LogUsage();
            return null;
        }

        public void LoadUsers(string[] userIds, Action<IUserProfile[]> callback)
        {
            LogUsage();
            callback?.Invoke(null);
        }

        public void LoadAchievements(Action<Achievement[]> callback)
        {
            LogUsage();
            callback?.Invoke(null);
        }

        public void UnlockAchievement(string achId, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public void RevealAchievement(string achId, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public void IncrementAchievement(string achId, int steps, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public void SetStepsAtLeast(string achId, int steps, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public void ShowAchievementsUI(Action<UIStatus> callback)
        {
            LogUsage();
            callback?.Invoke(UIStatus.VersionUpdateRequired);
        }

        public void AskForLoadFriendsResolution(Action<UIStatus> callback)
        {
            LogUsage();
            callback?.Invoke(UIStatus.VersionUpdateRequired);
        }

        public LoadFriendsStatus GetLastLoadFriendsStatus()
        {
            LogUsage();
            return LoadFriendsStatus.Unknown;
        }

        public void LoadFriends(int pageSize, bool forceReload, Action<LoadFriendsStatus> callback)
        {
            LogUsage();
            callback?.Invoke(LoadFriendsStatus.Unknown);
        }

        public void LoadMoreFriends(int pageSize, Action<LoadFriendsStatus> callback)
        {
            LogUsage();
            callback?.Invoke(LoadFriendsStatus.Unknown);
        }

        public void ShowCompareProfileWithAlternativeNameHintsUI(string userId, string otherPlayerInGameName, string currentPlayerInGameName, Action<UIStatus> callback)
        {
            LogUsage();
            callback?.Invoke(UIStatus.VersionUpdateRequired);
        }

        public void GetFriendsListVisibility(bool forceReload, Action<FriendsListVisibilityStatus> callback)
        {
            LogUsage();
            callback?.Invoke(FriendsListVisibilityStatus.Unknown);
        }

        public void ShowLeaderboardUI(string leaderboardId, LeaderboardTimeSpan span, Action<UIStatus> callback)
        {
            LogUsage();
            callback?.Invoke(UIStatus.VersionUpdateRequired);
        }

        public int LeaderboardMaxResults() => 25;

        public void LoadScores(string leaderboardId, LeaderboardStart start, int rowCount, LeaderboardCollection collection, LeaderboardTimeSpan timeSpan, Action<LeaderboardScoreData> callback)
        {
            LogUsage();
            callback?.Invoke(new LeaderboardScoreData(leaderboardId, ResponseStatus.LicenseCheckFailed));
        }

        public void LoadMoreScores(ScorePageToken token, int rowCount, Action<LeaderboardScoreData> callback)
        {
            LogUsage();
            callback?.Invoke(new LeaderboardScoreData(token.LeaderboardId, ResponseStatus.LicenseCheckFailed));
        }

        public void SubmitScore(string leaderboardId, long score, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public void SubmitScore(string leaderboardId, long score, string metadata, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public ISavedGameClient GetSavedGameClient()
        {
            LogUsage();
            return null;
        }

        public Events.IEventsClient GetEventsClient()
        {
            LogUsage();
            return null;
        }

        public void LoadFriends(Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public IUserProfile[] GetFriends()
        {
            LogUsage();
            return new IUserProfile[0];
        }

        private static void LogUsage() => Logger.d("Received method call on DummyClient - using stub implementation.");

    }

}

#endif