#if UNITY_ANDROID

using System;
using System.Collections.Generic;

using GooglePlayGames.Android.Java.Extensions;
using GooglePlayGames.Api.Nearby;
using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

using AAR    = GooglePlayGames.Api.Nearby.AdvertisingResult;
using ACRes  = GooglePlayGames.Api.Nearby.ConnectionResponse;
using ACReq  = GooglePlayGames.Api.Nearby.ConnectionRequest;
using AIDL   = GooglePlayGames.Api.Nearby.IDiscoveryListener;
using AIML   = GooglePlayGames.Api.Nearby.IMessageListener;
using ANCCfg = GooglePlayGames.Api.Nearby.NearbyConnectionConfiguration;

using ANCC = GooglePlayGames.Android.NearbyConnectionClient;

using JAO    = GooglePlayGames.Android.Java.AdvertisingOptions;
using JAOI   = GooglePlayGames.Android.Java.AdvertisingOptions.Instance;
using JCCI   = GooglePlayGames.Android.Java.ConnectionsClient.Instance;
using JCLCP  = GooglePlayGames.Android.Java.ConnectionLifecycleCallbackProxy;
using JCLCPC = GooglePlayGames.Android.Java.ConnectionLifecycleCallbackProxy.Callback;
using JDO    = GooglePlayGames.Android.Java.DiscoveryOptions;
using JDOI   = GooglePlayGames.Android.Java.DiscoveryOptions.Instance;
using JEDCP  = GooglePlayGames.Android.Java.EndpointDiscoveryCallbackProxy;
using JEDCPC = GooglePlayGames.Android.Java.EndpointDiscoveryCallbackProxy.Callback;
using JN     = GooglePlayGames.Android.Java.Nearby;
using JP     = GooglePlayGames.Android.Java.Payload;
using JPCP   = GooglePlayGames.Android.Java.PayloadCallbackProxy;
using JPCPC  = GooglePlayGames.Android.Java.PayloadCallbackProxy.Callback;
using JS     = GooglePlayGames.Android.Java.Strategy;
using JUP    = GooglePlayGames.Android.Java.UnityPlayer;

namespace GooglePlayGames.Android {

    public class NearbyConnectionClient : INearbyConnectionClient {

        private readonly static int    ApplicationInfoFlags = 0x00000080;
        private readonly static string ServiceId            = ReadServiceId();
        private readonly static string ServiceIdKey         = "com.google.android.gms.nearby.connection.SERVICE_ID";

        private  readonly JCCI m_client;
        internal          AIML MessageListener;

        private static string ReadServiceId()
        {
            using var jActivity = JUP.JCurrentActivity;
                  var package   = jActivity.GetPackageName();
            using var jManager  = jActivity.JGetPackageManager();
            using var jInfo     = jManager.JGetApplicationInfo(package, ApplicationInfoFlags);
            using var jBundle   = jInfo.JMetaData;
                  var id        = jBundle.GetString(ServiceIdKey);
            Logger.d("SystemId from Manifest: " + id);
            return id;
        }

        public NearbyConnectionClient()
        {
            PlayGamesHelperObject.CreateObject();
            NearbyHelperObject.CreateObject(this);
            m_client = JN.JGetConnectionsClient();
        }

        private JAOI CreateAdvertisingOptions()
        {
            using var jStrategy = JS.JP2P_CLUSTER;
            using var jBuilder  = JAO.MakeBuilder();
            return jBuilder.JSetStrategy(jStrategy).JBuild();
        }

        private JDOI CreateDiscoveryOptions()
        {
            using var jStrategy = JS.JP2P_CLUSTER;
            using var jBuilder  = JDO.MakeBuilder();
            return jBuilder.JSetStrategy(jStrategy).JBuild();
        }

        private void InternalSend(List<string> ids, byte[] payload)
        {
            Misc.CheckNotNull(ids, nameof(ids));
            Misc.CheckNotNull(payload, nameof(payload));
            using var jIds     = Utility.ToJavaStringList(ids);
            using var jPayload = JP.JFromBytes(payload);
            using var jTask    = m_client.JSendPayload(jIds, jPayload);
        }

        #region INearbyConnectionClient implementation

        public void AcceptConnectionRequest(string id, byte[] payload, AIML listener)
        {
            const string method = "INearbyConnectionClient.AcceptConnectionRequest(string, byte[], IMessageListener)";
            Logger.t($"AND: Calling {method}");

            Misc.CheckNotNull(listener, nameof(listener));

            MessageListener     = new UiMessageListener(listener);
                  var proxy     = JPCPC.MakeProxy(listener: listener);
            using var jCallback = JPCP.MakeInstance(proxy);
            using var jTask     = m_client.JAcceptConnection(id, jCallback);
        }

        public void DisconnectFromEndpoint(string id)
        {
            const string method = "INearbyConnectionClient.DisconnectFromEndpoint(string)";
            Logger.t($"AND: Calling {method}");

            Misc.CheckNotNull(id, nameof(id));

            m_client.DisconnectFromEndpoint(id);
        }

        public string GetAppBundleId()
        {
            const string method = "INearbyConnectionClient.GetAppBundleId()";
            Logger.t($"AND: Calling {method}");

            using var jActivity = JUP.JCurrentActivity;
            return jActivity.GetPackageName();
        }

        public string GetServiceId()
        {
            const string method = "INearbyConnectionClient.GetServiceId()";
            Logger.t($"AND: Calling {method}");

            return ServiceId;
        }

