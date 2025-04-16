// <copyright file="INearbyConnectionClient.cs" company="Google Inc.">
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
using System.Collections.Generic;

using AAR   = GooglePlayGames.Api.Nearby.AdvertisingResult;
using ACReq = GooglePlayGames.Api.Nearby.ConnectionRequest;
using ACRes = GooglePlayGames.Api.Nearby.ConnectionResponse;
using AED   = GooglePlayGames.Api.Nearby.EndpointDetails;
using AIDL  = GooglePlayGames.Api.Nearby.IDiscoveryListener;
using AIML  = GooglePlayGames.Api.Nearby.IMessageListener;

namespace GooglePlayGames.Api.Nearby {

    public interface IDiscoveryListener {

        void OnEndpointFound(AED details);

        void OnEndpointLost(string endpointId);

    }

    public interface IMessageListener {

        void OnMessageReceived(string endpointId, byte[] data, bool reliable);

        void OnRemoteEndpointDisconnected(string endpointId);

    }

    public interface INearbyConnectionClient {

        void AcceptConnectionRequest(string endpointId, byte[] payload, AIML listener);

        void DisconnectFromEndpoint(string endpointId);

        string GetAppBundleId();

        string GetServiceId();

        int MaxReliableMessagePayloadLength();

        int MaxUnreliableMessagePayloadLength();

        void RejectConnectionRequest(string endpointId);

        void SendConnectionRequest(string endpointName, string endpointId, byte[] payload, Action<ACRes> callback, AIML listener);

        void SendReliable(List<string> endpointIds, byte[] payload);

        void SendUnreliable(List<string> endpointIds, byte[] payload);

        void StartAdvertising(string name, List<string> serviceIds, TimeSpan? duration, Action<AAR> onResult, Action<ACReq> onRequest);

        void StartDiscovery(string serviceId, TimeSpan? timeout, AIDL listener);

        void StopAdvertising();

        void StopAllConnections();

        void StopDiscovery(string serviceId);

    }

}