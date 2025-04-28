#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JC  = GooglePlayGames.Android.JavaClass;
using JCC = GooglePlayGames.Android.Java.Class.Klass;
using JCI = GooglePlayGames.Android.Java.Class.Instance;
using JOI = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Class {

        public static readonly string ClassName               =  "Class";
        public static readonly string PackageName             =  "java.lang";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JCC MakeClass() => new();

        public static JCI JForName(string name) => JCC.Instance.JForName(name);

        internal sealed class Klass : JC {

            private static JCC s_instance = null;

            public static JCC Instance => s_instance ??= new JCC();

            internal Klass() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JCI JForName(string name)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.forName(string)");
                Misc.CheckNotNull(name, nameof(name));
                return CallStatic<JCI>("forName", name);
            }

        }

        public static JCI MakeInstance() => new(FullyQualifiedClassName, inherit: false);

        public static JCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance : JOI {

            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            public string GetName()
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getName()");
                return Call<string>("getName");
            }

            public bool IsAssignableFrom(JCI jClass)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.isAssignableFrom(Class)");
                return Call<bool>("isAssignableFrom", jClass);
            }

        }

    }

}

#endif