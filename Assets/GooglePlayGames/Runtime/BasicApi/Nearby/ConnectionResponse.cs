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

using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi.Nearby {

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

        public static ConnectionResponse Accepted(long clientId, string remoteId, byte[] payload) => new(clientId, remoteId, Status.Accepted, payload);

        public static ConnectionResponse AlreadyConnected(long clientId, string remoteId) => new(clientId, remoteId, Status.ErrorAlreadyConnected, s_emptyPayload);

        public static ConnectionResponse EndpointNotConnected(long clientId, string remoteId) => new(clientId, remoteId, Status.ErrorEndpointNotConnected, s_emptyPayload);

        public static ConnectionResponse InternalError(long clientId, string remoteId) => new(clientId, remoteId, Status.ErrorInternal, s_emptyPayload);

        public static ConnectionResponse NetworkNotConnected(long clientId, string remoteId) => new(clientId, remoteId, Status.ErrorNetworkNotConnected, s_emptyPayload);

        public static ConnectionResponse Rejected(long clientId, string remoteId) => new(clientId, remoteId, Status.Rejected, s_emptyPayload);

        private ConnectionResponse(long localClientId, string remoteEndpointId, Status code, byte[] payload)
        {
            LocalClientId    = localClientId;
            Payload          = Misc.CheckNotNull(payload);
            RemoteEndpointId = Misc.CheckNotNull(remoteEndpointId);
            ResponseStatus   = code;
        }

        public long   LocalClientId    { get; }
        public byte[] Payload          { get; }
        public string RemoteEndpointId { get; }
        public Status ResponseStatus   { get; }

    }

}