#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JBI = GooglePlayGames.Android.Java.Bitmap.Instance;
using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Bitmap {

        public static readonly string ClassName               =  "Bitmap";
        public static readonly string PackageName             =  "android.graphics";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JBI MakeInstance() => new();

        public static JBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

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