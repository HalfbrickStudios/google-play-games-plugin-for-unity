#if UNITY_ANDROID

using GooglePlayGames.OurUtils;

using JC = GooglePlayGames.JavaClass;

using JAI  = GooglePlayGames.Android.Java.Activity.Instance;
using JUPC = GooglePlayGames.Android.Java.UnityPlayer.Class;

namespace GooglePlayGames.Android.Java {

    internal static class UnityPlayer {

        public static readonly string ClassName               =  "UnityPlayer";
        public static readonly string PackageName             =  "com.unity3d.player";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JUPC MakeClass() => new();

        public static JAI JCurrentActivity => JUPC.Instance.JCurrentActivity;

        internal sealed class Class : JC {

            private static JUPC s_instance = null;

            public static JUPC Instance => s_instance ??= new JUPC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JAI JCurrentActivity
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.currentActivity");
                    return GetStatic<JAI>("currentActivity");
                }
            }

        }

    }

}

#endif