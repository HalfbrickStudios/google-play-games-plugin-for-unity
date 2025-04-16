#if UNITY_ANDROID

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using AEV = GooglePlayGames.Api.Events.EventVisibility;

using JEI = GooglePlayGames.Android.Java.Event.Instance;
using JOI = GooglePlayGames.Android.Java.Object.Instance;
using System;

namespace GooglePlayGames.Android.Java {

    internal static class Event {

        public static readonly string ClassName               =  "Event";
        public static readonly string PackageName             =  "com.google.android.gms.games.event";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JEI MakeInstance() => new();

        public static JEI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

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
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getEventId()");
                return Call<string>("getEventId");
            }

            public string GetName()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getName()");
                return Call<string>("getName");
            }

            public string GetDescription()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getDescription()");
                return Call<string>("getDescription");
            }

            public string GetIconImageUrl()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getIconImageUrl()");
                return Call<string>("getIconImageUrl");
            }

            public long GetValue()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getValue()");
                return Call<long>("getValue");
            }

            public bool IsVisible()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.isVisible()");
                return Call<bool>("isVisible");
            }

        }

    }

    internal static class EventExtensions {

        public static AEV GetEventVisibility(this JEI self) => Convert.ToAndroidEventVisibility(self.IsVisible());

    }

}

#endif