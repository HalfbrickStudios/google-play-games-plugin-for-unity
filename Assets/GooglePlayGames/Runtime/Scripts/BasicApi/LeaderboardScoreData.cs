// <copyright file="LeaderboardScoreData.cs" company="Google Inc.">
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

using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames.BasicApi {

    public sealed class LeaderboardScoreData {

        internal LeaderboardScoreData(string leaderboardId)
        {
            Id = leaderboardId;
        }

        internal LeaderboardScoreData(string leaderboardId, ResponseStatus status)
        {
            Id     = leaderboardId;
            Status = status;
        }

        private readonly List<PlayGamesScore> m_scores = new();

        public ulong          ApproximateCount { get; internal set; }
        public string         Id               { get; internal set; }
        public ScorePageToken NextPageToken    { get; internal set; }
        public IScore         PlayerScore      { get; internal set; }
        public ScorePageToken PrevPageToken    { get; internal set; }
        public ResponseStatus Status           { get; internal set; }
        public string         Title            { get; internal set; }

        public IScore[] Scores => m_scores.ToArray();
        public bool     Valid  => Status == ResponseStatus.Success || Status == ResponseStatus.SuccessWithStale;

        internal int AddScore(PlayGamesScore score)
        {
            m_scores.Add(score);
            return m_scores.Count;
        }

        public override string ToString() => $"[LeaderboardScoreData: mId={Id}, mStatus={Status}, mApproxCount={ApproximateCount}, mTitle={Title}]";

    }

}

#endif