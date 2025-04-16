#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JCRI = GooglePlayGames.Android.Java.ConnectionResolution.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JSI  = GooglePlayGames.Android.Java.Status.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class ConnectionResolution {

        public static readonly string ClassName               =  "ConnectionResolution";
        public static readonly string PackageName             =  "com.google.android.gms.nearby.connection";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JCRI MakeInstance() => new();
        
        public static JCRI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public JSI JGetStatus()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getStatus()");
                return Call<JSI>("getStatus");
            }

        }

    }

}

#endif