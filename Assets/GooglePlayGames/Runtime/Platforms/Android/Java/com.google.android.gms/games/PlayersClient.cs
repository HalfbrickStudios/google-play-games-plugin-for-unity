#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JAD  = GooglePlayGames.Android.Java.AnnotatedData;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JPBI = GooglePlayGames.Android.Java.PlayerBuffer.Instance;
using JPCI = GooglePlayGames.Android.Java.PlayersClient.Instance;
using JPI  = GooglePlayGames.Android.Java.Player.Instance;
using JT   = GooglePlayGames.Android.Java.Task;

namespace GooglePlayGames.Android.Java {

    internal static class PlayersClient {

        public static readonly string ClassName               =  "PlayersClient";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JPCI MakeInstance() => new();
        
        public static JPCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public JT.Instance<JAD.Instance<JPI>> JGetCurrentPlayer()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getCurrentPlayer()");
                return Call<JT.Instance<JAD.Instance<JPI>>>("getCurrentPlayer");
            }

            public JT.Instance<JAD.Instance<JPI>> JGetCurrentPlayer(bool reload)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getCurrentPlayer(bool)");
                return Call<JT.Instance<JAD.Instance<JPI>>>("getCurrentPlayer", reload);
            }

            public JT.Instance<JAD.Instance<JPBI>> JLoadFriends(int size, bool reload)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.loadFriends(int, bool)");
                return Call<JT.Instance<JAD.Instance<JPBI>>>("loadFriends", size, reload);
            }

            public JT.Instance<JAD.Instance<JPBI>> JLoadMoreFriends(int size)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.loadMoreFriends(int)");
                return Call<JT.Instance<JAD.Instance<JPBI>>>("loadMoreFriends", size);
            }

            public JT.Instance<JAD.Instance<JPI>> JLoadPlayer(string id)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.loadPlayer(string)");
                return Call<JT.Instance<JAD.Instance<JPI>>>("loadPlayer", id);
            }

        }

    }

}

#endif