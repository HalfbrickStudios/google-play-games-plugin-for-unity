#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JARI = GooglePlayGames.Android.Java.AuthenticationResult.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class AuthenticationResult {

        public static readonly string ClassName               =  "AuthenticationResult";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JARI MakeInstance() => new();
        
        public static JARI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public bool IsAuthenticated()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.isAuthenticated()");
                return Call<bool>("isAuthenticated");
            }

        }

    }

}

#endif