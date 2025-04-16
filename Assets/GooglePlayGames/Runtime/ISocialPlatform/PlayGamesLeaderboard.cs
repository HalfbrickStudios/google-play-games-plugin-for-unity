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

#if UNITY_ANDROID

using System;
using System.Collections.Generic;

using UnityEngine.SocialPlatforms;

using GooglePlayGames.Api;

using Range = UnityEngine.SocialPlatforms.Range;

namespace GooglePlayGames {

    public sealed class PlayGamesLeaderboard : ILeaderboard {

        public PlayGamesLeaderboard(string id)
        {
            Id = id;
        }

        private          string[]             m_filteredUserIds = null;
        private readonly List<PlayGamesScore> m_scores          = new();

        public string    Id             { get; private  set; }
        public bool      IsLoading      { get; internal set; }
        public IScore    LocalUserScore { get; private  set; }
        public uint      MaxRange       { get; private  set; }
        public Range     Range          { get; private  set; }
        public TimeScope TimeScope      { get; private  set; }
        public string    Title          { get; private  set; }
        public UserScope UserScope      { get; private  set; }

        public  IScore[] Scores     => m_scores.ToArray();
        public  int      ScoreCount => m_scores.Count;

        [Obsolete("Use IsLoading instead")]
        public bool Loading
        {
            get => IsLoading;
            private set => IsLoading = value;
        }

        internal int AddScore(PlayGamesScore score)
        {
            if (m_filteredUserIds == null || m_filteredUserIds.Length == 0) {
                m_scores.Add(score);
            } else {
                foreach (var fid in m_filteredUserIds) {
                    if (fid.Equals(score.UserId)) {
                        m_scores.Add(score);
                        break;
                    }
                }
            }
            return m_scores.Count;
        }

        internal bool HasAllScores() => m_scores.Count >= Range.count || m_scores.Count >= MaxRange;

        internal bool SetFromData(LeaderboardScoreData data)
        {
            if (data.IsValid) {
                Utils.Logger.d("Setting leaderboard from: " + data);
                SetMaxRange(data.ApproximateCount);
                SetTitle(data.Title);
                SetLocalUserScore((PlayGamesScore) data.PlayerScore);
                foreach (var score in data.Scores) {
                    AddScore((PlayGamesScore) score);
                }
                IsLoading = data.Scores.Length == 0 || HasAllScores();
            }
            return data.IsValid;
        }

        internal void SetLocalUserScore(PlayGamesScore score) => LocalUserScore = score;

        internal void SetMaxRange(ulong val) => MaxRange = (uint)val;

        internal void SetTitle(string value) => Title = value;

        #region ILeaderboard implementation

        [Obsolete("Use Id instead")]             public string    id             { get => Id;             set => Id        = value; }
        [Obsolete("Use IsLoading instead")]      public bool      loading        { get => IsLoading;                                }
        [Obsolete("Use LocalUserScore instead")] public IScore    localUserScore { get => LocalUserScore;                           }
        [Obsolete("Use MaxRange instead")]       public uint      maxRange       { get => MaxRange;                                 }
        [Obsolete("Use Range instead")]          public Range     range          { get => Range;          set => Range     = value; }
        [Obsolete("Use Scores instead")]         public IScore[]  scores         { get => Scores;                                   }
        [Obsolete("Use TimeScope instead")]      public TimeScope timeScope      { get => TimeScope;      set => TimeScope = value; }
        [Obsolete("Use Title instead")]          public string    title          { get => Title;                                    }
        [Obsolete("Use UserScope instead")]      public UserScope userScope      { get => UserScope;      set => UserScope = value; }

        public void LoadScores(Action<bool> callback) => PlayGamesPlatform.Instance.LoadScores(this, callback);

        public void SetUserFilter(string[] userIDs) => m_filteredUserIds = userIDs;

        #endregion

    }

}

#endif