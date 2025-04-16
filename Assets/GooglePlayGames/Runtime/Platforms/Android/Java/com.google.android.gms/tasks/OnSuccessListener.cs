#if UNITY_ANDROID

using System;
using System.Diagnostics.CodeAnalysis;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;
using UAJP = UnityEngine.AndroidJavaProxy;

using JO = GooglePlayGames.Android.JavaObject;

namespace GooglePlayGames.Android.Java {

    internal static class OnSuccessListener {

        public static readonly string ClassName               =  "OnSuccessListener";
        public static readonly string PackageName             =  "com.google.android.gms.tasks";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static Proxy<TResult> MakeProxy<TResult>(bool dispose, Proxy<TResult>.OnSuccessDelegate callback = null) => new(dispose, callback);

        internal sealed class Proxy<TResult> : UAJP {
            
            public delegate void OnSuccessDelegate(TResult result);

            public bool m_dispose;

            public event OnSuccessDelegate OnSuccess;

            internal Proxy(bool dispose, OnSuccessDelegate callback = null) : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<TResult>.ctor()");
                m_dispose = dispose;
                if (callback != null) OnSuccess += callback;
            }

            [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
            internal void onSuccess(UAJO result)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<TResult>.onSuccess(AndroidJavaObject)");
                var tresult = typeof(TResult);
                var tuajo   = typeof(UAJO);
                if (Jni.IsStrictSubclass(tresult, typeof(JO))) {
                    OnSuccessHandler(Jni.Wrap<TResult>(result));
                } else if (Jni.IsStrictSubclass(tresult, tuajo)) {
                    throw new NotImplementedException("You are using a subclass of AndroidJavaObject that does not inherit from GooglePlayGames.JavaObject; this is a developer bug!");
                } else {
                    throw new InvalidOperationException($"Cannot cast {tuajo} to {tresult} (unreachable branch?)");
                }
            }

            [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
            internal void onSuccess(TResult result)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<TResult>.onSuccess(TResult)");
                OnSuccessHandler(result);
            }

            internal void OnSuccessHandler(TResult result)
            {
                if (m_dispose && result is IDisposable disposable) {
                    using (disposable) {
                        OnSuccess?.Invoke(result);
                    }
                } else {
                    OnSuccess?.Invoke(result);
                }
            }

        }

    }

}

#endif