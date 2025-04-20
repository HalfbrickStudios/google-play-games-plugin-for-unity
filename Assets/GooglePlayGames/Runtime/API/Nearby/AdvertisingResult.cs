// <copyright file="AdvertisingResult.cs" company="Google Inc.">
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

using AR  = GooglePlayGames.Api.Nearby.AdvertisingResult;
using ARS = GooglePlayGames.Api.ResponseStatus;

namespace GooglePlayGames.Api.Nearby {

    public readonly struct AdvertisingResult {

        public AdvertisingResult(ARS status, string endpointName)
        {
            LocalEndpointName = Misc.CheckNotNull(endpointName);
            Status            = status;
        }

        public string LocalEndpointName { get; }
        public ARS    Status            { get; }

        public bool Succeeded => Status == ARS.Success;

        #region Object implementation

        public override string ToString() => $"AdvertisingResult(LocalEndpointName: {LocalEndpointName}, Status: {Status})";

        public override int GetHashCode() => HashCode.Combine(LocalEndpointName, Status);

        public override bool Equals(object other)
        {
            if (other is not AR it) return false;
            return LocalEndpointName.Equals(it.LocalEndpointName) &&
                   Status            ==     it.Status;
        }

        #endregion Object implementation

    }

}