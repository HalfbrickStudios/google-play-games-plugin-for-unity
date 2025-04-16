#if UNITY_ANDROID

using GooglePlayGames.OurUtils;

using JC = GooglePlayGames.JavaClass;

using JCSC = GooglePlayGames.Android.Java.ConnectionsStatusCodes.Class;

namespace GooglePlayGames.Android.Java {

    internal static class ConnectionsStatusCodes {

        public static readonly string ClassName               =  "ConnectionsStatusCodes";
        public static readonly string PackageName             =  "com.google.android.gms.nearby.connection";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static int STATUS_ALREADY_DISCOVERING => JCSC.Instance.STATUS_ALREADY_DISCOVERING;
        public static int STATUS_OK                  => JCSC.Instance.STATUS_OK;

        internal sealed class Class : JC {

            private static JCSC s_instance = null;

            public static JCSC Instance => s_instance ??= new JCSC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public int STATUS_ALREADY_DISCOVERING
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.STATUS_ALREADY_DISCOVERING");
                    return GetStatic<int>("STATUS_ALREADY_DISCOVERING");
                }
            }

            public int STATUS_OK
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.STATUS_OK");
                    return GetStatic<int>("STATUS_OK");
                }
            }

        }

    }

}

#endif