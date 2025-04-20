// <copyright file="PlayGamesLeaderboard.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc. All Rights Reserved.
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

using UIL = UnityEngine.SocialPlatforms.ILeaderboard;
using UIS = UnityEngine.SocialPlatforms.IScore;
using UR  = UnityEngine.SocialPlatforms.Range;
using UTS = UnityEngine.SocialPlatforms.TimeScope;
using UUS = UnityEngine.SocialPlatforms.UserScope;

using GPGL = GooglePlayGames.PlayGamesLeaderboard;
using GPGP = GooglePlayGames.PlayGamesPlatform;
using GPGS = GooglePlayGames.PlayGamesScore;

using ALSD = GooglePlayGames.Api.LeaderboardScoreData;

namespace GooglePlayGames {

    public sealed class PlayGamesLeaderboard : UIL {

        public PlayGamesLeaderboard(string id)
        {
            Id = id;
        }

        private          string[]   m_users  = null;
        private readonly List<GPGS> m_scores = new();

        public string Id             { get; private  set; }
        public bool   IsLoading      { get; internal set; }
        public UIS    LocalUserScore { get; private  set; }
        public uint   MaxRange       { get; private  set; }
        public UR     Range          { get; private  set; }
        public UTS    TimeScope      { get; private  set; }
        public string Title          { get; private  set; }
        public UUS    UserScope      { get; private  set; }

        public  UIS[] Scores     => m_scores.ToArray();
        public  int   ScoreCount => m_scores.Count;

        internal int AddScore(GPGS score)
        {
            if (m_users == null || m_users.Length == 0) {
                m_scores.Add(score);
            } else {
                foreach (var uid in m_users) {
                    if (uid.Equals(score.UserId)) {
                        m_scores.Add(score);
                        break;
                    }
                }
            }
            return m_scores.Count;
        }

        internal bool HasAllScores() => m_scores.Count >= Range.count || m_scores.Count >= MaxRange;

        internal bool ImportLeaderboardScoreData(ALSD data)
        {
            if (data.IsValid) {
                Utils.Logger.d("Setting leaderboard from: " + data);
                SetMaxRange(data.ApproximateCount);
                SetTitle(data.Title);
                SetLocalUserScore((GPGS)data.PlayerScore);
                foreach (var score in data.Scores) {
                    AddScore((GPGS)score);
                }
                IsLoading = data.Scores.Length == 0 || HasAllScores();
            }
            return data.IsValid;
        }

        internal void SetLocalUserScore(GPGS value) => LocalUserScore = value;

        internal void SetMaxRange(ulong value) => MaxRange = (uint)value;

        internal void SetTitle(string value) => Title = value;

        #region Backward compatibility layer

        [Obsolete("Use IsLoading instead")]
        public bool Loading
        {
            get => IsLoading;
            private set => IsLoading = value;
        }

        #endregion Backward compatibility layer

        #region ILeaderboard implementation

        [Obsolete("Use Id instead")]             public string id             { get => Id;             set => Id        = value; }
        [Obsolete("Use IsLoading instead")]      public bool   loading        { get => IsLoading;                                }
        [Obsolete("Use LocalUserScore instead")] public UIS    localUserScore { get => LocalUserScore;                           }
        [Obsolete("Use MaxRange instead")]       public uint   maxRange       { get => MaxRange;                                 }
        [Obsolete("Use Range instead")]          public UR     range          { get => Range;          set => Range     = value; }
        [Obsolete("Use Scores instead")]         public UIS[]  scores         { get => Scores;                                   }
        [Obsolete("Use TimeScope instead")]      public UTS    timeScope      { get => TimeScope;      set => TimeScope = value; }
        [Obsolete("Use Title instead")]          public string title          { get => Title;                                    }
        [Obsolete("Use UserScope instead")]      public UUS    userScope      { get => UserScope;      set => UserScope = value; }

        public void LoadScores(Action<bool> callback) => GPGP.Instance.LoadScores(this, callback);

        public void SetUserFilter(string[] ids) => m_users = ids;

        #endregion

        #region Object implementation

        public override string ToString() => $"PlayGamesLeaderboard(Id: {Id}, IsLoading: {IsLoading}, LocalUserScore: {LocalUserScore}, MaxRange: {MaxRange}, Range: {Range}, scores: {m_scores}, TimeScope: {TimeScope}, Title: {Title}, UserScope: {UserScope}, users: {m_users})";

        public override int GetHashCode()
        {
            var hash1 = HashCode.Combine(Id,       IsLoading, LocalUserScore, MaxRange,  Range);
            var hash2 = HashCode.Combine(m_scores, TimeScope, Title,          UserScope, m_users);
            return HashCode.Combine(hash1, hash2);
        }

        public override bool Equals(object other)
        {
            if (other is not GPGL it) return false;
            return Id            .Equals(it.Id)             &&
                   IsLoading      ==     it.IsLoading       &&
                   LocalUserScore.Equals(it.LocalUserScore) &&
                   MaxRange       ==     it.MaxRange        &&
                   Range         .Equals(it.Range)          &&
                   m_scores      .Equals(it.m_scores)       &&
                   TimeScope      ==     it.TimeScope       &&
                   Title         .Equals(it.Title)          &&
                   UserScope      ==     it.UserScope       &&
                   m_users        ==     it.m_users;
        }

        #endregion Object implementation

    }

}