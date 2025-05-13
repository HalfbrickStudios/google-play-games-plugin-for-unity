#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JCI  = GooglePlayGames.Android.Java.Context.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JPMI = GooglePlayGames.Android.Java.PackageManager.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Context {

        public static readonly string ClassName               =  "Context";
        public static readonly string PackageName             =  "android.content";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JCI MakeInstance() => new(FullyQualifiedClassName, inherit: false);

        public static JCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance : JOI {

            [Preserve]
            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            public string GetPackageName()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getPackageName()");
                return Call<string>("getPackageName");
            }

            public JPMI JGetPackageManager()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getPackageManager()");
                return Call<JPMI>("getPackageManager");
            }

        }

    }

}

#endif