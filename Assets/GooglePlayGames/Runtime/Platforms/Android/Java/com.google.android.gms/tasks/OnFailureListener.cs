#if UNITY_ANDROID

using System;
using System.Diagnostics.CodeAnalysis;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;
using UAJP = UnityEngine.AndroidJavaProxy;

using JE    = GooglePlayGames.Android.Java.Exception;
using JEI   = GooglePlayGames.Android.Java.Exception.Instance;
using JO    = GooglePlayGames.Android.Java.Object;
using JOFLP = GooglePlayGames.Android.Java.OnFailureListener.Proxy;

namespace GooglePlayGames.Android.Java {

    internal static class OnFailureListener {

        public static readonly string ClassName               =  "OnFailureListener";
        public static readonly string PackageName             =  "com.google.android.gms.tasks";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JOFLP MakeProxy(JOFLP.OnFailureDelegate callback = null) => new(callback);

        internal sealed class Proxy : UAJP {
            
            public delegate void OnFailureDelegate(JEI jException);

            public event OnFailureDelegate OnFailure;

            internal Proxy(OnFailureDelegate callback) : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}<TResult>.ctor()");
                if (callback != null) OnFailure += callback;
            }

            [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
            internal void onFailure(UAJO jException)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}<TResult>.onFailure(AndroidJavaObject)");
                using var jObject = JO.WrapInstance(jException);
                using var jClass  = jObject.JGetClass();
                if (JE.JClass.IsAssignableFrom(jClass)) {
                    OnFailure(Jni.Wrap<JEI>(jException));
                } else {
                    throw new InvalidOperationException($"Cannot cast {typeof(UAJO)} to {typeof(JEI)} (unreachable branch?)");
                }
            }

            [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
            internal void onFailure(JEI jException)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}<TResult>.onFailure(Exception.Instance)");
                OnFailureHandler(jException);
            }

            internal void OnFailureHandler(JEI jException)
            {
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