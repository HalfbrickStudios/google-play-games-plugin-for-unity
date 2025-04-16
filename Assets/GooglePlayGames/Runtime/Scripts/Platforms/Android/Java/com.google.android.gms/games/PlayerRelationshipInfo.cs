#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JPFS  = GooglePlayGames.Android.Java.Player.PlayerFriendStatus;
using JPRII = GooglePlayGames.Android.Java.PlayerRelationshipInfo.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class PlayerRelationshipInfo {

        public static readonly string ClassName               =  "PlayerRelationshipInfo";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JPRII MakeInstance() => new();
        
        public static JPRII WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public int JGetFriendStatus()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getFriendStatus()");
                return Call<int>("getFriendStatus");
            }

        }

    }

}

#endif