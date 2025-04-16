#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using AFLVS = GooglePlayGames.Api.FriendsListVisibilityStatus;

using JCPII = GooglePlayGames.Android.Java.CurrentPlayerInfo.Instance;
using JFLVS = GooglePlayGames.Android.Java.Player.FriendsListVisibilityStatus;
using JOI   = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class CurrentPlayerInfo {

        public static readonly string ClassName               =  "CurrentPlayerInfo";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";
        
        public static JCPII MakeInstance() => new();
        
        public static JCPII WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public int JGetFriendsListVisibilityStatus()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getFriendsListVisibilityStatus()");
                return Call<int>("getFriendsListVisibilityStatus");
            }

        }

    }

    internal static class CurrentPlayerInfoExtensions {

        public static AFLVS GetFriendsListVisibilityStatus(this JCPII self)
        {
            var status = self.JGetFriendsListVisibilityStatus();
            return Utility.ToAndroidFriendsListVisibilityStatus(status);
        }

    }

}

#endif