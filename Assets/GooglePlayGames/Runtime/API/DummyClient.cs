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
using ADC   = GooglePlayGames.Api.DummyClient;
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

        private void LogDummy(string method)
        {
            Logger.t($"NO-OP: Dummy implementation called INearbyConnectionClient.{method}");
        }

        #region IPlayGamesClient implementation

        public void AskForLoadFriendsResolution(Action<AUS> callback)
        {
            LogDummy("AskForLoadFriendsResolution(Action<UiStatus>)");
            callback?.Invoke(AUS.VersionUpdateRequired);
        }

        public void Authenticate(Action<ASIS> callback)
        {
            LogDummy("Authenticate(Action<SignInStatus>)");
            callback?.Invoke(ASIS.Canceled);
        }

        public AIEC GetEventsClient()
        {
            LogDummy("GetEventsClient()");
            return null;
        }

        public UIUP[] GetFriends()
        {
            LogDummy("GetFriends()");
            return new UIUP[0];
        }

        public void GetFriendsListVisibility(bool reload, Action<AFLVS> callback)
        {
            LogDummy("GetFriendsListVisibility(bool, Action<FriendsListVisibilityStatus>)");
            callback?.Invoke(AFLVS.Unknown);
        }

        public ALFS GetLastLoadFriendsStatus()
        {
            LogDummy("GetLastLoadFriendsStatus()");
            return ALFS.Unknown;
        }

        public void GetPlayerStats(Action<ACSC, APS> callback)
        {
            LogDummy("GetPlayerStats(Action<CommonStatusCodes, PlayerStats>)");
            callback?.Invoke(ACSC.ApiNotConnected, new APS());
        }

        public AISGC GetSavedGameClient()
        {
            LogDummy("GetSavedGameClient()");
            return null;
        }

        public string GetUserDisplayName()
        {
            LogDummy("GetUserDisplayName()");
            return "Player";
        }

        public string GetUserId()
        {
            LogDummy("GetUserId()");
            return "DummyID";
        }

        public string GetUserImageUrl()
        {
            LogDummy("GetUserImageUrl()");
            return null;
        }

        public void IncrementAchievement(string id, int steps, Action<bool> callback)
        {
            LogDummy("IncrementAchievement(string, int, Action<bool>)");
            callback?.Invoke(false);
        }

        public bool IsAuthenticated()
        {
            LogDummy("IsAuthenticated()");
            return false;
        }

        public int LeaderboardMaxResults()
        {
            LogDummy("LeaderboardMaxResults()");
            return 25;
        }

        public void LoadAchievements(Action<AA[]> callback)
        {
            LogDummy("LoadAchievements(Action<Achievement[]>)");
            callback?.Invoke(null);
        }

        public void LoadFriends(Action<bool> callback)
        {
            LogDummy("LoadFriends(Action<bool>)");
            callback?.Invoke(false);
        }

        public void LoadFriends(int size, bool reload, Action<ALFS> callback)
        {
            LogDummy("LoadFriends(int, bool, Action<LoadFriendsStatus>)");
            callback?.Invoke(ALFS.Unknown);
        }

        public void LoadMoreFriends(int size, Action<ALFS> callback)
        {
            LogDummy("LoadFriends(int, Action<LoadFriendsStatus>)");
            callback?.Invoke(ALFS.Unknown);
        }

        public void LoadMoreScores(ASPC cursor, int size, Action<ALSD> callback)
        {
            LogDummy("LoadMoreScores(ScorePageCursor, int, Action<LeaderboardScoreData>)");
            callback?.Invoke(new ALSD(cursor.LeaderboardId, ARS.LicenseCheckFailed));
        }

        public void LoadScores(string leaderboardId, ALS start, int size, ALC collection, ALTS span, Action<ALSD> callback)
        {
            LogDummy("LoadScores(string, LeaderboardStart, int, LeaderboardCollection, LeaderboardTimeSpan, Action<LeaderboardScoreData>)");
            callback?.Invoke(new ALSD(leaderboardId, ARS.LicenseCheckFailed));
        }

        public void LoadUsers(string[] ids, Action<UIUP[]> callback)
        {
            LogDummy("LoadUsers(string[], Action<IUserProfile[]>)");
            callback?.Invoke(null);
        }

        public void ManuallyAuthenticate(Action<ASIS> callback)
        {
            LogDummy("ManuallyAuthenticate(Action<SignInStatus>)");
            callback?.Invoke(ASIS.Canceled);
        }

        public void RequestRecallAccessToken(Action<ARA> callback)
        {
            LogDummy("RequestRecallAccessToken(Action<RecallAccess>)");
            callback?.Invoke(null);
        }

        public void RequestServerSideAccess(bool refresh, Action<string> callback)
        {
            LogDummy("RequestServerSideAccess(bool, Action<string>)");
            callback?.Invoke(null);
        }

        public void RevealAchievement(string id, Action<bool> callback)
        {
            LogDummy("RevealAchievement(string, Action<bool>)");
            callback?.Invoke(false);
        }

        public void SetStepsAtLeast(string id, int steps, Action<bool> callback)
        {
            LogDummy("SetStepsAtLeast(string, int, Action<bool>)");
            callback?.Invoke(false);
        }

        public void ShowAchievementsUI(Action<AUS> callback)
        {
            LogDummy("ShowAchievementsUI(Action<UiStatus>)");
            callback?.Invoke(AUS.VersionUpdateRequired);
        }

        public void ShowCompareProfileWithAlternativeNameHintsUI(string userId, string comparandUserName, string userName, Action<AUS> callback)
        {
            LogDummy("ShowCompareProfileWithAlternativeNameHintsUI(string, string, string, Action<UiStatus>)");
            callback?.Invoke(AUS.VersionUpdateRequired);
        }

        public void ShowLeaderboardUI(string id, ALTS span, Action<AUS> callback)
        {
            LogDummy("ShowLeaderboardUI(string, LeaderboardTimeSpan, Action<UiStatus>)");
            callback?.Invoke(AUS.VersionUpdateRequired);
        }

        public void SubmitScore(string leaderboardId, long score, Action<bool> callback)
        {
            LogDummy("SubmitScore(string, long, Action<bool>)");
            callback?.Invoke(false);
        }

        public void SubmitScore(string leaderboardId, long score, string metadata, Action<bool> callback)
        {
            LogDummy("SubmitScore(string, long, string, Action<bool>)");
            callback?.Invoke(false);
        }

        public void UnlockAchievement(string id, Action<bool> callback)
        {
            LogDummy("UnlockAchievement(string, Action<bool>)");
            callback?.Invoke(false);
        }

        #endregion IPlayGamesClient implementation

        #region Object implementation

        public override string ToString() => "DummyClient()";

        public override int GetHashCode() => HashCode.Combine(GetType(), ToString());

        public override bool Equals(object other) => other is ADC;

        #endregion Object implementation

    }

}