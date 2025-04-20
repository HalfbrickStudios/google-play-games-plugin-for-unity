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

using System;
using System.Collections.Generic;

using UnityEngine.SocialPlatforms;

using ALSD = GooglePlayGames.Api.LeaderboardScoreData;
using APGS = GooglePlayGames.PlayGamesScore;
using ARS  = GooglePlayGames.Api.ResponseStatus;
using ASPC = GooglePlayGames.Api.ScorePageCursor;

namespace GooglePlayGames.Api {

    public sealed class LeaderboardScoreData {

        private readonly List<APGS> m_scores = new();

        internal LeaderboardScoreData(string leaderboardId)
        {
            Id = leaderboardId;
        }

        internal LeaderboardScoreData(string leaderboardId, ARS status) : this(leaderboardId)
        {
            Status = status;
        }

        public ulong  ApproximateCount   { get; internal set; }
        public string Id                 { get; internal set; }
        public ASPC   NextPageCursor     { get; internal set; }
        public IScore PlayerScore        { get; internal set; }
        public ASPC   PreviousPageCursor { get; internal set; }
        public ARS    Status             { get; internal set; }
        public string Title              { get; internal set; }

        public IScore[] Scores  => m_scores.ToArray();
        public bool     IsValid => Status == ARS.Success || Status == ARS.SuccessWithStale;

        internal int AddScore(APGS score)
        {
            m_scores.Add(score);
            return m_scores.Count;
        }

        #region Backward compatibility layer

        [Obsolete("Use IsValid instead")]
        public bool Valid => IsValid;

        [Obsolete("Use NextPageCursor instead")]
        public ASPC NextPageToken
        {
                     get => NextPageCursor;
            internal set => NextPageCursor = value;
        }

        [Obsolete("Use PreviousPageCursor instead")]
        public ASPC PrevPageToken
        {
                     get => PreviousPageCursor;
            internal set => PreviousPageCursor = value;
        }

        #endregion Backward compatibility layer

        #region Object implementation

        public override string ToString() => $"LeaderboardScoreData(ApproximateCount: {ApproximateCount}, Id: {Id}, NextPageCursor: {NextPageCursor}, PlayerScore: {PlayerScore}, PreviousPageCursor: {PreviousPageCursor}, Status: {Status}, Title: {Title})";

        public override int GetHashCode() => HashCode.Combine(ApproximateCount, Id, NextPageCursor, PlayerScore, PreviousPageCursor, Status, Title);

        public override bool Equals(object other)
        {
            if (other is not ALSD it) return false;
            return ApproximateCount   ==     it.ApproximateCount    &&
                   Id                .Equals(it.Id)                 &&
                   NextPageCursor    .Equals(it.NextPageCursor)     &&
                   PlayerScore       .Equals(it.PlayerScore)        &&
                   PreviousPageCursor.Equals(it.PreviousPageCursor) &&
                   Status             ==     it.Status              &&
                   Title             .Equals(it.Title);
        }

        #endregion Object implementation

    }

}