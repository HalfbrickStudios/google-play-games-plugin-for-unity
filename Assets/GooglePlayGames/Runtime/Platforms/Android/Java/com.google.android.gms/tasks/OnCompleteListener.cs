#if UNITY_ANDROID

using System;
using System.Diagnostics.CodeAnalysis;

using GooglePlayGames.Utils;

using AJP = UnityEngine.AndroidJavaProxy;

using JT = GooglePlayGames.Android.Java.Task;

namespace GooglePlayGames.Android.Java {

    internal static class OnCompleteListener {

        public static readonly string ClassName               =  "OnCompleteListener";
        public static readonly string PackageName             =  "com.google.android.gms.tasks";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static Proxy<TResult> MakeProxy<TResult>(Proxy<TResult>.OnCompleteDelegate callback = null) => new(callback);

        internal sealed class Proxy<TResult> : AJP {
            
            public delegate void OnCompleteDelegate(JT.Instance<TResult> jTask);

            public event OnCompleteDelegate OnComplete;

            internal Proxy(OnCompleteDelegate callback = null) : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}<TResult>.ctor()");
                if (callback != null) OnComplete += callback;
            }

            [SuppressMessage("Style", "IDE1006", Justification = "Must match Java interface name")]
            internal void onComplete(JT.Instance<TResult> jTask)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}<TResult>.onComplete(Task<TResult>)");
                if (jTask is IDisposable disposable) {
                    using (disposable) {
                        OnComplete?.Invoke(jTask);
                    }
                } else {
                    OnComplete?.Invoke(jTask);
                }
            }

        }

    }

}

#endif