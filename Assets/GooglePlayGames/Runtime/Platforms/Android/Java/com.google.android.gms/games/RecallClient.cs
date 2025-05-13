#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JRAI = GooglePlayGames.Android.Java.RecallAccess.Instance;
using JRCI = GooglePlayGames.Android.Java.RecallClient.Instance;
using JT   = GooglePlayGames.Android.Java.Task;

namespace GooglePlayGames.Android.Java {

    internal static class RecallClient {

        public static readonly string ClassName               =  "RecallClient";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JRCI MakeInstance() => new();
        
        public static JRCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

            public JT.Instance<JRAI> JRequestRecallAccess()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.requestRecallAccess()");
                return Call<JT.Instance<JRAI>>("requestRecallAccess");
            }

        }

    }

}

#endif