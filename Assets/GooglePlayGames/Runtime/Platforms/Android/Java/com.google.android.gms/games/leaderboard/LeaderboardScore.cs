#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JLSI = GooglePlayGames.Android.Java.LeaderboardScore.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JPI  = GooglePlayGames.Android.Java.Player.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class LeaderboardScore {

        public static readonly string ClassName               =  "LeaderboardScore";
        public static readonly string PackageName             =  "com.google.android.gms.games.leaderboard";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JLSI MakeInstance() => new();

        public static JLSI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public long GetRank()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getRank()");
                return Call<long>("getRank");
            }

            public long GetRawScore()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getRawScore()");
                return Call<long>("getRawScore");
            }

            public JPI JGetScoreHolder()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getScoreHolder()");
                return Call<JPI>("getScoreHolder");
            }

            public string GetScoreTag()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getScoreTag()");
                return Call<string>("getScoreTag");
            }

            public long GetTimestampMillis()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getTimestampMillis()");
                return Call<long>("getTimestampMillis");
            }

        }

    }

}

#endif