// <copyright file="ConnectionRequest.cs" company="Google Inc.">
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
using System.Linq;

using GooglePlayGames.Utils;

using ACR = GooglePlayGames.Api.Nearby.ConnectionRequest;
using AED = GooglePlayGames.Api.Nearby.EndpointDetails;

namespace GooglePlayGames.Api.Nearby {

    public readonly struct ConnectionRequest {

        public ConnectionRequest(string endpointId, string endpointName, string serviceId, byte[] payload)
        {
            Logger.d("Constructing ConnectionRequest");
            Payload        = Misc.CheckNotNull(payload);
            RemoteEndpoint = new AED(endpointId, endpointName, serviceId);
        }

        public byte[] Payload        { get; }
        public AED    RemoteEndpoint { get; }

        #region Object implementation

        public override string ToString() => $"ConnectionRequest(Payload: bytes[{Payload.Length}], RemoteEndpoint: {RemoteEndpoint})";

        public override int GetHashCode() => HashCode.Combine(Payload, RemoteEndpoint);

        public override bool Equals(object other)
        {
            if (other is not ACR it) return false;
            return Payload       .SequenceEqual (it.Payload) &&
                   RemoteEndpoint        .Equals(it.RemoteEndpoint);
        }

        #endregion Object implementation

    }

}