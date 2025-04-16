#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JSI  = GooglePlayGames.Android.Java.Snapshot.Instance;
using JSEI = GooglePlayGames.Android.Java.SnapshotEntity.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class SnapshotEntity {

        public static readonly string ClassName               =  "SnapshotEntity";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JSEI MakeInstance() => new();

        public static JSEI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JSI {

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