#if UNITY_ANDROID

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JC = GooglePlayGames.JavaClass;

using JAI  = GooglePlayGames.Android.Java.Activity.Instance;
using JCCI = GooglePlayGames.Android.Java.ConnectionsClient.Instance;
using JCI  = GooglePlayGames.Android.Java.Context.Instance;
using JNC  = GooglePlayGames.Android.Java.Nearby.Class;

namespace GooglePlayGames.Android.Java {

    internal static class Nearby {

        public static readonly string ClassName               =  "Nearby";
        public static readonly string PackageName             =  "com.google.android.gms.nearby";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JNC MakeClass() => new();

        public static JCCI JGetConnectionsClient(             ) => JNC.Instance.JGetConnectionsClient(         );
        public static JCCI JGetConnectionsClient(JCI jContext ) => JNC.Instance.JGetConnectionsClient(jContext );
        public static JCCI JGetConnectionsClient(JAI jActivity) => JNC.Instance.JGetConnectionsClient(jActivity);

        internal sealed class Class : JC {

            private static JNC s_instance = null;

            public static JNC Instance => s_instance ??= new JNC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JCCI JGetConnectionsClient(JCI jContext)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getConnectionsClient(Context)");
                Misc.CheckNotNull(jContext, nameof(jContext));
                return CallStatic<UAJO>("getConnectionsClient", jContext) as JCCI;
            }

            public JCCI JGetConnectionsClient(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getConnectionsClient(Activity)");
                Misc.CheckNotNull(jActivity, nameof(jActivity));
                return CallStatic<UAJO>("getConnectionsClient", jActivity) as JCCI;
            }

        }

    }

    internal static class NearbyExtensions {

        public static JCCI JGetConnectionsClient(this JNC self)
        {
            using var jActivity = UnityPlayer.JCurrentActivity;
            return self.JGetConnectionsClient(jActivity);
        }

    }

}

#endif