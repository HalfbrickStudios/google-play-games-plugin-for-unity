#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JEI = GooglePlayGames.Android.Java.Exception.Instance;
using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Exception {

        public static readonly string ClassName               =  "Exception";
        public static readonly string PackageName             =  "java.lang";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JEI MakeInstance() => new(FullyQualifiedClassName, inherit: false);
        
        public static JEI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance : JOI {

            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

        }

    }

}

#endif