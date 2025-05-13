#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JAL  = GooglePlayGames.Android.Java.ArrayList;
using JLI  = GooglePlayGames.Android.Java.Leaderboard.Instance;
using JLVI = GooglePlayGames.Android.Java.LeaderboardVariant.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Leaderboard {

        public static readonly string ClassName               =  "Leaderboard";
        public static readonly string PackageName             =  "com.google.android.gms.games.leaderboard";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JLI MakeInstance() => new();
        
        public static JLI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

            public string GetDisplayName()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getDisplayName()");
                return Call<string>("getDisplayName");
            }

            public JAL.Instance<JLVI> JGetVariants()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getVariants()");
                return Call<UAJO>("getVariants") as JAL.Instance<JLVI>;
            }

        }

    }

}

#endif