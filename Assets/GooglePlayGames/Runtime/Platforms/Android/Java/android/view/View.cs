#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI = GooglePlayGames.Android.Java.Object.Instance;
using JVI = GooglePlayGames.Android.Java.View.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class View {

        public static readonly string ClassName               =  "View";
        public static readonly string PackageName             =  "android.view";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JVI MakeInstance() => new();
        
        public static JVI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

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