#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JADB  = GooglePlayGames.Android.Java.AbstractDataBuffer;
using JSMI  = GooglePlayGames.Android.Java.SnapshotMetadata.Instance;
using JSMBI = GooglePlayGames.Android.Java.SnapshotMetadataBuffer.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class SnapshotMetadataBuffer {

        public static readonly string ClassName               =  "SnapshotMetadataBuffer";
        public static readonly string PackageName             =  "com.google.android.gms.games.snapshot";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JSMBI MakeInstance() => new();

        public static JSMBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JADB.Instance<JSMI> {

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