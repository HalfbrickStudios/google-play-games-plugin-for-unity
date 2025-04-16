#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JRAI = GooglePlayGames.Android.Java.RecallAccess.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class RecallAccess {

        public static readonly string ClassName               =  "RecallAccess";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JRAI MakeInstance() => new();
        
        public static JRAI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public string GetSessionId()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getSessionId()");
                return Call<string>("getSessionId");
            }

        }

    }

}

#endif