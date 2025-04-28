// <copyright file="NearbyConnectionConfiguration.cs" company="Google Inc.">
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

using AIS  = GooglePlayGames.Api.Nearby.InitializationStatus;
using ANCC = GooglePlayGames.Api.Nearby.NearbyConnectionConfiguration;

namespace GooglePlayGames.Api.Nearby {

    public enum InitializationStatus {
        InternalError         = 2,
        Success               = 0,
        VersionUpdateRequired = 1,
    }

    public readonly struct NearbyConnectionConfiguration {

        public const int MaxReliableMessagePayloadLength   = 4096;
        public const int MaxUnreliableMessagePayloadLength = 1168;

        public NearbyConnectionConfiguration(Action<AIS> callback, long clientId)
        {
            InitializationCallback = Misc.CheckNotNull(callback);
            LocalClientId          = clientId;
        }

        public Action<AIS> InitializationCallback { get; }
        public long        LocalClientId          { get; }

        #region Object implementation

        public override string ToString() => $"NearbyConnectionConfiguration(InitializationCallback: {InitializationCallback}, LocalClientId: {LocalClientId})";
        
        public override int GetHashCode() => HashCode.Combine(InitializationCallback, LocalClientId);

        public override bool Equals(object other)
        {
            if (other is not ANCC it) return false;
            return Utility.Equals(InitializationCallback, it.InitializationCallback) &&
                   Utility.Equals(LocalClientId,          it.LocalClientId);
        }

        #endregion Object implementation

    }

}