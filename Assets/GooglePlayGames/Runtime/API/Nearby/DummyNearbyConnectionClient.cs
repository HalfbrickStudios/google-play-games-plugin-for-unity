// <copyright file="DummyNearbyConnectionClient.cs" company="Google Inc.">
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

using GooglePlayGames.Utils;

using AAR   = GooglePlayGames.Api.Nearby.AdvertisingResult;
using ACReq = GooglePlayGames.Api.Nearby.ConnectionRequest;
using ACRes = GooglePlayGames.Api.Nearby.ConnectionResponse;
using AIDL  = GooglePlayGames.Api.Nearby.IDiscoveryListener;
using AINCC = GooglePlayGames.Api.Nearby.INearbyConnectionClient;
using AIML  = GooglePlayGames.Api.Nearby.IMessageListener;
using ANCC  = GooglePlayGames.Api.Nearby.NearbyConnectionConfiguration;

namespace GooglePlayGames.Api.Nearby {

    internal sealed class DummyNearbyConnectionClient : AINCC {

        public void AcceptConnectionRequest(string endpointId, byte[] payload, AIML listener)
        {
            Logger.d("AcceptConnectionRequest in dummy implementation called");
        }

        public void DisconnectFromEndpoint(string endpointId)
        {
            Logger.d("DisconnectFromEndpoint in dummy implementation called");
        }

        public string GetAppBundleId() => "dummy.bundle.id";

        public string GetServiceId() => "dummy.service.id";

        public string LocalDeviceId() => "DummyDevice";

        public string LocalEndpointId() => string.Empty;

        public int MaxReliableMessagePayloadLength() => ANCC.MaxReliableMessagePayloadLength;

        public int MaxUnreliableMessagePayloadLength() => ANCC.MaxUnreliableMessagePayloadLength;

        public void RejectConnectionRequest(string endpointId)
        {
            Logger.d("RejectConnectionRequest in dummy implementation called");
        }

        public void SendConnectionRequest(string endpointName, string endpointId, byte[] payload, Action<ACRes> callback, AIML listener)
        {
            Logger.d("SendConnectionRequest called from dummy implementation");
            callback?.Invoke(ACRes.Rejected(0, string.Empty));
        }

        public void SendReliable(List<string> endpointIds, byte[] payload)
        {
            Logger.d("SendReliable called from dummy implementation");
        }

        public void SendUnreliable(List<string> endpointIds, byte[] payload)
        {
            Logger.d("SendUnreliable called from dummy implementation");
        }

        public void StartAdvertising(string name, List<string> serviceIds, TimeSpan? duration, Action<AAR> onResult, Action<ACReq> onRequest)
        {
            onResult?.Invoke(new AAR(ResponseStatus.LicenseCheckFailed, string.Empty));
        }

        public void StartDiscovery(string serviceId, TimeSpan? timeout, AIDL listener)
        {
            Logger.d("StartDiscovery in dummy implementation called");
        }

        public void StopAdvertising()
        {
            Logger.d("StopAvertising in dummy implementation called");
        }

        public void StopAllConnections()
        {
            Logger.d("StopAllConnections in dummy implementation called");
        }

        public void StopDiscovery(string serviceId)
        {
            Logger.d("StopDiscovery in dummy implementation called");
        }

    }

}