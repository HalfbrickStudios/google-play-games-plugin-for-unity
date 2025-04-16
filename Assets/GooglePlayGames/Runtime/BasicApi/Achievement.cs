// <copyright file="Achievement.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
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

namespace GooglePlayGames.BasicApi {

    public sealed class Achievement {

        public Achievement()
        {
            CurrentSteps     = 0;
            Description      = string.Empty;
            Id               = string.Empty;
            IsIncremental    = false;
            IsRevealed       = false;
            IsUnlocked       = false;
            LastModifiedTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            Name             = string.Empty;
            TotalSteps       = 0;
        }

        public int      CurrentSteps     { get; set; }
        public string   Description      { get; set; }
        public string   Id               { get; set; }
        public bool     IsIncremental    { get; set; }
        public bool     IsRevealed       { get; set; }
        public bool     IsUnlocked       { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string   Name             { get; set; }
        public int      Points           { get; set; }
        public string   RevealedImageUrl { get; set; }
        public int      TotalSteps       { get; set; }
        public string   UnlockedImageUrl { get; set; }

        #region Object implementation

        public override string ToString()
        {
            var type = IsIncremental ? "INCREMENTAL" : "STANDARD";
            return $"[Achievement] id={Id}, name={Name}, desc={Description}, type={type}, revealed={IsRevealed}, unlocked={IsUnlocked}, steps={CurrentSteps}/{TotalSteps}";
        }

        #endregion Object implementation

    }

}

#endif