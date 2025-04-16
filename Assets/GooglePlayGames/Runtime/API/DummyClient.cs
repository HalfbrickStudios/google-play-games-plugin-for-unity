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

using System;

using GooglePlayGames.Utils;

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
using APS   = GooglePlayGames.Api.PlayerStats;
using ARA   = GooglePlayGames.Api.RecallAccess;
using ARS   = GooglePlayGames.Api.ResponseStatus;
using ASIS  = GooglePlayGames.Api.SignInStatus;
using ASPC  = GooglePlayGames.Api.ScorePageCursor;
using AUS   = GooglePlayGames.Api.UiStatus;

namespace GooglePlayGames.Api {

    internal sealed class DummyClient : AIPGC {

        private static void LogUsage() => Logger.d("Received method call on DummyClient - using stub implementation.");

        #region IPlayGamesClient implementation

        public void AskForLoadFriendsResolution(Action<AUS> callback)
        {
            LogUsage();
            callback?.Invoke(AUS.VersionUpdateRequired);
        }

        public void Authenticate(Action<ASIS> callback)
        {
            LogUsage();
            callback?.Invoke(ASIS.Canceled);
        }

        public AIEC GetEventsClient()
        {
            LogUsage();
            return null;
        }

        public UIUP[] GetFriends()
        {
            LogUsage();
            return new UIUP[0];
        }

        public void GetFriendsListVisibility(bool reload, Action<AFLVS> callback)
        {
            LogUsage();
            callback?.Invoke(AFLVS.Unknown);
        }

        public ALFS GetLastLoadFriendsStatus()
        {
            LogUsage();
            return ALFS.Unknown;
        }

        public void GetPlayerStats(Action<ACSC, APS> callback)
        {
            LogUsage();
            callback?.Invoke(ACSC.ApiNotConnected, new APS());
        }

        public AISGC GetSavedGameClient()
        {
            LogUsage();
            return null;
        }

        public string GetUserDisplayName()
        {
            LogUsage();
            return "Player";
        }

        public string GetUserId()
        {
            LogUsage();
            return "DummyID";
        }

        public string GetUserImageUrl()
        {
            LogUsage();
            return null;
        }

        public void IncrementAchievement(string id, int steps, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public bool IsAuthenticated()
        {
            LogUsage();
            return false;
        }

        public int LeaderboardMaxResults() => 25;

        public void LoadAchievements(Action<AA[]> callback)
        {
            LogUsage();
            callback?.Invoke(null);
        }

        public void LoadFriends(Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public void LoadFriends(int size, bool reload, Action<ALFS> callback)
        {
            LogUsage();
            callback?.Invoke(ALFS.Unknown);
        }

        public void LoadMoreFriends(int size, Action<ALFS> callback)
        {
            LogUsage();
            callback?.Invoke(ALFS.Unknown);
        }

        public void LoadMoreScores(ASPC cursor, int size, Action<ALSD> callback)
        {
            LogUsage();
            callback?.Invoke(new ALSD(cursor.LeaderboardId, ARS.LicenseCheckFailed));
        }

        public void LoadScores(string leaderboardId, ALS start, int size, ALC collection, ALTS span, Action<ALSD> callback)
        {
            LogUsage();
            callback?.Invoke(new ALSD(leaderboardId, ARS.LicenseCheckFailed));
        }

        public void LoadUsers(string[] ids, Action<UIUP[]> callback)
        {
            LogUsage();
            callback?.Invoke(null);
        }

        public void ManuallyAuthenticate(Action<ASIS> callback)
        {
            LogUsage();
            callback?.Invoke(ASIS.Canceled);
        }

        public void RequestRecallAccessToken(Action<ARA> callback)
        {
            LogUsage();
            callback?.Invoke(null);
        }

        public void RequestServerSideAccess(bool refresh, Action<string> callback)
        {
            LogUsage();
            callback?.Invoke(null);
        }

        public void RevealAchievement(string id, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public void SetStepsAtLeast(string id, int steps, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        public void ShowAchievementsUI(Action<AUS> callback)
        {
            LogUsage();
            callback?.Invoke(AUS.VersionUpdateRequired);
        }

        public void ShowCompareProfileWithAlternativeNameHintsUI(string userId, string comparandUserName, string userName, Action<AUS> callback)
        {
            LogUsage();
            callback?.Invoke(AUS.VersionUpdateRequired);
        }

        public void ShowLeaderboardUI(string id, ALTS span, Action<AUS> callback)
        {
            LogUsage();
            callback?.Invoke(AUS.VersionUpdateRequired);
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

        public void UnlockAchievement(string id, Action<bool> callback)
        {
            LogUsage();
            callback?.Invoke(false);
        }

        #endregion IPlayGamesClient implementation

    }

}