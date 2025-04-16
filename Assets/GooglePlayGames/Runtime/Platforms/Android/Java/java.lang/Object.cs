#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JO = GooglePlayGames.JavaObject;

using JCI = GooglePlayGames.Android.Java.Class.Instance;
using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Object {

        public static readonly string ClassName               =  "Object";
        public static readonly string PackageName             =  "java.lang";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JOI MakeInstance() => new(FullyQualifiedClassName, inherit: false);
        
        public static JOI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance : JO {
            
            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName) 
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            protected Instance(string fullyQualifiedClassName, params object[] args) : base(fullyQualifiedClassName, args) { }

            public JCI JGetClass()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getClass()");
                return Get<JCI>("getClass");
            }

            public string JToString()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.toString()");
                return Call<string>("toString");
            }

        }

    }

}

#endif