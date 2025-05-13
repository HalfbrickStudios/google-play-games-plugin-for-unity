#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JC = GooglePlayGames.Android.JavaClass;

using JCPII  = GooglePlayGames.Android.Java.CurrentPlayerInfo.Instance;
using JFLVSC = GooglePlayGames.Android.Java.Player.FriendsListVisibilityStatus.Class;
using JOI    = GooglePlayGames.Android.Java.Object.Instance;
using JP     = GooglePlayGames.Android.Java.Player;
using JPFSC  = GooglePlayGames.Android.Java.Player.PlayerFriendStatus.Class;
using JPI    = GooglePlayGames.Android.Java.Player.Instance;
using JPRII  = GooglePlayGames.Android.Java.PlayerRelationshipInfo.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Player {

        public static readonly string ClassName               =  "Player";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        internal static class PlayerFriendStatus {

            public static readonly string ClassName               = $"{JP.ClassName}$PlayerFriendStatus";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static int FRIEND          => JPFSC.Instance.FRIEND;
            public static int NO_RELATIONSHIP => JPFSC.Instance.NO_RELATIONSHIP;
            public static int UNKNOWN         => JPFSC.Instance.UNKNOWN;

            internal sealed class Class : JC {

                private static JPFSC s_instance = null;

                public static JPFSC Instance => s_instance ??= new JPFSC();

                internal Class() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
                }

                public int FRIEND
                {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.FRIEND");
                        return GetStatic<int>("FRIEND");
                    }
                }

                public int NO_RELATIONSHIP
                {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.NO_RELATIONSHIP");
                        return GetStatic<int>("NO_RELATIONSHIP");
                    }
                }

                public int UNKNOWN
                {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.UNKNOWN");
                        return GetStatic<int>("UNKNOWN");
                    }
                }

            }

        }

        internal static class FriendsListVisibilityStatus {

            public static readonly string ClassName               = $"{JP.ClassName}FriendsListVisibilityStatus";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static int FEATURE_UNAVAILABLE => JFLVSC.Instance.FEATURE_UNAVAILABLE;
            public static int REQUEST_REQUIRED    => JFLVSC.Instance.REQUEST_REQUIRED;
            public static int UNKNOWN             => JFLVSC.Instance.UNKNOWN;
            public static int VISIBLE             => JFLVSC.Instance.VISIBLE;

            internal sealed class Class : JC {

                private static JFLVSC s_instance = null;

                public static JFLVSC Instance => s_instance ??= new JFLVSC();

                internal Class() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
                }

                public int FEATURE_UNAVAILABLE
                {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.FEATURE_UNAVAILABLE");
                        return GetStatic<int>("FEATURE_UNAVAILABLE");
                    }
                }

                public int REQUEST_REQUIRED
                {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.REQUEST_REQUIRED");
                        return GetStatic<int>("REQUEST_REQUIRED");
                    }
                }

                public int UNKNOWN
                {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.UNKNOWN");
                        return GetStatic<int>("UNKNOWN");
                    }
                }

                public int VISIBLE
                {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.VISIBLE");
                        return GetStatic<int>("VISIBLE");
                    }
                }

            }

        }

        public static JPI MakeInstance() => new();
        
        public static JPI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

            public JCPII JGetCurrentPlayerInfo()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getCurrentPlayerInfo()");
                return Call<JCPII>("getCurrentPlayerInfo");
            }

            public string GetDisplayName()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getDisplayName()");
                return Call<string>("getDisplayName");
            }

            public string GetIconImageUrl()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getIconImageUrl()");
                return Call<string>("getIconImageUrl");
            }

            public JPRII JGetRelationshipInfo()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getRelationshipInfo()");
                return Call<JPRII>("getRelationshipInfo");
            }

            public string GetPlayerId()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getPlayerId()");
                return Call<string>("getPlayerId");
            }

        }

    }

}

#endif