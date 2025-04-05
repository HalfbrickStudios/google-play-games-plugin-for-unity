// <copyright file="IPlayGamesClient.cs" company="Google Inc.">
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

using GooglePlayGames.BasicApi.Events;
using GooglePlayGames.BasicApi.SavedGame;

namespace GooglePlayGames.BasicApi {

    public interface IPlayGamesClient {
      
        void Authenticate(Action<SignInStatus> callback);

        void ManuallyAuthenticate(Action<SignInStatus> callback);

        bool IsAuthenticated();

        void RequestServerSideAccess(bool forceRefreshToken, Action<string> callback);

        void RequestRecallAccessToken(Action<RecallAccess> callback);

        string GetUserId();

        void LoadFriends(Action<bool> callback);

        string GetUserDisplayName();

        string GetUserImageUrl();

        void GetPlayerStats(Action<CommonStatusCodes, PlayerStats> callback);

        void LoadUsers(string[] userIds, Action<IUserProfile[]> callback);

        void LoadAchievements(Action<Achievement[]> callback);

        void UnlockAchievement(string achievementId, Action<bool> successOrFailureCalllback);

        void RevealAchievement(string achievementId, Action<bool> successOrFailureCalllback);

        void IncrementAchievement(string achievementId, int steps, Action<bool> successOrFailureCalllback);

        void SetStepsAtLeast(string achId, int steps, Action<bool> callback);

        void ShowAchievementsUI(Action<UIStatus> callback);

        void AskForLoadFriendsResolution(Action<UIStatus> callback);

        LoadFriendsStatus GetLastLoadFriendsStatus();

        void ShowCompareProfileWithAlternativeNameHintsUI(string otherUserId, string otherPlayerInGameName, string currentPlayerInGameName, Action<UIStatus> callback);

        void GetFriendsListVisibility(bool forceReload, Action<FriendsListVisibilityStatus> callback);

        void LoadFriends(int pageSize, bool forceReload, Action<LoadFriendsStatus> callback);

        void LoadMoreFriends(int pageSize, Action<LoadFriendsStatus> callback);

        void ShowLeaderboardUI(string leaderboardId, LeaderboardTimeSpan span, Action<UIStatus> callback);

        void LoadScores(string leaderboardId, LeaderboardStart start, int rowCount, LeaderboardCollection collection, LeaderboardTimeSpan timeSpan, Action<LeaderboardScoreData> callback);

        void LoadMoreScores(ScorePageToken token, int rowCount, Action<LeaderboardScoreData> callback);

        int LeaderboardMaxResults();

        void SubmitScore(string leaderboardId, long score, Action<bool> successOrFailureCalllback);

        void SubmitScore(string leaderboardId, long score, string metadata, Action<bool> successOrFailureCalllback);

        ISavedGameClient GetSavedGameClient();

        IEventsClient GetEventsClient();

        IUserProfile[] GetFriends();

    }

}

#endif