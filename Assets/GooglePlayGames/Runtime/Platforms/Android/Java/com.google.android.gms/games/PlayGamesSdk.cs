#if UNITY_ANDROID

using GooglePlayGames.OurUtils;

using JC = GooglePlayGames.JavaClass;

using JCI   = GooglePlayGames.Android.Java.Context.Instance;
using JPGSC = GooglePlayGames.Android.Java.PlayGamesSdk.Class;
using JUP   = GooglePlayGames.Android.Java.UnityPlayer;

namespace GooglePlayGames.Android.Java {

    internal static class PlayGamesSdk {

        public static readonly string ClassName               =  "PlayGamesSdk";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static void Initialize(            ) => JPGSC.Instance.Initialize(        );
        public static void Initialize(JCI jContext) => JPGSC.Instance.Initialize(jContext);

        internal sealed class Class : JC {

            private static JPGSC s_instance = null;

            public static JPGSC Instance => s_instance ??= new JPGSC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public void Initialize(JCI jContext)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.initialize(Context)");
                Misc.CheckNotNull(jContext, nameof(jContext));
                CallStatic("initialize", jContext);
            }

        }

    }

    internal static class PlayGamesSdkExtensions {

        public static void Initialize(this JPGSC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            self.Initialize(jActivity);
        }

    }

}

#endif