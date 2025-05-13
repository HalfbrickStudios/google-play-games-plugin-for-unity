#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JSI = GooglePlayGames.Android.Java.Status.Instance;
using JEI = GooglePlayGames.Android.Java.Exception.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Status {

        public static readonly string ClassName               =  "Status";
        public static readonly string PackageName             =  "com.google.android.gms.common.api";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JSI MakeInstance() => new();

        public static JSI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JEI {

            [Preserve]
            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public int GetStatusCode()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getStatusCode()");
                return Call<int>("getStatusCode");
            }

        }

    }

}

#endif