        public int MaxReliableMessagePayloadLength()
        {
            const string method = "INearbyConnectionClient.MaxReliableMessagePayloadLength()";
            Logger.t($"AND: Calling {method}");

            return ANCCfg.MaxReliableMessagePayloadLength;
        }

        public int MaxUnreliableMessagePayloadLength()
        {
            const string method = "INearbyConnectionClient.MaxUnreliableMessagePayloadLength()";
            Logger.t($"AND: Calling {method}");

            return ANCCfg.MaxUnreliableMessagePayloadLength;
        }

        public void RejectConnectionRequest(string id)
        {
            const string method = "INearbyConnectionClient.RejectConnectionRequest(string)";
            Logger.t($"AND: Calling {method}");

            Misc.CheckNotNull(id, nameof(id));
            
            using var jTask = m_client.JRejectConnection(id);
        }

        public void SendConnectionRequest(string name, string id, byte[] payload, Action<ACRes> onCallback, AIML listener)
        {
            const string method = "INearbyConnectionClient.SendConnectionRequest(string, string, byte[], Action<ConnectionResponse>, IMessageListener)";
            Logger.t($"AND: Calling {method}");

            Misc.CheckNotNull(listener, nameof(listener));

                  var wrapped   = new UiMessageListener(listener);
                  var jProxy    = JCLCPC.MakeProxy(m_client, listener, onCallback);
            using var jCallback = JCLCP.MakeInstance(jProxy);
            using var jTask     = m_client.JRequestConnection(name, id, jCallback);
        }

        public void SendReliable(List<string> ids, byte[] payload)
        {
            const string method = "INearbyConnectionClient.SendReliable(List<string>, byte[])";
            Logger.t($"AND: Calling {method}");

            InternalSend(ids, payload);
        }

        public void SendUnreliable(List<string> ids, byte[] payload)
        {
            const string method = "INearbyConnectionClient.SendUnreliable(List<string>, byte[])";
            Logger.t($"AND: Calling {method}");

            InternalSend(ids, payload);
        }

        public void StartAdvertising(string name, List<string> ids, TimeSpan? duration, Action<AAR> resultCallback, Action<ACReq> requestCallback)
        {
            const string method = "INearbyConnectionClient.StartAdvertising(string, List<string>, TimeSpan?, Action<AdvertisingResult>, Action<ConnectionRequest>)";
            Logger.t($"AND: Calling {method}");

            Misc.CheckNotNull(resultCallback, nameof(resultCallback));
            Misc.CheckNotNull(requestCallback, nameof(requestCallback));
            if (duration.HasValue && duration.Value.Ticks < 0) {
                throw new InvalidOperationException(nameof(duration) + " must be positive");
            }
            requestCallback = Utility.ToUiAction(requestCallback);
            resultCallback  = Utility.ToUiAction(resultCallback);

                  var proxy     = JCLCPC.MakeProxy(this, resultCallback, requestCallback);
            using var jCallback = JCLCP.MakeInstance(proxy);
            using var jOptions  = CreateAdvertisingOptions();
            using var jTask     = m_client.JStartAdvertising(name, GetServiceId(), jCallback, jOptions);
            jTask.JAddOnSuccessListener(() => {
                Logger.t($"AND: Success {method}");
                NearbyHelperObject.StartAdvertisingTimer(duration);
            });
        }

        public void StopAdvertising()
        {
            const string method = "INearbyConnectionClient.StopAdvertising()";
            Logger.t($"AND: Calling {method}");

            m_client.StopAdvertising();
            MessageListener = null;
        }

        public void StopAllConnections()
        {
            const string method = "INearbyConnectionClient.StopAllConnections()";
            Logger.t($"AND: Calling {method}");

            m_client.StopAllEndpoints();
            MessageListener = null;
        }

        public void StopDiscovery(string id)
        {
            const string method = "INearbyConnectionClient.StopDiscovery(string)";
            Logger.t($"AND: Calling {method}");

            m_client.StopDiscovery();
        }

        public void StartDiscovery(string id, TimeSpan? duration, AIDL listener)
        {
            const string method = "INearbyConnectionClient.StartDiscovery(string, TimeSpan?, IDiscoveryListener)";
            Logger.t($"AND: Calling {method}");

            Misc.CheckNotNull(id, nameof(id));
            Misc.CheckNotNull(listener, nameof(listener));
            if (duration.HasValue && duration.Value.Ticks < 0) {
                throw new InvalidOperationException(nameof(duration) + " must be positive");
            }

                  var wrapped   = new UiDiscoveryListener(listener);
                  var proxy     = JEDCPC.MakeProxy(wrapped);
            using var jCallback = JEDCP.MakeInstance(proxy);
            using var jOptions  = CreateDiscoveryOptions();
            using var jTask     = m_client.JStartDiscovery(id, jCallback, jOptions);
            jTask.JAddOnSuccessListener(() => {
                Logger.t($"AND: Success {method}");
                NearbyHelperObject.StartDiscoveryTimer(duration);
            });
        }

        #endregion INearbyConnectionClient implementation

        #region Object implementation

        private bool m_stringify = false;

        public override string ToString()
        {
            if (m_stringify) return $"NearbyConnectionClient(...)";
            try {
                m_stringify = true;
                return $"NearbyConnectionClient(client: {m_client}, listener: {MessageListener})";
            } finally {
                m_stringify = false;
            }
        }

        public override int GetHashCode() => HashCode.Combine(m_client, MessageListener);

        public override bool Equals(object other)
        {
            if (other is not ANCC it) return false;
            return Utility.Equals(m_client,        it.m_client) &&
                   Utility.Equals(MessageListener, it.MessageListener);
        }

        #endregion Object implementation

    }

}

#endif