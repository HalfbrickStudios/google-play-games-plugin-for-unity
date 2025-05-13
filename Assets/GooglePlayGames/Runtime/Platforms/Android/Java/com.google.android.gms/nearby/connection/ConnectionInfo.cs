#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JCII = GooglePlayGames.Android.Java.ConnectionInfo.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class ConnectionInfo {

        public static readonly string ClassName               =  "ConnectionInfo";
        public static readonly string PackageName             =  "com.google.android.gms.nearby.connection";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JCII MakeInstance() => new();
        
        public static JCII WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            [Preserve]
            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public string GetEndpointName()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getEndpointName()");
                return Call<string>("getEndpointName");
            }

        }

    }

}

#endif