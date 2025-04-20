#if UNITY_ANDROID

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JC = GooglePlayGames.Android.JavaClass;

using JTC = GooglePlayGames.Android.Java.Tasks.Class;
using JTI = GooglePlayGames.Android.Java.Task.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Tasks {

        public static readonly string ClassName               =  "Tasks";
        public static readonly string PackageName             =  "com.google.android.gms.tasks";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JTC MakeClass() => new();

        public static JTI JWhenAll(UAJO jTasks) => JTC.Instance.JWhenAll(jTasks);

        internal sealed class Class : JC {

            private static JTC s_instance = null;

            public static JTC Instance => s_instance ??= new JTC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JTI JWhenAll(UAJO jTasks)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.whenAll(Task[])");
                return CallStatic<UAJO>("whenAll", jTasks) as JTI;
            }

        }

    }

}

#endif