#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JAII = GooglePlayGames.Android.Java.ApplicationInfo.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JPMI = GooglePlayGames.Android.Java.PackageManager.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class PackageManager {

        public static readonly string ClassName               =  "PackageManager";
        public static readonly string PackageName             =  "android.content.pm";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JPMI MakeInstance() => new();

        public static JPMI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: true);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public JAII JGetApplicationInfo(string package, int flags)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getApplicationInfo(string, int)");
                Misc.CheckNotNull(package, nameof(package));
                return Call<JAII>("getApplicationInfo", package, flags);
            }

        }

    }

}

#endif