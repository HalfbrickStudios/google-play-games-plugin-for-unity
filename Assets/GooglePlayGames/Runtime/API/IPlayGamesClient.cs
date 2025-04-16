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

using System;

using UIUP = UnityEngine.SocialPlatforms.IUserProfile;

using AA    = GooglePlayGames.Api.Achievement;
using ACSC  = GooglePlayGames.Api.CommonStatusCodes;
using AFLVS = GooglePlayGames.Api.FriendsListVisibilityStatus;
using AIEC  = GooglePlayGames.Api.Events.IEventsClient;
using AISGC = GooglePlayGames.Api.SavedGame.ISavedGameClient;
using ALC   = GooglePlayGames.Api.LeaderboardCollection;
using ALFS  = GooglePlayGames.Api.LoadFriendsStatus;
using ALS   = GooglePlayGames.Api.LeaderboardStart;
using ALSD  = GooglePlayGames.Api.LeaderboardScoreData;
using ALTS  = GooglePlayGames.Api.LeaderboardTimeSpan;
using APS   = GooglePlayGames.Api.PlayerStats;
using ARA   = GooglePlayGames.Api.RecallAccess;
using ASIS  = GooglePlayGames.Api.SignInStatus;
using ASPC  = GooglePlayGames.Api.ScorePageCursor;
using AUS   = GooglePlayGames.Api.UiStatus;

namespace GooglePlayGames.Api {

    public interface IPlayGamesClient {
      
        void AskForLoadFriendsResolution(Action<AUS> callback);
        
        void Authenticate(Action<ASIS> callback);

        AIEC GetEventsClient();

        UIUP[] GetFriends();

        void GetFriendsListVisibility(bool reload, Action<AFLVS> callback);
        
        ALFS GetLastLoadFriendsStatus();

        void GetPlayerStats(Action<ACSC, APS> callback);

        AISGC GetSavedGameClient();

        string GetUserDisplayName();

        string GetUserId();

        string GetUserImageUrl();

        int LeaderboardMaxResults();

        void LoadAchievements(Action<AA[]> callback);

        void LoadFriends(Action<bool> callback);

        void LoadFriends(int size, bool reload, Action<ALFS> callback);

        void LoadMoreFriends(int size, Action<ALFS> callback);

        void LoadMoreScores(ASPC cursor, int size, Action<ALSD> callback);

        void LoadScores(string leaderboardId, ALS start, int size, ALC collection, ALTS span, Action<ALSD> callback);

        void LoadUsers(string[] users, Action<UIUP[]> callback);

        void IncrementAchievement(string id, int steps, Action<bool> callback);

        bool IsAuthenticated();

        void ManuallyAuthenticate(Action<ASIS> callback);

        void RequestRecallAccessToken(Action<ARA> callback);

        void RequestServerSideAccess(bool refresh, Action<string> callback);

        void RevealAchievement(string id, Action<bool> callback);

        void SetStepsAtLeast(string id, int steps, Action<bool> callback);

        void ShowAchievementsUI(Action<AUS> callback);

        void ShowCompareProfileWithAlternativeNameHintsUI(string userId, string comparandUserName, string userName, Action<AUS> callback);

        void ShowLeaderboardUI(string id, ALTS span, Action<AUS> callback);

        void SubmitScore(string leaderboardId, long score, Action<bool> callback);

        void SubmitScore(string leaderboardId, long score, string metadata, Action<bool> callback);

        void UnlockAchievement(string id, Action<bool> callback);

    }

}