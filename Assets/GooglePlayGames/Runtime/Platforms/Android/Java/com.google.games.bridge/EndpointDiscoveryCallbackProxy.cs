#if UNITY_ANDROID

using System;
using System.Diagnostics.CodeAnalysis;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;
using UAJP = UnityEngine.AndroidJavaProxy;

using AED  = GooglePlayGames.Api.Nearby.EndpointDetails;
using AIDL = GooglePlayGames.Api.Nearby.IDiscoveryListener;

using JDEII     = GooglePlayGames.Android.Java.DiscoveredEndpointInfo.Instance;
using JEDCPCPFD = GooglePlayGames.Android.Java.EndpointDiscoveryCallbackProxy.Callback.Proxy.OnEndpointFoundDelegate;
using JEDCPCPLD = GooglePlayGames.Android.Java.EndpointDiscoveryCallbackProxy.Callback.Proxy.OnEndpointLostDelegate;
using JOI       = GooglePlayGames.Android.Java.Object.Instance;
using JPCPCP    = GooglePlayGames.Android.Java.EndpointDiscoveryCallbackProxy.Callback.Proxy;
using JPCPI     = GooglePlayGames.Android.Java.EndpointDiscoveryCallbackProxy.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class EndpointDiscoveryCallbackProxy {

        public static readonly string ClassName               =  "EndpointDiscoveryCallbackProxy";
        public static readonly string PackageName             =  "com.google.games.bridge";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        internal static class Callback {

            public static readonly string ClassName               = $"{PayloadCallbackProxy.ClassName}$Callback";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static JPCPCP MakeProxy(AIDL listener = null, JEDCPCPFD onFound = null, JEDCPCPLD onLost = null) => new(listener, onFound, onLost);

            internal sealed class Proxy : UAJP {

                public delegate void OnEndpointFoundDelegate(string id, JDEII jInfo);
                public delegate void OnEndpointLostDelegate (string id             );

                public event JEDCPCPFD OnEndpointFound;
                public event JEDCPCPLD OnEndpointLost;

                private readonly AIDL m_listener;

                internal Proxy(AIDL listener, JEDCPCPFD onFound, JEDCPCPLD onLost) : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.ctor(IDiscoveryListener, Action<string, DiscoveredEndpointInfo>, Action<string>)");
                    if (onFound != null) OnEndpointFound += onFound;
                    if (onLost  != null) OnEndpointLost  += onLost;
                    m_listener = listener;
                }

                private AED CreateEndPointDetails(string id, JDEII info) => new(id, info.GetEndpointName(), info.GetServiceId());

                [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
                public void onEndpointFound(string id, JDEII jInfo)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.onEndpointFound(string, DiscoveredEndpointInfo)");
                    OnEndpointFound?.Invoke(id, jInfo);
                    m_listener?.OnEndpointFound(CreateEndPointDetails(id, jInfo));
                }

                [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
                public void onEndpointLost(string id)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.onEndpointLost(string)");
                    OnEndpointLost?.Invoke(id);
                    m_listener?.OnEndpointLost(id);
                }

            }

        }

        public static JPCPI MakeInstance(JPCPCP proxy) => new(proxy);
        
        public static JPCPI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            [Preserve]
            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(JPCPCP proxy) : base(FullyQualifiedClassName, proxy)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}(Callback)");
            }

        }

    }

}

#endif