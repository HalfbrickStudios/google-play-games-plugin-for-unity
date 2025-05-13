#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JSMEI = GooglePlayGames.Android.Java.SnapshotMetadataEntity.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class SnapshotMetadataEntity {

        public static readonly string ClassName               = "SnapshotMetadataEntity";
        public static readonly string PackageName             = "com.google.android.gms.games.snapshot";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JSMEI MakeInstance() => new();

        public static JSMEI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

        }

    }

}

#endif