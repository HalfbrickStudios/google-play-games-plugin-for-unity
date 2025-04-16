#if UNITY_ANDROID

using System;
using System.Collections.Generic;

using GooglePlayGames.Android.Java.Extensions;
using GooglePlayGames.Api.Nearby;
using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

using AAR   = GooglePlayGames.Api.Nearby.AdvertisingResult;
using ACRes = GooglePlayGames.Api.Nearby.ConnectionResponse;
using ACReq = GooglePlayGames.Api.Nearby.ConnectionRequest;
using AIDL  = GooglePlayGames.Api.Nearby.IDiscoveryListener;
using AIML  = GooglePlayGames.Api.Nearby.IMessageListener;
using ANCC  = GooglePlayGames.Api.Nearby.NearbyConnectionConfiguration;

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
            var package         = jActivity.GetPackageName();
            using var jManager  = jActivity.JGetPackageManager();
            using var jInfo     = jManager.JGetApplicationInfo(package, ApplicationInfoFlags);
            using var jBundle   = jInfo.JMetaData;
            var id              = jBundle.GetString(ServiceIdKey);
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
            Misc.CheckNotNull(listener, nameof(listener));
            MessageListener     = new UiMessageListener(listener);
            var proxy           = JPCPC.MakeProxy(listener: listener);
            using var jCallback = JPCP.MakeInstance(proxy);
            using var jTask     = m_client.JAcceptConnection(id, jCallback);
        }

        public void DisconnectFromEndpoint(string id)
        {
            Misc.CheckNotNull(id, nameof(id));
            m_client.DisconnectFromEndpoint(id);
        }

        public string GetAppBundleId()
        {
            using var jActivity = JUP.JCurrentActivity;
            return jActivity.GetPackageName();
        }

        public string GetServiceId() => ServiceId;

        public int MaxReliableMessagePayloadLength() => ANCC.MaxReliableMessagePayloadLength;

        public int MaxUnreliableMessagePayloadLength() => ANCC.MaxUnreliableMessagePayloadLength;

        public void RejectConnectionRequest(string id)
        {
            Misc.CheckNotNull(id, nameof(id));
            using var jTask = m_client.JRejectConnection(id);
        }

        public void SendConnectionRequest(string name, string id, byte[] payload, Action<ACRes> onCallback, AIML listener)
        {
            Misc.CheckNotNull(listener, nameof(listener));
            var wrapped         = new UiMessageListener(listener);
            var jProxy          = JCLCPC.MakeProxy(m_client, listener, onCallback);
            using var jCallback = JCLCP.MakeInstance(jProxy);
            using var jTask     = m_client.JRequestConnection(name, id, jCallback);
        }

        public void SendReliable(List<string> ids, byte[] payload) => InternalSend(ids, payload);

        public void SendUnreliable(List<string> ids, byte[] payload) => InternalSend(ids, payload);

        public void StartAdvertising(string name, List<string> ids, TimeSpan? duration, Action<AAR> resultCallback, Action<ACReq> requestCallback)
        {
            Misc.CheckNotNull(resultCallback, nameof(resultCallback));
            Misc.CheckNotNull(requestCallback, nameof(requestCallback));
            if (duration.HasValue && duration.Value.Ticks < 0) {
                throw new InvalidOperationException(nameof(duration) + " must be positive");
            }

            requestCallback = Utility.ToUiAction(requestCallback);
            resultCallback  = Utility.ToUiAction(resultCallback);

            var proxy           = JCLCPC.MakeProxy(this, resultCallback, requestCallback);
            using var jCallback = JCLCP.MakeInstance(proxy);
            using var jOptions  = CreateAdvertisingOptions();
            using var jTask     = m_client.JStartAdvertising(name, GetServiceId(), jCallback, jOptions);
            jTask.JAddOnSuccessListener(() => NearbyHelperObject.StartAdvertisingTimer(duration));
        }

        public void StopAdvertising()
        {
            m_client.StopAdvertising();
            MessageListener = null;
        }

        public void StopAllConnections()
        {
            m_client.StopAllEndpoints();
            MessageListener = null;
        }

        public void StopDiscovery(string id) => m_client.StopDiscovery();

        public void StartDiscovery(string id, TimeSpan? duration, AIDL listener)
        {
            Misc.CheckNotNull(id, nameof(id));
            Misc.CheckNotNull(listener, nameof(listener));
            if (duration.HasValue && duration.Value.Ticks < 0) {
                throw new InvalidOperationException(nameof(duration) + " must be positive");
            }

            var wrapped         = new UiDiscoveryListener(listener);
            var proxy           = JEDCPC.MakeProxy(wrapped);
            using var jCallback = JEDCP.MakeInstance(proxy);
            using var jOptions  = CreateDiscoveryOptions();
            using var jTask     = m_client.JStartDiscovery(id, jCallback, jOptions);
            jTask.JAddOnSuccessListener(() => NearbyHelperObject.StartDiscoveryTimer(duration));
        }

        #endregion INearbyConnectionClient implementation

    }

}

#endif