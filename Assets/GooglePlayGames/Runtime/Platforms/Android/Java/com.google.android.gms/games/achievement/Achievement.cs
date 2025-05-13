#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JC = GooglePlayGames.Android.JavaClass;

using JAI = GooglePlayGames.Android.Java.Achievement.Instance;
using JAC = GooglePlayGames.Android.Java.Achievement.Class;
using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Achievement {

        public static readonly string ClassName               = "Achievement";
        public static readonly string PackageName             = "com.google.android.gms.games.achievement";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static int STATE_HIDDEN   => JAC.Instance.STATE_HIDDEN;
        public static int STATE_REVEALED => JAC.Instance.STATE_REVEALED;
        public static int STATE_UNLOCKED => JAC.Instance.STATE_UNLOCKED;

        public static int TYPE_INCREMENTAL => JAC.Instance.TYPE_INCREMENTAL;
        public static int TYPE_STANDARD    => JAC.Instance.TYPE_STANDARD;

        internal sealed class Class : JC {

            private static JAC s_instance = null;

            public static JAC Instance => s_instance ??= new JAC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public int STATE_HIDDEN
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.STATE_HIDDEN");
                    return GetStatic<int>("STATE_HIDDEN");
                }
            }

            public int STATE_REVEALED
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.STATE_REVEALED");
                    return GetStatic<int>("STATE_REVEALED");
                }
            }

            public int STATE_UNLOCKED
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.STATE_UNLOCKED");
                    return GetStatic<int>("STATE_UNLOCKED");
                }
            }

            public int TYPE_INCREMENTAL
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.TYPE_INCREMENTAL");
                    return GetStatic<int>("TYPE_INCREMENTAL");
                }
            }

            public int TYPE_STANDARD
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.TYPE_STANDARD");
                    return GetStatic<int>("TYPE_STANDARD");
                }
            }

        }

        public static JAI MakeInstance() => new();

        public static JAI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

            public string GetAchievementId()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getAchievementId()");
                return Call<string>("getAchievementId");
            }

            public int GetCurrentSteps()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getCurrentSteps()");
                return Call<int>("getCurrentSteps");
            }

            public string GetDescription()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getDescription()");
                return Call<string>("getDescription");
            }

            public long GetLastUpdatedTimestamp()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getLastUpdatedTimestamp()");
                return Call<long>("getLastUpdatedTimestamp");
            }

            public string GetName()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getName()");
                return Call<string>("getName");
            }

            public string GetRevealedImageUrl()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getRevealedImageUrl()");
                return Call<string>("getRevealedImageUrl");
            }

            public int JGetState()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getState()");
                return Call<int>("getState");
            }

            public int GetTotalSteps()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getTotalSteps()");
                return Call<int>("getTotalSteps");
            }


            public int JGetType()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getType()");
                return Call<int>("getType");
            }

            public string GetUnlockedImageUrl()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getUnlockedImageUrl()");
                return Call<string>("getUnlockedImageUrl");
            }

            public ulong GetXpValue()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getXpValue()");
                return Call<ulong>("getXpValue");
            }

        }

    }

}

namespace GooglePlayGames.Android.Java.Extensions {

    internal static class AchievementExtensions {

        public static bool IsIncremental(this JAI self) => Utility.ToAndroidAchievementIsIncremental(self.JGetType());

        public static bool IsRevealed(this JAI self) => Utility.ToAndroidAchievementIsRevealed(self.JGetState());

        public static bool IsUnlocked(this JAI self) => Utility.ToAndroidAchievementIsUnlocked(self.JGetState());

    }

}

#endif