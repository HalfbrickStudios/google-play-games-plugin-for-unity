#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JBI    = GooglePlayGames.Android.Java.Bitmap.Instance;
using JOI    = GooglePlayGames.Android.Java.Object.Instance;
using JSMCB  = GooglePlayGames.Android.Java.SnapshotMetadataChange.Builder;
using JSMCBI = GooglePlayGames.Android.Java.SnapshotMetadataChange.Builder.Instance;
using JSMCI  = GooglePlayGames.Android.Java.SnapshotMetadataChange.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class SnapshotMetadataChange {

        public static readonly string ClassName               =  "SnapshotMetadataChange";
        public static readonly string PackageName             =  "com.google.android.gms.games.snapshot";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JSMCBI MakeBuilder() => JSMCB.MakeInstance();

        public static class Builder {

            public static readonly string ClassName               = $"{SnapshotMetadataChange.ClassName}$Builder";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static JSMCBI MakeInstance() => new();

            public static JSMCBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

            internal sealed class Instance : JOI {

                internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
                {
                    if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
                }

                internal Instance() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
                }

                public JSMCI JBuild()
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.build()");
                    return Call<JSMCI>("build");
                }

                public JSMCBI JSetCoverImage(JBI jBitmap)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.setCoverImage(Bitmap)");
                    Misc.CheckNotNull(jBitmap, nameof(jBitmap));
                    return Call<JSMCBI>("setCoverImage", jBitmap);
                }

                public JSMCBI JSetDescription(string description)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.setDescription(string)");
                    Misc.CheckNotNull(description, nameof(description));
                    return Call<JSMCBI>("setDescription", description);
                }

                public JSMCBI JSetPlayedTimeMillis(long time)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.setPlayedTimeMillis(long)");
                    return Call<JSMCBI>("setPlayedTimeMillis", time);
                }

            }

        }

        public static JSMCI MakeInstance() => new();

        public static JSMCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            public Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

        }

    }

}

#endif