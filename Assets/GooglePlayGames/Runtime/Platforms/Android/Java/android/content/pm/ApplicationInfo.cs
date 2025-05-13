#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using Logger = GooglePlayGames.Utils.Logger;

using UAJO = UnityEngine.AndroidJavaObject;

using JBI  = GooglePlayGames.Android.Java.Bundle.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JPII = GooglePlayGames.Android.Java.ApplicationInfo.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class ApplicationInfo {

        public static readonly string ClassName               =  "ApplicationInfo";
        public static readonly string PackageName             =  "android.content.pm";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JPII MakeInstance() => new();

        public static JPII WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

            public JBI JMetaData
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.metaData");
                    return Get<JBI>("metaData");
                }
            }

        }

    }

}

#endif