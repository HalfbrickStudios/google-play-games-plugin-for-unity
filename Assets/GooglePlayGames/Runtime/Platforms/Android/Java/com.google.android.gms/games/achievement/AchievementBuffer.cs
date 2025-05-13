#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JABI = GooglePlayGames.Android.Java.AchievementBuffer.Instance;
using JADB = GooglePlayGames.Android.Java.AbstractDataBuffer;
using JAI  = GooglePlayGames.Android.Java.Achievement.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class AchievementBuffer {

        public static readonly string ClassName               =  "AchievementBuffer";
        public static readonly string PackageName             =  "com.google.android.gms.games.achievement";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JABI MakeInstance() => new();
        
        public static JABI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JADB.Instance<JAI> {

            [Preserve]
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