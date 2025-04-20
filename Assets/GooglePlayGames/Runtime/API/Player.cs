// <copyright file="Player.cs" company="Google Inc.">
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

using GPGUP = GooglePlayGames.PlayGamesUserProfile;

using AP = GooglePlayGames.Api.Player;

namespace GooglePlayGames.Api {

    internal sealed class Player : GPGUP {

        internal Player(string playerName, string playerId, string avatar) : base(playerName, playerId, avatar) { }

        #region Object implementation

        public override string ToString() => $"Player({base.ToString()})";

        public override int GetHashCode() => HashCode.Combine(GetType(), ToString());

        public override bool Equals(object other) => base.Equals(other) && other is AP;

        #endregion Object implementation

    }

}