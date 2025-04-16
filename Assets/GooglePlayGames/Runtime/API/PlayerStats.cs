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

using System;

namespace GooglePlayGames.Api {

    public sealed class PlayerStats {

        private static readonly float UNSET_VALUE = -1.0f;

        public PlayerStats()
        {
            IsValid = false;
        }

        public PlayerStats(int purchases, float averageSession, int lastPlayed, int sessions, float sessPercentile, float spendPercentile, float spendProbability, float churn, float spender, float spendIn28Days)
        {
            AvgSessionLength       = averageSession;
            ChurnProbability       = churn;
            DaysSinceLastPlayed    = lastPlayed;
            HighSpenderProbability = spender;
            NumberOfPurchases      = purchases;
            NumberOfSessions       = sessions;
            SessPercentile         = sessPercentile;
            SpendPercentile        = spendPercentile;
            SpendProbability       = spendProbability;
            TotalSpendNext28Days   = spendIn28Days;
            IsValid                = true;
        }

        public float AvgSessionLength       { get; }
        public float ChurnProbability       { get; }
        public int   DaysSinceLastPlayed    { get; }
        public float HighSpenderProbability { get; }
        public bool  IsValid                { get; }
        public int   NumberOfPurchases      { get; }
        public int   NumberOfSessions       { get; }
        public float SessPercentile         { get; }
        public float SpendPercentile        { get; }
        public float SpendProbability       { get; }
        public float TotalSpendNext28Days   { get; }

        public bool HasAvgSessionLength()       => AvgSessionLength       !=       UNSET_VALUE;
        public bool HasChurnProbability()       => ChurnProbability       !=       UNSET_VALUE;
        public bool HasDaysSinceLastPlayed()    => DaysSinceLastPlayed    != (int) UNSET_VALUE;
        public bool HasHighSpenderProbability() => HighSpenderProbability !=       UNSET_VALUE;
        public bool HasNumberOfPurchases()      => NumberOfPurchases      != (int) UNSET_VALUE;
        public bool HasNumberOfSessions()       => NumberOfSessions       != (int) UNSET_VALUE;
        public bool HasSessPercentile()         => SessPercentile         !=       UNSET_VALUE;
        public bool HasSpendPercentile()        => SpendPercentile        !=       UNSET_VALUE;
        public bool HasTotalSpendNext28Days()   => TotalSpendNext28Days   !=       UNSET_VALUE;

        #region Backward compatibility layer

        [Obsolete("Use IsValid instead")]
        public bool Valid => IsValid;

        #endregion Backward compatibility layer

    }

}