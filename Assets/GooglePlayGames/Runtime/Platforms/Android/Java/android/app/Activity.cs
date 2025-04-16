#if UNITY_ANDROID

using System;

using UAJO = UnityEngine.AndroidJavaObject;

using Logger = GooglePlayGames.Utils.Logger;

using JAI = GooglePlayGames.Android.Java.Activity.Instance;
using JCI = GooglePlayGames.Android.Java.Context.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Activity {

        public static readonly string ClassName               =  "Activity";
        public static readonly string PackageName             =  "android.app";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JAI MakeInstance() => new();

        public static JAI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JCI {

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