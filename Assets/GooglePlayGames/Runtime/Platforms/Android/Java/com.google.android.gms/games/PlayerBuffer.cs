#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JADB = GooglePlayGames.Android.Java.AbstractDataBuffer;
using JAI  = GooglePlayGames.Android.Java.Player.Instance;
using JPBI = GooglePlayGames.Android.Java.PlayerBuffer.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class PlayerBuffer {

        public static readonly string ClassName               =  "PlayerBuffer";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JPBI MakeInstance() => new();
        
        public static JPBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JADB.Instance<JAI> {

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