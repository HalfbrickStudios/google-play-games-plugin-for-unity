#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class DataBuffer {

        public static readonly string ClassName               =  "DataBuffer";
        public static readonly string PackageName             =  "com.google.android.gms.common.data";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static Instance<T> MakeInstance<T>() => new(FullyQualifiedClassName, inherit: false);

        public static Instance<T> WrapInstance<T>(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance<T> : JOI {

            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            public T JGet(int index)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.get(int)");
                return Call<T>("get", index);
            }

            public int GetCount()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getCount()");
                return Call<int>("getCount");
            }

            public void Release()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.release()");
                Call("release");
            }

        }

    }

}

#endif