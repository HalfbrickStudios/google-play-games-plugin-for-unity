// <copyright file="AndroidTokenClient.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc.
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
//  limitations under the License.
// </copyright>

using UTS = UnityEngine.SocialPlatforms.TimeScope;

using ALTS = GooglePlayGames.Api.LeaderboardTimeSpan;

namespace GooglePlayGames.Utils {

    internal static partial class Utility {

        public static int ToAndroidAchievementSteps(double progress, int steps) => progress >= 100.0 ? steps : (int)(progress * steps / 100.0);

        public static ALTS ToAndroidLeaderboardTimeSpan(UTS scope) => scope switch
        {
            UTS.AllTime => ALTS.AllTime,
            UTS.Week    => ALTS.Weekly,
            UTS.Today   => ALTS.Daily,
            _           => ALTS.AllTime,
        };

    }

}