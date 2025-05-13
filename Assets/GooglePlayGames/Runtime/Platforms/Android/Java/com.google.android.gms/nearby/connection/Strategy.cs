#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JC = GooglePlayGames.Android.JavaClass;

using JOI = GooglePlayGames.Android.Java.Object.Instance;
using JSC = GooglePlayGames.Android.Java.Strategy.Class;
using JSI = GooglePlayGames.Android.Java.Strategy.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Strategy {

        public static readonly string ClassName               =  "Strategy";
        public static readonly string PackageName             =  "com.google.android.gms.nearby.connection";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JSC MakeClass() => new();

        public static JSI JP2P_CLUSTER => JSC.Instance.JP2P_CLUSTER;

        internal sealed class Class : JC {

            private static JSC s_instance = null;

            public static JSC Instance => s_instance ??= new JSC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JSI JP2P_CLUSTER
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.P2P_CLUSTER");
                    return GetStatic<UAJO>("P2P_CLUSTER") as JSI;
                }
            }

        }

        public static JSI MakeInstance() => new();
        
        public static JSI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            [Preserve]
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