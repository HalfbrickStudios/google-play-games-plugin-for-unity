#if UNITY_ANDROID

using System;
using System.Diagnostics.CodeAnalysis;

using GooglePlayGames.OurUtils;

using AJP = UnityEngine.AndroidJavaProxy;

using JEI   = GooglePlayGames.Android.Java.Exception.Instance;
using JOFLP = GooglePlayGames.Android.Java.OnFailureListener.Proxy;

namespace GooglePlayGames.Android.Java {

    internal static class OnFailureListener {

        public static readonly string ClassName               =  "OnFailureListener";
        public static readonly string PackageName             =  "com.google.android.gms.tasks";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JOFLP MakeProxy(JOFLP.OnFailureDelegate callback = null) => new(callback);

        internal sealed class Proxy : AJP {
            
            public delegate void OnFailureDelegate(JEI jException);

            public event OnFailureDelegate OnFailure;

            internal Proxy(OnFailureDelegate callback) : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<TResult>.ctor()");
                if (callback != null) OnFailure += callback;
            }

            [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
            internal void onFailure(JEI jException)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<TResult>.onFailure(Exception)");
                if (jException is IDisposable disposable) {
                    using (disposable) {
                        OnFailure?.Invoke(jException);
                    }
                } else {
                    OnFailure?.Invoke(jException);
                }
            }

        }

    }

}

#endif