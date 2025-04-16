#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JAOB  = GooglePlayGames.Android.Java.AdvertisingOptions.Builder;
using JAOBI = GooglePlayGames.Android.Java.AdvertisingOptions.Builder.Instance;
using JAOI  = GooglePlayGames.Android.Java.AdvertisingOptions.Instance;
using JSI   = GooglePlayGames.Android.Java.Strategy.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class AdvertisingOptions {

        public static readonly string ClassName               = "AdvertisingOptions";
        public static readonly string PackageName             =  "com.google.android.gms.nearby.connection";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JAOBI MakeBuilder() => JAOB.MakeInstance();

        public static class Builder {

            public static readonly string ClassName               = $"{DiscoveryOptions.ClassName}$Builder";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static JAOBI MakeInstance() => new();
            
            public static JAOBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

            internal sealed class Instance : JOI {

                internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
                {
                    if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
                }

                internal Instance() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
                }

                public JAOI JBuild()
                {
                    Logger.t($"JNI: Call {FullyQualifiedClassName}.build()");
                    return Call<JAOI>("build");
                }

                public JAOBI JSetStrategy(JSI jStrategy)
                {
                    Logger.t($"JNI: Call {FullyQualifiedClassName}.setStrategy(Strategy)");
                    return Call<JAOBI>("setStrategy", jStrategy);
                }

            }

        }

        public static JAOI MakeInstance() => new();
        
        public static JAOI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            public Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

        }

    }

}

#endif