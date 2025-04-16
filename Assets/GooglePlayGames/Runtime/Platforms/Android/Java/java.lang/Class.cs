#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JCI = GooglePlayGames.Android.Java.Class.Instance;
using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Class {

        public static readonly string ClassName               =  "Class";
        public static readonly string PackageName             =  "java.lang";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JCI MakeInstance() => new(FullyQualifiedClassName, inherit: false);

        public static JCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance : JOI {

            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            public string GetName()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getName()");
                return Call<string>("getName");
            }

        }

    }

}

#endif