#if UNITY_ANDROID

using System;
using System.Diagnostics.CodeAnalysis;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;
using UAJP = UnityEngine.AndroidJavaProxy;

using AAR   = GooglePlayGames.Api.Nearby.AdvertisingResult;
using ACReq = GooglePlayGames.Api.Nearby.ConnectionRequest;
using ACRes = GooglePlayGames.Api.Nearby.ConnectionResponse;
using AIML  = GooglePlayGames.Api.Nearby.IMessageListener;
using ARS   = GooglePlayGames.Api.ResponseStatus;

using ANCC = GooglePlayGames.Android.NearbyConnectionClient;

using JCCI         = GooglePlayGames.Android.Java.ConnectionsClient.Instance;
using JCII         = GooglePlayGames.Android.Java.ConnectionInfo.Instance;
using JCLCP        = GooglePlayGames.Android.Java.ConnectionLifecycleCallbackProxy;
using JCLCPCP      = GooglePlayGames.Android.Java.ConnectionLifecycleCallbackProxy.Callback.Proxy;
using JCLCPCPAD    = GooglePlayGames.Android.Java.ConnectionLifecycleCallbackProxy.Callback.Proxy.OnAdvertisingResultDelegate;
using JCLCPCPCReqD = GooglePlayGames.Android.Java.ConnectionLifecycleCallbackProxy.Callback.Proxy.OnConnectionRequestDelegate;
using JCLCPCPCResD = GooglePlayGames.Android.Java.ConnectionLifecycleCallbackProxy.Callback.Proxy.OnConnectionResponseDelegate;
using JCLCPI       = GooglePlayGames.Android.Java.ConnectionLifecycleCallbackProxy.Instance;
using JCRI         = GooglePlayGames.Android.Java.ConnectionResolution.Instance;
using JCSC         = GooglePlayGames.Android.Java.ConnectionsStatusCodes;
using JOI          = GooglePlayGames.Android.Java.Object.Instance;
using JPCP         = GooglePlayGames.Android.Java.PayloadCallbackProxy;
using JPCPC        = GooglePlayGames.Android.Java.PayloadCallbackProxy.Callback;

namespace GooglePlayGames.Android.Java {

    internal static class ConnectionLifecycleCallbackProxy {

        public static readonly string ClassName               =  "ConnectionLifecycleCallbackProxy";
        public static readonly string PackageName             =  "com.google.games.bridge";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        internal static class Callback {

            public static readonly string ClassName               = $"{JCLCP.ClassName}$Callback";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            private static readonly long NearbyClientId = 0L;

            public static JCLCPCP MakeProxy(ANCC client, Action<AAR> onResult = null, Action<ACReq> onRequest = null)
            {
                void onResultDelegate (AAR   arg1) => onResult (arg1);
                void onRequestDelegate(ACReq arg1) => onRequest(arg1);
                return MakeProxy(client, (JCLCPCPAD)onResultDelegate, onRequestDelegate);
            }

            public static JCLCPCP MakeProxy(JCCI client, AIML listener = null, Action<ACRes> onResponse = null)
            {
                void onResponseDelegate(ACRes arg1) => onResponse(arg1);
                return MakeProxy(client, listener, (JCLCPCPCResD)onResponseDelegate);
            }

            public static JCLCPCP MakeProxy(ANCC client, JCLCPCPAD onResult = null, JCLCPCPCReqD onRequest  = null) => new(client, onResult, onRequest);
            public static JCLCPCP MakeProxy(JCCI client, AIML      listener = null, JCLCPCPCResD onResponse = null) => new(client, listener, onResponse);

            internal sealed class Proxy : UAJP {

                public delegate void OnAdvertisingResultDelegate (AAR   jResult);   // AdvertisingConnectionLifecycleCallbackProxy
                public delegate void OnConnectionRequestDelegate (ACReq jRequest);  // AdvertisingConnectionLifecycleCallbackProxy
                public delegate void OnConnectionResponseDelegate(ACRes jResponse); // DiscoveringConnectionLifecycleCallback

                public event JCLCPCPAD    OnAdvertisingResult;  // AdvertisingConnectionLifecycleCallbackProxy
                public event JCLCPCPCReqD OnConnectionRequest;  // AdvertisingConnectionLifecycleCallbackProxy
                public event JCLCPCPCResD OnConnectionResponse; // DiscoveringConnectionLifecycleCallback

                private readonly ANCC   m_aClient;  // AdvertisingConnectionLifecycleCallbackProxy
                private readonly JCCI   m_jClient;  // DiscoveringConnectionLifecycleCallback
                private          string m_endpoint; // AdvertisingConnectionLifecycleCallbackProxy
                private readonly AIML   m_listener; // DiscoveringConnectionLifecycleCallback

                private readonly bool m_isAdvertising; // true for AdvertisingConnectionLifecycleCallbackProxy, false for DiscoveringConnectionLifecycleCallback

                internal Proxy(ANCC client, JCLCPCPAD onResult, JCLCPCPCReqD onRequest) : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.ctor(INearbyConnectionClient, Action<AdvertisingResult>, Action<ConnectionRequest>)");
                    m_isAdvertising = true;
                    m_aClient        = client;
                    if (onResult  != null) OnAdvertisingResult += onResult;
                    if (onRequest != null) OnConnectionRequest += onRequest;
                }

                public Proxy(JCCI client, AIML listener, JCLCPCPCResD onResponse) : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.ctor(ConnectionsClient, IMessageListener, Action<ConnectionResponse>)");
                    m_isAdvertising = false;
                    m_jClient       = client;
                    m_listener      = listener;
                    if (onResponse != null) OnConnectionResponse += onResponse;
                }

