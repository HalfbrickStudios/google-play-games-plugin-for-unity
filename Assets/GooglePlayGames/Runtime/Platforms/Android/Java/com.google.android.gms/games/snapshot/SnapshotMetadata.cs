#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JSMI = GooglePlayGames.Android.Java.SnapshotMetadata.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class SnapshotMetadata {

        public static readonly string ClassName               =  "SnapshotMetadata";
        public static readonly string PackageName             =  "com.google.android.gms.games.snapshot";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JSMI MakeInstance() => new();

        public static JSMI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

            public string GetCoverImageUrl()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getCoverImageUrl()");
                return Call<string>("getCoverImageUrl");
            }

            public string GetDescription()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getDescription()");
                return Call<string>("getDescription");
            }

            public JSMI JFreeze()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.freeze()");
                return Call<JSMI>("freeze");
            }

            public long GetLastModifiedTimestamp()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getLastModifiedTimestamp()");
                return Call<long>("getLastModifiedTimestamp");
            }

            public long GetPlayedTime()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getPlayedTime()");
                return Call<long>("getPlayedTime");
            }

            public string GetSnapshotId()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getSnapshotId()");
                return Call<string>("getSnapshotId");
            }

            public string GetUniqueName()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getUniqueName()");
                return Call<string>("getUniqueName");
            }

        }

    }

}

#endif