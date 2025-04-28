#if UNITY_ANDROID

using System;
using System.Diagnostics.CodeAnalysis;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;
using UAJP = UnityEngine.AndroidJavaProxy;

using JO = GooglePlayGames.Android.Java.Object;
using JT = GooglePlayGames.Android.Java.Task;

namespace GooglePlayGames.Android.Java {

    internal static class OnCompleteListener {

        public static readonly string ClassName               =  "OnCompleteListener";
        public static readonly string PackageName             =  "com.google.android.gms.tasks";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static Proxy<TResult> MakeProxy<TResult>(Proxy<TResult>.OnCompleteDelegate callback = null) => new(callback);

        internal sealed class Proxy<TResult> : UAJP {
            
            public delegate void OnCompleteDelegate(JT.Instance<TResult> jTask);

            public event OnCompleteDelegate OnComplete;

            internal Proxy(OnCompleteDelegate callback = null) : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}<TResult>.ctor()");
                if (callback != null) OnComplete += callback;
            }

            [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
            internal void onComplete(UAJO jTask)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}<TResult>.onComplete(AndroidJavaObject)");
                using var jObject = JO.WrapInstance(jTask);
                using var jClass  = jObject.JGetClass();
                if (JT.JClass.IsAssignableFrom(jClass)) {
                    OnCompleteHandler(Jni.Wrap<JT.Instance<TResult>>(jTask));
                } else {
                    throw new InvalidOperationException($"Cannot cast {typeof(UAJO)} to {typeof(JT.Instance<TResult>)} (unreachable branch?)");
                }
            }

            [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
            internal void onComplete(JT.Instance<TResult> task)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}<TResult>.onComplete(Task.Instance<TResult>)");
                OnCompleteHandler(task);
            }

            internal void OnCompleteHandler(JT.Instance<TResult> task)
            {
                if (task is IDisposable disposable) {
                    using (disposable) {
                        OnComplete?.Invoke(task);
                    }
                } else {
                    OnComplete?.Invoke(task);
                }
            }

        }

    }

}

#endif