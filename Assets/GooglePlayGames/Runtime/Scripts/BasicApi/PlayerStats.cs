// <copyright file="PlayerStats.cs" company="Google Inc.">
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
//    limitations under the License.
// </copyright>

#if UNITY_ANDROID

namespace GooglePlayGames.BasicApi {

    public sealed class PlayerStats {

        private static readonly float UNSET_VALUE = -1.0f;

        public PlayerStats(int numberOfPurchases, float avgSessionLength, int daysSinceLastPlayed, int numberOfSessions, float sessPercentile, float spendPercentile, float spendProbability, float churnProbability, float highSpenderProbability, float totalSpendNext28Days)
        {
            AvgSessionLength       = avgSessionLength;
            ChurnProbability       = churnProbability;
            DaysSinceLastPlayed    = daysSinceLastPlayed;
            HighSpenderProbability = highSpenderProbability;
            NumberOfPurchases      = numberOfPurchases;
            NumberOfSessions       = numberOfSessions;
            SessPercentile         = sessPercentile;
            SpendPercentile        = spendPercentile;
            SpendProbability       = spendProbability;
            TotalSpendNext28Days   = totalSpendNext28Days;
            Valid                  = true;
        }

        public PlayerStats()
        {
            Valid = false;
        }

        public float AvgSessionLength       { get; private set; }
        public float ChurnProbability       { get; private set; }
        public int   DaysSinceLastPlayed    { get; private set; }
        public float HighSpenderProbability { get; private set; }
        public int   NumberOfPurchases      { get; private set; }
        public int   NumberOfSessions       { get; private set; }
        public float SessPercentile         { get; private set; }
        public float SpendPercentile        { get; private set; }
        public float SpendProbability       { get; private set; }
        public float TotalSpendNext28Days   { get; private set; }
        public bool  Valid                  { get; private set; }

        public bool HasAvgSessionLength()       => AvgSessionLength       !=       UNSET_VALUE;
        public bool HasChurnProbability()       => ChurnProbability       !=       UNSET_VALUE;
        public bool HasDaysSinceLastPlayed()    => DaysSinceLastPlayed    != (int) UNSET_VALUE;
        public bool HasHighSpenderProbability() => HighSpenderProbability !=       UNSET_VALUE;
        public bool HasNumberOfPurchases()      => NumberOfPurchases      != (int) UNSET_VALUE;
        public bool HasNumberOfSessions()       => NumberOfSessions       != (int) UNSET_VALUE;
        public bool HasSessPercentile()         => SessPercentile         !=       UNSET_VALUE;
        public bool HasSpendPercentile()        => SpendPercentile        !=       UNSET_VALUE;
        public bool HasTotalSpendNext28Days()   => TotalSpendNext28Days   !=       UNSET_VALUE;

    }

}

#endif