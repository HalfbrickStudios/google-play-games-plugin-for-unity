using GooglePlayGames.OurUtils;

using UAJC = UnityEngine.AndroidJavaClass;
using UAJO = UnityEngine.AndroidJavaObject;

namespace GooglePlayGames {

    internal class JavaClass : UAJC {

        private static string Normalize(string name)
        {
            var result = name;
            if (string.IsNullOrEmpty(name)) {
                // No-op
            } else if (name.StartsWith("L") && name.EndsWith(";")) {
                result = name[1..^1].Replace('/', '.');
            } else {
                result = name.Replace('/', '.');
            }
            if (result != name) {
                Logger.d($"JNI: Normalized \"{name}\" => \"{result}\"");
            }
            return result; 
        }

        protected JavaClass(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
        {
            if (!inherit) Logger.t($"JNI: Initializing {fullyQualifiedClassName}");
        }

        public new FieldType GetStatic<FieldType>(string name)
        {
            FieldType nativeCall()  => base.GetStatic<FieldType>(name);
            UAJO      wrappedCall() => base.GetStatic<UAJO>(name);
            return Jni.WrapCall(nativeCall, wrappedCall);
        }

        public new void CallStatic(string name, params object[] args)
        {
            Jni.DowncastParameters(args);
            base.CallStatic(name, args);
        }

        public new ReturnType CallStatic<ReturnType>(string name, params object[] args)
        {
            Jni.DowncastParameters(args);
            ReturnType nativeCall()  => base.CallStatic<ReturnType>(name, args);
            UAJO       wrappedCall() => base.CallStatic<UAJO>(name, args);
            return Jni.WrapCall(nativeCall, wrappedCall);
        }

        public new FieldType Get<FieldType>(string name)
        {
            FieldType nativeCall()  => base.Get<FieldType>(name);
            UAJO      wrappedCall() => base.Get<UAJO>(name);
            return Jni.WrapCall(nativeCall, wrappedCall);
        }

        public new void Call(string name, params object[] args)
        {
            Jni.DowncastParameters(args);
            base.Call(name, args);
        }

        public new ReturnType Call<ReturnType>(string name, params object[] args)
        {
            Jni.DowncastParameters(args);
            ReturnType nativeCall()  => base.Call<ReturnType>(name, args);
            UAJO       wrappedCall() => base.Call<UAJO>(name, args);
            return Jni.WrapCall(nativeCall, wrappedCall);
        }

    }

}