#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JAD   = GooglePlayGames.Android.Java.AnnotatedData;
using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JPSCI = GooglePlayGames.Android.Java.PlayerStatsClient.Instance;
using JPSI  = GooglePlayGames.Android.Java.PlayerStats.Instance;
using JT    = GooglePlayGames.Android.Java.Task;

namespace GooglePlayGames.Android.Java {

    internal static class PlayerStatsClient {

        public static readonly string ClassName               =  "PlayerStatsClient";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";
        
        public static JPSCI MakeInstance() => new();

        public static JPSCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public JT.Instance<JAD.Instance<JPSI>> JLoadPlayerStats(bool reload)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.loadPlayerStats(bool)");
                return Call<JT.Instance<JAD.Instance<JPSI>>>("loadPlayerStats", reload);
            }

        }

    }

}

#endif