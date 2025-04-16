#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class ArrayList {

        public static readonly string ClassName               =  "ArrayList";
        public static readonly string PackageName             =  "java.util";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static Instance<E> MakeInstance<E>() => new();
        
        public static Instance<E> WrapInstance<E>(UAJO jObject) => new(jObject.GetRawObject());

        internal sealed class Instance<E> : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public bool Add(E element)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<E>.add(E)");
                return Call<bool>("add", element);
            }

            public E JGet(int index)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<E>.get(int)");
                return Call<E>("get", index);
            }

        }

    }

}

#endif