#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JEBI = GooglePlayGames.Android.Java.EventBuffer.Instance;
using JADB = GooglePlayGames.Android.Java.AbstractDataBuffer;
using JAI  = GooglePlayGames.Android.Java.Event.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class EventBuffer {

        public static readonly string ClassName               =  "EventBuffer";
        public static readonly string PackageName             =  "com.google.android.gms.games.event";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JEBI MakeInstance() => new();
        
        public static JEBI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JADB.Instance<JAI> {

            [Preserve]
            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

        }

    }

}

#endif