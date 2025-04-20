// <copyright file="ScorePageToken.cs" company="Google Inc.">
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

using ALC  = GooglePlayGames.Api.LeaderboardCollection;
using ALTS = GooglePlayGames.Api.LeaderboardTimeSpan;
using ASPC = GooglePlayGames.Api.ScorePageCursor;
using ASPD = GooglePlayGames.Api.ScorePageDirection;

namespace GooglePlayGames.Api {

    public enum ScorePageDirection {
        Backward = 2,
        Forward  = 1,
    }

    public sealed class ScorePageCursor {

        internal ScorePageCursor(object token, string leaderboardId, ALC collection, ALTS span, ASPD direction)
        {
            Collection    = collection;
            Direction     = direction;
            LeaderboardId = leaderboardId;
            TimeSpan      = span;
            Token         = token;
        }

        public ALC      Collection    { get; }
        public ASPD     Direction     { get; }
        public string   LeaderboardId { get; }
        public ALTS     TimeSpan      { get; }
        internal object Token         { get; }

        #region Object implementation

        public override string ToString() => $"ScorePageCursor(Collection: {Collection}, Direction: {Direction}, LeaderboardId: {LeaderboardId}, TimeSpan: {TimeSpan}, Token: {Token})";

        public override int GetHashCode() => HashCode.Combine(Collection, Direction, LeaderboardId, TimeSpan);

        public override bool Equals(object other)
        {
            if (other is not ASPC it) return false;
            return Collection    ==     it.Collection     &&
                   Direction     ==     it.Direction      &&
                   LeaderboardId.Equals(it.LeaderboardId) &&
                   TimeSpan      ==     it.TimeSpan;

        }

        #endregion Object implementation

    }

}