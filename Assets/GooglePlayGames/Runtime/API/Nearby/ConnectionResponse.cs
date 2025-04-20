// <copyright file="ConnectionResponse.cs" company="Google Inc.">
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

using ACR  = GooglePlayGames.Api.Nearby.ConnectionResponse;
using ACRS = GooglePlayGames.Api.Nearby.ConnectionResponse.Status;

namespace GooglePlayGames.Api.Nearby {

    public readonly struct ConnectionResponse {

        public enum Status {
            Accepted                  = 0,
            ErrorAlreadyConnected     = 5,
            ErrorEndpointNotConnected = 4,
            ErrorInternal             = 2,
            ErrorNetworkNotConnected  = 3,
            Rejected                  = 1,
        }

        private static readonly byte[] s_emptyPayload = new byte[0];

        public static ACR Accepted(long clientId, string endpointId, byte[] payload) => new(clientId, endpointId, ACRS.Accepted, payload);

        public static ACR AlreadyConnected(long clientId, string endpointId) => new(clientId, endpointId, ACRS.ErrorAlreadyConnected, s_emptyPayload);

        public static ACR EndpointNotConnected(long clientId, string endpointId) => new(clientId, endpointId, ACRS.ErrorEndpointNotConnected, s_emptyPayload);

        public static ACR InternalError(long clientId, string endpointId) => new(clientId, endpointId, ACRS.ErrorInternal, s_emptyPayload);

        public static ACR NetworkNotConnected(long clientId, string endpointId) => new(clientId, endpointId, ACRS.ErrorNetworkNotConnected, s_emptyPayload);

        public static ACR Rejected(long clientId, string endpointId) => new(clientId, endpointId, ACRS.Rejected, s_emptyPayload);

        private ConnectionResponse(long clientId, string endpointId, ACRS status, byte[] payload)
        {
            LocalClientId    = clientId;
            Payload          = Misc.CheckNotNull(payload);
            RemoteEndpointId = Misc.CheckNotNull(endpointId);
            ResponseStatus   = status;
        }

        public long   LocalClientId    { get; }
        public byte[] Payload          { get; }
        public string RemoteEndpointId { get; }
        public ACRS   ResponseStatus   { get; }

        #region Object implementation

        public override string ToString() => $"ConnectionResponse(LocalClientId: {LocalClientId}, Payload: bytes[{Payload.Length}], RemoteEndpointId: {RemoteEndpointId}, ResponseStatus: {ResponseStatus})";

        public override int GetHashCode() => HashCode.Combine(LocalClientId, Payload, RemoteEndpointId, ResponseStatus);

        public override bool Equals(object other)
        {
            if (other is not ACR it) return false;
            return LocalClientId            ==     it.LocalClientId     &&
                   Payload         .SequenceEqual (it.Payload)          &&
                   RemoteEndpointId        .Equals(it.RemoteEndpointId) &&
                   ResponseStatus           ==     it.ResponseStatus;
        }

        #endregion Object implementation

    }

}