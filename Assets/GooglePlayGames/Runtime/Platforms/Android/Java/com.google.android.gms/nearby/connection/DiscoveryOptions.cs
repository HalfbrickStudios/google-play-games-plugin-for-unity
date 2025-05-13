#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JDOB  = GooglePlayGames.Android.Java.DiscoveryOptions.Builder;
using JDOBI = GooglePlayGames.Android.Java.DiscoveryOptions.Builder.Instance;
using JDOI  = GooglePlayGames.Android.Java.DiscoveryOptions.Instance;
using JSI   = GooglePlayGames.Android.Java.Strategy.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class DiscoveryOptions {

        public static readonly string ClassName               =  "DiscoveryOptions";
        public static readonly string PackageName             =  "com.google.android.gms.nearby.connection";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JDOBI MakeBuilder() => JDOB.MakeInstance();

        public static class Builder {

            public static readonly string ClassName               = $"{DiscoveryOptions.ClassName}$Builder";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static JDOBI MakeInstance() => new();
            
            public static JDOBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

            internal sealed class Instance : JOI {

                [Preserve]
                internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
                {
                    if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
                }

                internal Instance() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
                }

                public JDOI JBuild()
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.build()");
                    return Call<JDOI>("build");
                }

                public JDOBI JSetStrategy(JSI jStrategy)
                {
                    Logger.t($"JNI: Calling {FullyQualifiedClassName}.setStrategy(Strategy)");
                    return Call<JDOBI>("setStrategy", jStrategy);
                }

            }

        }

        public static JDOI MakeInstance() => new();
        
        public static JDOI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            [Preserve]
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