#if UNITY_ANDROID

using System;

using UnityEngine.Scripting;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using AEV = GooglePlayGames.Api.Events.EventVisibility;

using JEI = GooglePlayGames.Android.Java.Event.Instance;
using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Event {

        public static readonly string ClassName               =  "Event";
        public static readonly string PackageName             =  "com.google.android.gms.games.event";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JEI MakeInstance() => new();

        public static JEI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

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

            public string GetEventId()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getEventId()");
                return Call<string>("getEventId");
            }

            public string GetName()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getName()");
                return Call<string>("getName");
            }

            public string GetDescription()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getDescription()");
                return Call<string>("getDescription");
            }

            public string GetIconImageUrl()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getIconImageUrl()");
                return Call<string>("getIconImageUrl");
            }

            public long GetValue()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getValue()");
                return Call<long>("getValue");
            }

            public bool IsVisible()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.isVisible()");
                return Call<bool>("isVisible");
            }

        }

    }

}

namespace GooglePlayGames.Android.Java.Extensions {

    internal static class EventExtensions {

        public static AEV GetEventVisibility(this JEI self) => Utility.ToAndroidEventVisibility(self.IsVisible());

    }

}

#endif