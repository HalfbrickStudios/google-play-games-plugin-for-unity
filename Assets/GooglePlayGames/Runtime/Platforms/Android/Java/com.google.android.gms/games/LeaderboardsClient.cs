#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JAD   = GooglePlayGames.Android.Java.AnnotatedData;
using JLI   = GooglePlayGames.Android.Java.Leaderboard.Instance;
using JLSBI = GooglePlayGames.Android.Java.LeaderboardScoreBuffer.Instance;
using JLSCI = GooglePlayGames.Android.Java.LeaderboardsClient.Instance;
using JLSsI = GooglePlayGames.Android.Java.LeaderboardsClient.LeaderboardScores.Instance;
using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JT    = GooglePlayGames.Android.Java.Task;

namespace GooglePlayGames.Android.Java {

    internal static class LeaderboardsClient {

        public static readonly string ClassName               =  "LeaderboardsClient";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        internal static class LeaderboardScores {

            public static readonly string ClassName               = $"{LeaderboardsClient.ClassName}$LeaderboardScores";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static JLSsI MakeInstance() => new();
            
            public static JLSsI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

            internal sealed class Instance : JOI {

                internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
                {
                    if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
                }

                internal Instance() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
                }

                public JLI JGetLeaderboard()
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.getLeaderboard()");
                    return Call<JLI>("getLeaderboard");
                }

                public JLSBI JGetScores()
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.getScores()");
                    return Call<JLSBI>("getScores");
                }

                public void Release()
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.release()");
                    Call("release");
                }

                #region IDisposable implementation

                private bool m_disposed = false;

                protected override void Dispose(bool disposing)
                {
                    if (!m_disposed) {
                        if (disposing) {
                            // No-op, no managed resources to dispose
                        }
                        Release();
                        m_disposed = true;
                    }
                    base.Dispose(disposing);
                }

                #endregion IDisposable implementation

            }

        }

        public static JLSCI MakeInstance() => new();
        
        public static JLSCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            public Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public JT.Instance<JAD.Instance<JLSsI>> JLoadMoreScores(JLSBI lsbo, int rows, int direction)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.loadMoreScores(LeaderboardScoreBuffer, int, int)");
                return Call<JT.Instance<JAD.Instance<JLSsI>>>("loadMoreScores", lsbo, rows, direction);
            }

            public JT.Instance<JAD.Instance<JLSsI>> JLoadPlayerCenteredScores(string id, int span, int collection, int rows)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.loadPlayerCenteredScores(string, TimeSpan, Collection, int)");
                return Call<JT.Instance<JAD.Instance<JLSsI>>>("loadPlayerCenteredScores", id, span, collection, rows);
            }

            public JT.Instance<JAD.Instance<JLSsI>> JLoadTopScores(string id, int span, int collection, int rows)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.loadTopScores(string, TimeSpan, Collection, int)");
                return Call<JT.Instance<JAD.Instance<JLSsI>>>("loadTopScores", id, span, collection, rows);
            }

            public void SubmitScore(string id, long score)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.submitScore(string, long)");
                Call("submitScore", id, score);
            }

            public void SubmitScore(string id, long score, string metadata)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.submitScore(string, long, string)");
                Call("submitScore", id, score, metadata);
            }

        }

    }

}

#endif