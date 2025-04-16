#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JBI  = GooglePlayGames.Android.Java.Bundle.Instance;
using JBBI = GooglePlayGames.Android.Java.BaseBundle.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Bundle {

        public static readonly string ClassName               =  "Bundle";
        public static readonly string PackageName             =  "android.os";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JBI MakeInstance() => new();
        
        public static JBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JBBI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

        }

    }

}

#endif