#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JRAEI = GooglePlayGames.Android.Java.ResolvableApiException.Instance;
using JOI   = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class ResolvableApiException {

        public static readonly string ClassName               =  "ResolvableApiException";
        public static readonly string PackageName             =  "com.google.android.gms.common.api";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JRAEI MakeInstance() => new();

        public static JRAEI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public JOI JGetResolution()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getResolution()");
                return Call<JOI>("getResolution");
            }

        }

    }

}

#endif