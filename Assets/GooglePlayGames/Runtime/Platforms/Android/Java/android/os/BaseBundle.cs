#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JBBI = GooglePlayGames.Android.Java.BaseBundle.Instance;
using JCI  = GooglePlayGames.Android.Java.Context.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class BaseBundle {

        public static readonly string ClassName               = "BaseBundle";
        public static readonly string PackageName             =  "android.os";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JBBI MakeInstance() => new(FullyQualifiedClassName, inherit: false);

        public static JBBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance : JCI {

            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            public string GetString(string key)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getString(string)");
                return Call<string>("getString", key);
            }

        }

    }

}

#endif