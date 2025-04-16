#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JADB  = GooglePlayGames.Android.Java.AbstractDataBuffer;
using JLSBI = GooglePlayGames.Android.Java.LeaderboardScoreBuffer.Instance;
using JLSI  = GooglePlayGames.Android.Java.LeaderboardScore.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class LeaderboardScoreBuffer {

        public static readonly string ClassName               =  "LeaderboardScoreBuffer";
        public static readonly string PackageName             =  "com.google.android.gms.games.leaderboard";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JLSBI MakeInstance() => new();
        
        public static JLSBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JADB.Instance<JLSI> {

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