#if UNITY_ANDROID

using GooglePlayGames.Utils;

using JC = GooglePlayGames.JavaClass;

using JAI   = GooglePlayGames.Android.Java.Activity.Instance;
using JACI  = GooglePlayGames.Android.Java.AchievementsClient.Instance;
using JECI  = GooglePlayGames.Android.Java.EventsClient.Instance;
using JGSII = GooglePlayGames.Android.Java.GamesSignIn.Instance;
using JLCI  = GooglePlayGames.Android.Java.LeaderboardsClient.Instance;
using JPGC  = GooglePlayGames.Android.Java.PlayGames.Class;
using JPCI  = GooglePlayGames.Android.Java.PlayersClient.Instance;
using JPSCI = GooglePlayGames.Android.Java.PlayerStatsClient.Instance;
using JRCI  = GooglePlayGames.Android.Java.RecallClient.Instance;
using JSCI  = GooglePlayGames.Android.Java.SnapshotsClient.Instance;
using JUP   = GooglePlayGames.Android.Java.UnityPlayer;

namespace GooglePlayGames.Android.Java {

    internal static class PlayGames {

        public static readonly string ClassName               =  "PlayGames";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JPGC MakeClass() => new();

        public static JACI  JGetAchievementsClient(             ) => JPGC.Instance.JGetAchievementsClient(         );
        public static JACI  JGetAchievementsClient(JAI jActivity) => JPGC.Instance.JGetAchievementsClient(jActivity);
        public static JECI  JGetEventsClient      (             ) => JPGC.Instance.JGetEventsClient      (         );
        public static JECI  JGetEventsClient      (JAI jActivity) => JPGC.Instance.JGetEventsClient      (jActivity);
        public static JGSII JGetGamesSignInClient (             ) => JPGC.Instance.JGetGamesSignInClient (         );
        public static JGSII JGetGamesSignInClient (JAI jActivity) => JPGC.Instance.JGetGamesSignInClient (jActivity);
        public static JLCI  JGetLeaderboardsClient(             ) => JPGC.Instance.JGetLeaderboardsClient(         );
        public static JLCI  JGetLeaderboardsClient(JAI jActivity) => JPGC.Instance.JGetLeaderboardsClient(jActivity);
        public static JPCI  JGetPlayersClient     (             ) => JPGC.Instance.JGetPlayersClient     (         );
        public static JPCI  JGetPlayersClient     (JAI jActivity) => JPGC.Instance.JGetPlayersClient     (jActivity);
        public static JPSCI JGetPlayerStatsClient (             ) => JPGC.Instance.JGetPlayerStatsClient (         );
        public static JPSCI JGetPlayerStatsClient (JAI jActivity) => JPGC.Instance.JGetPlayerStatsClient (jActivity);
        public static JRCI  JGetRecallClient      (             ) => JPGC.Instance.JGetRecallClient      (         );
        public static JRCI  JGetRecallClient      (JAI jActivity) => JPGC.Instance.JGetRecallClient      (jActivity);
        public static JSCI  JGetSnapshotsClient   (             ) => JPGC.Instance.JGetSnapshotsClient   (         );
        public static JSCI  JGetSnapshotsClient   (JAI jActivity) => JPGC.Instance.JGetSnapshotsClient   (jActivity);

        internal sealed class Class : JC {

            private static JPGC s_instance = null;

            public static JPGC Instance => s_instance ??= new JPGC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JACI JGetAchievementsClient(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getAchievementsClient(Activity)");
                return CallStatic<JACI>("getAchievementsClient", jActivity);
            }

            public JECI JGetEventsClient(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getEventsClient(Activity)");
                return CallStatic<JECI>("getEventsClient", jActivity);
            }

            public JGSII JGetGamesSignInClient(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getGamesSignInClient(Activity)");
                return CallStatic<JGSII>("getGamesSignInClient", jActivity);
            }

            public JLCI JGetLeaderboardsClient(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getLeaderboardsClient(Activity)");
                return CallStatic<JLCI>("getLeaderboardsClient", jActivity);
            }

            public JPCI JGetPlayersClient(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getPlayersClient(Activity)");
                return CallStatic<JPCI>("getPlayersClient", jActivity);
            }

            public JPSCI JGetPlayerStatsClient(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getPlayerStatsClient(Activity)");
                return CallStatic<JPSCI>("getPlayerStatsClient", jActivity);
            }

            public JRCI JGetRecallClient(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getRecallClient(Activity)");
                return CallStatic<JRCI>("getRecallClient", jActivity);
            }

            public JSCI JGetSnapshotsClient(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getSnapshotsClient(Activity)");
                return CallStatic<JSCI>("getSnapshotsClient", jActivity);
            }

        }

    }

    internal static class PlayGamesExtensions {

        public static JACI JGetAchievementsClient(this JPGC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JGetAchievementsClient(jActivity);
        }

        public static JECI JGetEventsClient(this JPGC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JGetEventsClient(jActivity);
        }

        public static JGSII JGetGamesSignInClient(this JPGC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JGetGamesSignInClient(jActivity);
        }

        public static JLCI JGetLeaderboardsClient(this JPGC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JGetLeaderboardsClient(jActivity);
        }

        public static JPCI JGetPlayersClient(this JPGC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JGetPlayersClient(jActivity);
        }

        public static JPSCI JGetPlayerStatsClient(this JPGC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JGetPlayerStatsClient(jActivity);
        }

        public static JRCI JGetRecallClient(this JPGC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JGetRecallClient(jActivity);
        }

        public static JSCI JGetSnapshotsClient(this JPGC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JGetSnapshotsClient(jActivity);
        }

    }

}

#endif