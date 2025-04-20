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
using ADNCC = GooglePlayGames.Api.Nearby.DummyNearbyConnectionClient;
using AIDL  = GooglePlayGames.Api.Nearby.IDiscoveryListener;
using AINCC = GooglePlayGames.Api.Nearby.INearbyConnectionClient;
using AIML  = GooglePlayGames.Api.Nearby.IMessageListener;
using ANCC  = GooglePlayGames.Api.Nearby.NearbyConnectionConfiguration;

namespace GooglePlayGames.Api.Nearby {

    internal sealed class DummyNearbyConnectionClient : AINCC {

        private void LogDummy(string method)
        {
            Logger.t($"NO-OP: Dummy implementation called INearbyConnectionClient.{method}");
        }

        public void AcceptConnectionRequest(string endpointId, byte[] payload, AIML listener)
        {
            LogDummy("AcceptConnectionRequest(string, byte[], IMessageListener)");
        }

        public void DisconnectFromEndpoint(string endpointId)
        {
            LogDummy("DisconnectFromEndpoint(string)");
        }

        public string GetAppBundleId()
        {
            LogDummy("GetAppBundleId()");
            return "dummy.bundle.id";
        }

        public string GetServiceId()
        {
            LogDummy("GetServiceId()");
            return "dummy.service.id";
        }

        public string LocalDeviceId()
        {
            LogDummy("LocalDeviceId()");
            return "DummyDevice";
        }

        public string LocalEndpointId()
        {
            LogDummy("LocalEndpointId()");
            return string.Empty;
        }

        public int MaxReliableMessagePayloadLength()
        {
            LogDummy("MaxReliableMessagePayloadLength()");
            return ANCC.MaxReliableMessagePayloadLength;
        }

        public int MaxUnreliableMessagePayloadLength()
        {
            LogDummy("MaxUnreliableMessagePayloadLength()");
            return ANCC.MaxUnreliableMessagePayloadLength;
        }

        public void RejectConnectionRequest(string endpointId)
        {
            LogDummy("RejectConnectionRequest(string)");
        }

        public void SendConnectionRequest(string endpointName, string endpointId, byte[] payload, Action<ACRes> callback, AIML listener)
        {
            LogDummy("SendConnectionRequest(string, string, byte[], Action<ConnectionResponse>, IMessageListener)");
            callback?.Invoke(ACRes.Rejected(0, string.Empty));
        }

        public void SendReliable(List<string> endpointIds, byte[] payload)
        {
            LogDummy("SendReliable(List<string>, byte[])");
        }

        public void SendUnreliable(List<string> endpointIds, byte[] payload)
        {
            LogDummy("SendUnreliable(List<string>, byte[])");
        }

        public void StartAdvertising(string name, List<string> serviceIds, TimeSpan? duration, Action<AAR> onResult, Action<ACReq> onRequest)
        {
            LogDummy("StartAdvertising(string, List<string>, TimeSpan, Action<AdvertisingResult>, Action<ConnectionRequest>)");
            onResult?.Invoke(new AAR(ResponseStatus.LicenseCheckFailed, string.Empty));
        }

        public void StartDiscovery(string serviceId, TimeSpan? timeout, AIDL listener)
        {
            LogDummy("StartDiscovery(string, TimeSpan, IDiscoveryListener)");
        }

        public void StopAdvertising()
        {
            LogDummy("StopAdvertising()");
        }

        public void StopAllConnections()
        {
            LogDummy("StopAllConnections()");
        }

        public void StopDiscovery(string serviceId)
        {
            LogDummy("StopDiscovery(string)");
        }

        #region Object implementation

        public override string ToString() => "DummyNearbyConnectionClient()";

        public override int GetHashCode() => HashCode.Combine(GetType(), ToString());

        public override bool Equals(object other) => other is ADNCC;

        #endregion Object implementation

    }

}