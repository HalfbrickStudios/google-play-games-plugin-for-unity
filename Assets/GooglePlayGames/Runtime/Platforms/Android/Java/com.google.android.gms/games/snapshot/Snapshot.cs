#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JSCI = GooglePlayGames.Android.Java.SnapshotContents.Instance;
using JSI  = GooglePlayGames.Android.Java.Snapshot.Instance;
using JSMI = GooglePlayGames.Android.Java.SnapshotMetadata.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Snapshot {

        public static readonly string ClassName               =  "Snapshot";
        public static readonly string PackageName             =  "com.google.android.gms.games.snapshot";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JSI MakeInstance() => new(FullyQualifiedClassName, inherit: false);
        
        public static JSI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance : JOI {

            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            public JSI JFreeze()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.freeze()");
                return Call<UAJO>("freeze") as JSI;
            }

            public JSMI JGetMetadata()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getMetadata()");
                return Call<UAJO>("getMetadata") as JSMI;
            }

            public JSCI JGetSnapshotContents()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getSnapshotContents()");
                return Call<UAJO>("getSnapshotContents") as JSCI;
            }

        }

    }

}

#endif