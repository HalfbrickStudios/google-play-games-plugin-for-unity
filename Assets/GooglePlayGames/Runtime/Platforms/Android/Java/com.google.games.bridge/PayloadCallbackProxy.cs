#if UNITY_ANDROID

using System;
using System.Diagnostics.CodeAnalysis;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;
using UAJP = UnityEngine.AndroidJavaProxy;

using AIML = GooglePlayGames.Api.Nearby.IMessageListener;

using JOI     = GooglePlayGames.Android.Java.Object.Instance;
using JPCPCP  = GooglePlayGames.Android.Java.PayloadCallbackProxy.Callback.Proxy;
using JPCPCPD = GooglePlayGames.Android.Java.PayloadCallbackProxy.Callback.Proxy.OnPayloadReceivedsDelegate;
using JPCPI   = GooglePlayGames.Android.Java.PayloadCallbackProxy.Instance;
using JPI     = GooglePlayGames.Android.Java.Payload.Instance;
using JPT     = GooglePlayGames.Android.Java.Payload.Type;

namespace GooglePlayGames.Android.Java {

    internal static class PayloadCallbackProxy {

        public static readonly string ClassName               =  "PayloadCallbackProxy";
        public static readonly string PackageName             =  "com.google.games.bridge";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        internal static class Callback {

            public static readonly string ClassName               = $"{PayloadCallbackProxy.ClassName}$Callback";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static JPCPCP MakeProxy(AIML listener = null, JPCPCPD callback = null) => new(listener, callback);

            internal sealed class Proxy : UAJP {

                public delegate void OnPayloadReceivedsDelegate(string id, JPI jPayload);

                public event JPCPCPD OnPayloadReceiveds;

                private readonly AIML m_listener;

                internal Proxy(AIML listener, JPCPCPD callback) : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
                    if (callback != null) OnPayloadReceiveds += callback;
                    m_listener = listener;
                }

                [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
                public void onPayloadReceiveds(string id, JPI jPayload)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.onPayloadReceiveds(string, Payload)");
                    OnPayloadReceiveds?.Invoke(id, jPayload);
                    if (jPayload.JGetType() != JPT.BYTES) return;
                    m_listener?.OnMessageReceived(id, jPayload.AsBytes(), reliable: true);
                }

            }

        }

        public static JPCPI MakeInstance(JPCPCP proxy) => new(proxy);
        
        public static JPCPI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

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