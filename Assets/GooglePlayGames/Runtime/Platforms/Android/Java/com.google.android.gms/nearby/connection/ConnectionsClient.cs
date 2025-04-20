#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JAOI    = GooglePlayGames.Android.Java.AdvertisingOptions.Instance;
using JCCI    = GooglePlayGames.Android.Java.ConnectionsClient.Instance;
using JCLCPI  = GooglePlayGames.Android.Java.ConnectionLifecycleCallbackProxy.Instance;
using JDOI    = GooglePlayGames.Android.Java.DiscoveryOptions.Instance;
using JEDCPCI = GooglePlayGames.Android.Java.EndpointDiscoveryCallbackProxy.Instance;
using JOI     = GooglePlayGames.Android.Java.Object.Instance;
using JPCPI   = GooglePlayGames.Android.Java.PayloadCallbackProxy.Instance;
using JPI     = GooglePlayGames.Android.Java.Payload.Instance;
using JTI     = GooglePlayGames.Android.Java.Task.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class ConnectionsClient {

        public static readonly string ClassName               =  "ConnectionsClient";
        public static readonly string PackageName             =  "com.google.android.gms.nearby.connection";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JCCI MakeInstance() => new();
        
        public static JCCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public void DisconnectFromEndpoint(string id)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.disconnectFromEndpoint(string)");
                Call("disconnectFromEndpoint", id);
            }

            public JTI JAcceptConnection(string id, JPCPI callback)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.acceptConnection(string, PayloadCallbackProxy)");
                Misc.CheckNotNull(id, nameof(id));
                Misc.CheckNotNull(callback, nameof(callback));
                return Call<JTI>("acceptConnection", id, callback);
            }

            public JTI JRejectConnection(string id)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.rejectConnection(string)");
                Misc.CheckNotNull(id, nameof(id));
                return Call<JTI>("rejectConnection", id);
            }

            public JTI JRequestConnection(string name, string id, JCLCPI jCallback)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.requestConnection(string, string, ConnectionLifecycleCallbackProxy)");
                Misc.CheckNotNull(name, nameof(name));
                Misc.CheckNotNull(id, nameof(id));
                Misc.CheckNotNull(jCallback, nameof(jCallback));
                return Call<JTI>("requestConnection", name, id, jCallback);
            }

            public JTI JSendPayload(JOI jIds, JPI jPayload)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.sendPayload(Object, Payload)");
                Misc.CheckNotNull(jIds, nameof(jIds));
                Misc.CheckNotNull(jPayload, nameof(jPayload));
                return Call<JTI>("sendPayload", jIds, jPayload);
            }

            public JTI JStartAdvertising(string name, string service, JCLCPI jCallback, JAOI jOptions)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.startAdvertising(string, string, ConnectionLifecycleCallbackProxy, AdvertisingOptions)");
                Misc.CheckNotNull(name, nameof(name));
                Misc.CheckNotNull(service, nameof(service));
                Misc.CheckNotNull(jCallback, nameof(jCallback));
                Misc.CheckNotNull(jOptions, nameof(jOptions));
                return Call<JTI>("startAdvertising", name, service, jCallback, jOptions);
            }

            public JTI JStartDiscovery(string service, JEDCPCI jCallback, JDOI jOptions)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.startDiscovery(string, EndpointDiscoveryCallbackProxy, DiscoveryOptions)");
                Misc.CheckNotNull(service, nameof(service));
                Misc.CheckNotNull(jCallback, nameof(jCallback));
                Misc.CheckNotNull(jOptions, nameof(jOptions));
                return Call<JTI>("startDiscovery", service, jCallback, jOptions);
            }

            public void StopAdvertising()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.stopAdvertising()");
                Call("stopAdvertising");
            }

            public void StopAllEndpoints()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.stopAllEndpoints()");
                Call("stopAllEndpoints");
            }

            public void StopDiscovery()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.stopDiscovery()");
                Call("stopDiscovery");
            }

        }

    }

}

#endif