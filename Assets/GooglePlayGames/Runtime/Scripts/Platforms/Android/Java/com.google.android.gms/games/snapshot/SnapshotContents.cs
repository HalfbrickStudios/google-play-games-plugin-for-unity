#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JSCI = GooglePlayGames.Android.Java.SnapshotContents.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class SnapshotContents {

        public static readonly string ClassName               =  "SnapshotContents";
        public static readonly string PackageName             =  "com.google.android.gms.games.snapshot";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JSCI MakeInstance() => new();

        public static JSCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public bool IsClosed()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.isClosed()");
                return Call<bool>("isClosed");
            }

            public byte[] ReadFully()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.readFully()");
                return Call<byte[]>("readFully");
            }

            public bool WriteBytes(byte[] bytes)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.writeBytes(byte[])");
                Misc.CheckNotNull(bytes, nameof(bytes));
                return Call<bool>("writeBytes", bytes);
            }

        }

    }

}

#endif