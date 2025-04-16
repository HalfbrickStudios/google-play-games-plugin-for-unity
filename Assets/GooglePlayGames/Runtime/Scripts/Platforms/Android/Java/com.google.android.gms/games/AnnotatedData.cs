#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using ARS = GooglePlayGames.BasicApi.ResponseStatus;

using JAD = GooglePlayGames.Android.Java.AnnotatedData;
using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class AnnotatedData {

        public static readonly string ClassName               =  "AnnotatedData";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static Instance<T> MakeInstance<T>() => new();
        
        public static Instance<T> WrapInstance<T>(UAJO jObject) => new(jObject.GetRawObject());

        internal sealed class Instance<T> : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public T JGet()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.get()");
                return Call<T>("get");
            }

            public bool IsStale()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.isStale()");
                return Call<bool>("isStale");
            }

        }

    }

    internal static class AnnotatedDataExtensions {

        public static ARS GetResponseStatus<T>(this JAD.Instance<T> self) => Convert.ToAndroidResponseStatus(self.IsStale());

    }

}

#endif