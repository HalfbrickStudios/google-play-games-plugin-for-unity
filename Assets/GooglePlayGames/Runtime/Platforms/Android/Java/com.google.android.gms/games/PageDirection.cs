#if UNITY_ANDROID

using GooglePlayGames.Utils;

using JC = GooglePlayGames.JavaClass;

using JPDC = GooglePlayGames.Android.Java.PageDirection.Class;

namespace GooglePlayGames.Android.Java {

    internal static class PageDirection {

        public static readonly string ClassName               =  "PageDirection";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static int NEXT => JPDC.Instance.NEXT;
        public static int NONE => JPDC.Instance.NONE;
        public static int PREV => JPDC.Instance.PREV;

        internal sealed class Class : JC {

            private static JPDC s_instance = null;

            public static JPDC Instance => s_instance ??= new JPDC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public int NEXT
            {
                get {
                    Logger.t($"JNI: Reading static field {FullyQualifiedClassName}.NEXT");
                    return GetStatic<int>("NEXT");
                }
            }

            public int NONE
            {
                get {
                    Logger.t($"JNI: Reading static field {FullyQualifiedClassName}.NONE");
                    return GetStatic<int>("NONE");
                }
            }

            public int PREV
            {
                get {
                    Logger.t($"JNI: Reading static field {FullyQualifiedClassName}.PREV");
                    return GetStatic<int>("PREV");
                }
            }

        }

    }

}

#endif