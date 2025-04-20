#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JABI = GooglePlayGames.Android.Java.AchievementBuffer.Instance;
using JACI = GooglePlayGames.Android.Java.AchievementsClient.Instance;
using JAD  = GooglePlayGames.Android.Java.AnnotatedData;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JT   = GooglePlayGames.Android.Java.Task;

namespace GooglePlayGames.Android.Java {

    internal static class AchievementsClient {

        public static readonly string ClassName               =  "AchievementsClient";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JACI MakeInstance() => new();
        
        public static JACI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public void Increment(string id, int steps)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.increment(string, int)");
                Misc.CheckNotNull(id, nameof(id));
                Misc.CheckPositive(steps, nameof(steps));
                Call("increment", id, steps);
            }

            public JT.Instance<JAD.Instance<JABI>> JLoad(bool reload)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.reload(bool)");
                return Call<JT.Instance<JAD.Instance<JABI>>>("load", reload);
            }

            public void Reveal(string id)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.reveal(string)");
                Misc.CheckNotNull(id, nameof(id));
                Call("reveal", id);
            }

            public void SetSteps(string id, int steps)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.setSteps(string, int)");
                Misc.CheckNotNull(id, nameof(id));
                Misc.CheckPositive(steps, nameof(steps));
                Call("setSteps", id, steps);
            }

            public void Unlock(string id)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.unlock(string)");
                Misc.CheckNotNull(id, nameof(id));
                Call("unlock", id);
            }

        }

    }

}

#endif