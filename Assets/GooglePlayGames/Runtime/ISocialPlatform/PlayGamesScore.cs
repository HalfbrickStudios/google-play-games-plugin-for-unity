// <copyright file="PlayGamesScore.cs" company="Google Inc.">
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

using System;

using GooglePlayGames.Utils;

using GPGP = GooglePlayGames.PlayGamesPlatform;
using GPGS = GooglePlayGames.PlayGamesScore;

using UIS = UnityEngine.SocialPlatforms.IScore;

namespace GooglePlayGames {

    public sealed class PlayGamesScore : UIS {

        internal PlayGamesScore(DateTime date, string leaderboardId, ulong rank, string playerId, ulong value, string metadata)
        {
            Date          =       date;
            LeaderboardId =       leaderboardId;
            MetaData      =       metadata;
            Rank          =  (int)rank;
            UserId        =       playerId;
            Value         = (long)value;
        }

        public DateTime Date           { get;      }
        public string   LeaderboardId  { get; set; }
        public string   MetaData       { get;      }
        public int      Rank           { get;      }
        public string   UserId         { get;      }
        public long     Value          { get; set; }

        public string FormattedValue => Value.ToString();

        #region IScore implementation

        [Obsolete("Use Date instead")]           public DateTime date           { get => Date;                                         }
        [Obsolete("Use FormattedValue instead")] public string   formattedValue { get => FormattedValue;                               }
        [Obsolete("Use LeaderboardId instead")]  public string   leaderboardID  { get => LeaderboardId;  set => LeaderboardId = value; }
        [Obsolete("Use MetaData instead")]       public string   metaData       { get => MetaData;                                     }
        [Obsolete("Use Rank instead")]           public int      rank           { get => Rank;                                         }
        [Obsolete("Use UserId instead")]         public string   userID         { get => UserId;                                       }
        [Obsolete("Use Value instead")]          public long     value          { get => Value;          set => Value         = value; }

        public void ReportScore(Action<bool> callback) => GPGP.Instance.ReportScore(Value, LeaderboardId, MetaData, callback);

        #endregion IScore implementation

        #region Object implementation

        public override string ToString() => $"PlayGamesScore(Date: {Date}, LeaderboardId: {LeaderboardId}, MetaData: {MetaData}, Rank: {Rank}, UserId: {UserId}, Value: {Value})";

        public override int GetHashCode() => HashCode.Combine(Date, LeaderboardId, MetaData, Rank, UserId, Value);

        public override bool Equals(object other)
        {
            if (other is not GPGS it) return false;
            return Utility.Equals(Date,          it.Date)          &&
                   Utility.Equals(LeaderboardId, it.LeaderboardId) &&
                   Utility.Equals(MetaData,      it.MetaData)      &&
                   Utility.Equals(Rank,          it.Rank)          &&
                   Utility.Equals(UserId,        it.UserId)        &&
                   Utility.Equals(Value,         it.Value);
        }

        #endregion Object implementation

    }

}