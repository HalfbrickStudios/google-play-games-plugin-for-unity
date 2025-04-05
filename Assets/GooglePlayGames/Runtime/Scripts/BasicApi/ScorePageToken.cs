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

#if UNITY_ANDROID

namespace GooglePlayGames.BasicApi {

    public enum ScorePageDirection {
        Backward = 2,
        Forward  = 1,
    }

    public sealed class ScorePageToken {

        internal ScorePageToken(object internalObject, string id, LeaderboardCollection collection, LeaderboardTimeSpan timespan, ScorePageDirection direction)
        {
            Collection     = collection;
            Direction      = direction;
            InternalObject = internalObject;
            LeaderboardId  = id;
            TimeSpan       = timespan;
        }

        public   LeaderboardCollection Collection     { get; private set; }
        public   ScorePageDirection    Direction      { get; private set; }
        internal object                InternalObject { get; private set; }
        public   string                LeaderboardId  { get; private set; }
        public   LeaderboardTimeSpan   TimeSpan       { get; private set; }

    }

}

#endif