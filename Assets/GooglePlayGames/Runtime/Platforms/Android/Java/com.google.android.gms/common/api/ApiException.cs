#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JAEI = GooglePlayGames.Android.Java.ApiException.Instance;
using JEI  = GooglePlayGames.Android.Java.Exception.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class ApiException {

        public static readonly string ClassName               =  "ApiException";
        public static readonly string PackageName             =  "com.google.android.gms.common.api";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JAEI MakeInstance() => new(FullyQualifiedClassName, inherit: false);

        public static JAEI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance : JEI {

            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            public int GetStatusCode()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getStatusCode()");
                return Call<int>("getStatusCode");
            }

        }

    }

}

#endif