                [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
                public void onConnectionInitiated(string id, JCII jInfo)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.onConnectionInitiated(string, ConnectionInfo)");
                    if (m_isAdvertising) {
                        m_endpoint = jInfo.GetEndpointName();
                        var request = new ACReq(id, m_endpoint, m_aClient.GetServiceId(), new byte[0]);
                        OnConnectionRequest?.Invoke(request);
                    } else {
                        var jProxy = JPCPC.MakeProxy(listener: m_listener);
                        using var jCallback = JPCP.MakeInstance(jProxy);
                        using var jTask = m_jClient.JAcceptConnection(id, jCallback);
                    }
                }

                [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
                public void onConnectionResult(string id, JCRI jResolution)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.onConnectionInitiated(string, ConnectionResolution)");
                    if (m_isAdvertising) {
                        var status = 0;
                        using (var jStatus = jResolution.JGetStatus()) {
                            status = jStatus.GetStatusCode();
                        }
                        AAR result;
                        if (status == JCSC.STATUS_OK) {
                            result = new AAR(ARS.Success, m_endpoint);
                        } else if (status == JCSC.STATUS_ALREADY_DISCOVERING) {
                            result = new AAR(ARS.NotAuthorized, m_endpoint);
                        } else {
                            result = new AAR(ARS.InternalError, m_endpoint);
                        }
                        OnAdvertisingResult?.Invoke(result);
                    } else {
                        var status = 0;
                        using (var jStatus = jResolution.JGetStatus()) {
                            status = jStatus.GetStatusCode();
                        }
                        ACRes response;
                        if (status == JCSC.STATUS_OK) {
                            response = ACRes.Accepted(NearbyClientId, id, new byte[0]);
                        } else if (status == JCSC.STATUS_ALREADY_DISCOVERING) {
                            response = ACRes.AlreadyConnected(NearbyClientId, id);
                        } else {
                            response = ACRes.Rejected(NearbyClientId, id);
                        }
                        OnConnectionResponse?.Invoke(response);
                    }
                }

                [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
                public void onDisconnected(string id)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.onDisconnected(string)");
                    if (m_isAdvertising) {
                        m_aClient.MessageListener?.OnRemoteEndpointDisconnected(id);
                    } else {
                        m_listener?.OnRemoteEndpointDisconnected(id);
                    }
                }

            }

        }

        public static JCLCPI MakeInstance(JCLCPCP proxy) => new(proxy);
        
        public static JCLCPI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            [Preserve]
            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(JCLCPCP proxy) : base(FullyQualifiedClassName, proxy)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}(Callback)");
            }

        }

    }

}

#endif