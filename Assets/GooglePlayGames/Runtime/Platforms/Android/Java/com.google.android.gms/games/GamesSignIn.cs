#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JARI  = GooglePlayGames.Android.Java.AuthenticationResult.Instance;
using JGSII = GooglePlayGames.Android.Java.GamesSignIn.Instance;
using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JT    = GooglePlayGames.Android.Java.Task;

namespace GooglePlayGames.Android.Java {

    internal static class GamesSignIn {

        public static readonly string ClassName               =  "GamesSignIn";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JGSII MakeInstance() => new();
        
        public static JGSII WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

            public JT.Instance<JARI> JIsAuthenticated()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.isAuthenticated()");
                return Call<JT.Instance<JARI>>("isAuthenticated");
            }

            public JT.Instance<JARI> JSignIn()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.signIn()");
                return Call<JT.Instance<JARI>>("signIn");
            }

            public JT.Instance<string> JRequestServerSideAccess(string webId, bool reload)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.requestServerSideAccess(string, bool)");
                Misc.CheckNotNull(webId, nameof(webId));
                return Call<JT.Instance<string>>("requestServerSideAccess", webId, reload);
            }

        }

    }

}

#endif