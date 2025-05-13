#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JC = GooglePlayGames.Android.JavaClass;

using ALC  = GooglePlayGames.Api.LeaderboardCollection;
using ALTS = GooglePlayGames.Api.LeaderboardTimeSpan;

using JLVC = GooglePlayGames.Android.Java.LeaderboardVariant.Class;
using JLVI = GooglePlayGames.Android.Java.LeaderboardVariant.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class LeaderboardVariant {

        public static readonly string ClassName               =  "LeaderboardVariant";
        public static readonly string PackageName             =  "com.google.android.gms.games.leaderboard";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static int COLLECTION_FRIENDS => JLVC.Instance.COLLECTION_FRIENDS;
        public static int COLLECTION_PUBLIC  => JLVC.Instance.COLLECTION_PUBLIC;

        public static int TIME_SPAN_ALL_TIME => JLVC.Instance.TIME_SPAN_ALL_TIME;
        public static int TIME_SPAN_DAILY    => JLVC.Instance.TIME_SPAN_DAILY;
        public static int TIME_SPAN_WEEKLY   => JLVC.Instance.TIME_SPAN_WEEKLY;

        internal sealed class Class : JC {

            private static JLVC s_instance = null;

            public static JLVC Instance => s_instance ??= new JLVC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public int COLLECTION_FRIENDS
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.COLLECTION_FRIENDS");
                    return GetStatic<int>("COLLECTION_FRIENDS");
                }
            }

            public int COLLECTION_PUBLIC
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.COLLECTION_PUBLIC");
                    return GetStatic<int>("COLLECTION_PUBLIC");
                }
            }

            public int TIME_SPAN_ALL_TIME
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.TIME_SPAN_ALL_TIME");
                    return GetStatic<int>("TIME_SPAN_ALL_TIME");
                }
            }

            public int TIME_SPAN_DAILY
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.TIME_SPAN_DAILY");
                    return GetStatic<int>("TIME_SPAN_DAILY");
                }
            }

            public int TIME_SPAN_WEEKLY
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.TIME_SPAN_WEEKLY");
                    return GetStatic<int>("TIME_SPAN_WEEKLY");
                }
            }

        }

        public static JLVI MakeInstance() => new();
        
        public static JLVI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

            public int JGetCollection()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getCollection()");
                return Call<int>("getCollection");
            }

            public long GetNumScores()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getNumScores()");
                return Call<long>("getNumScores");
            }

            public long GetPlayerRank()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getPlayerRank()");
                return Call<long>("getPlayerRank");
            }

            public string GetPlayerScoreTag()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getPlayerScoreTag()");
                return Call<string>("getPlayerScoreTag");
            }

            public long GetRawPlayerScore()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getRawPlayerScore()");
                return Call<long>("getRawPlayerScore");
            }

            public int JGetTimeSpan()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getTimeSpan()");
                return Call<int>("getTimeSpan");
            }

            public bool HasPlayerInfo()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.hasPlayerInfo()");
                return Call<bool>("hasPlayerInfo");
            }

        }

    }

}

namespace GooglePlayGames.Android.Java.Extensions {

    internal static class LeaderboardVariantExtensions {

        public static ALC GetCollection(this JLVI self) => Utility.ToAndroidLeaderboardCollection(self.JGetCollection());

        public static ALTS GetTimeSpan(this JLVI self) => Utility.ToAndroidLeaderboardTimeSpan(self.JGetTimeSpan());

    }

}

#endif