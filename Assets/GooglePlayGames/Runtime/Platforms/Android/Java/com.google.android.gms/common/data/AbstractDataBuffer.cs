#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JBI = GooglePlayGames.Android.Java.Bundle.Instance;
using JDB = GooglePlayGames.Android.Java.DataBuffer;

namespace GooglePlayGames.Android.Java {

    internal static class AbstractDataBuffer {

        public static readonly string ClassName               =  "AbstractDataBuffer";
        public static readonly string PackageName             =  "com.google.android.gms.common.data";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";
        
        public static Instance<T> MakeInstance<T>() => new(FullyQualifiedClassName, inherit: false);

        public static Instance<T> WrapInstance<T>(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance<T> : JDB.Instance<T> {

            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            public JBI JGetMetadata()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getMetadata()");
                return Call<JBI>("getMetadata");
            }

        }

    }

}

#endif