#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JAD  = GooglePlayGames.Android.Java.AnnotatedData;
using JEBI = GooglePlayGames.Android.Java.EventBuffer.Instance;
using JECI = GooglePlayGames.Android.Java.EventsClient.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JT   = GooglePlayGames.Android.Java.Task;

namespace GooglePlayGames.Android.Java {

    internal static class EventsClient {

        public static readonly string ClassName               =  "EventsClient";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JECI MakeInstance() => new();
        
        public static JECI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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
                Logger.t($"JNI: Call {FullyQualifiedClassName}.increment(string, int)");
                Misc.CheckNotNull(id, nameof(id));
                Misc.CheckPositive(steps, nameof(steps));
                Call("increment", id, steps);
            }

            public JT.Instance<JAD.Instance<JEBI>> JLoad(bool reload)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.load(bool)");
                return Call<UAJO>("load", reload) as JT.Instance<JAD.Instance<JEBI>>;
            }

            public JT.Instance<JAD.Instance<JEBI>> JLoadByIds(bool reload, params string[] ids)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.loadByIds(bool, string[])");
                Misc.CheckNotNull(ids, nameof(ids));
                return Call<UAJO>("loadByIds", reload, ids) as JT.Instance<JAD.Instance<JEBI>>;
            }

        }

    }

}

#endif