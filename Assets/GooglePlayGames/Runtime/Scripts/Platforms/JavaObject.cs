using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

namespace GooglePlayGames {

    internal class JavaObject : UAJO {

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

        protected JavaObject(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
        {
            if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
        }

        protected JavaObject(IntPtr pointer, bool inherit = true) : base(pointer)
        {
            if (!inherit) Logger.t($"JNI: Wrapping instance of {pointer}");
        }

        protected JavaObject(string fullyQualifiedClassName, params object[] args) : base(fullyQualifiedClassName, args) { }

        public new FieldType GetStatic<FieldType>(string name)
        {
            FieldType primitiveCall() => base.GetStatic<FieldType>(name);
            UAJO      subclassCall()  => base.GetStatic<UAJO>(name);
            return Jni.WrapCall(primitiveCall, subclassCall);
        }

        public new void CallStatic(string name, params object[] args)
        {
            Jni.DowncastParameters(args);
            base.CallStatic(name, args);
        }

        public new ReturnType CallStatic<ReturnType>(string name, params object[] args)
        {
            Jni.DowncastParameters(args);
            ReturnType primitiveCall() => base.CallStatic<ReturnType>(name, args);
            UAJO       subclassCall()  => base.CallStatic<UAJO>(name, args);
            return Jni.WrapCall(primitiveCall, subclassCall);
        }

        public new FieldType Get<FieldType>(string name)
        {
            FieldType primitiveCall() => base.Get<FieldType>(name);
            UAJO      subclassCall()  => base.Get<UAJO>(name);
            return Jni.WrapCall(primitiveCall, subclassCall);
        }

        public new void Call(string name, params object[] args)
        {
            Jni.DowncastParameters(args);
            base.Call(name, args);
        }

        public new ReturnType Call<ReturnType>(string name, params object[] args)
        {
            Jni.DowncastParameters(args);
            ReturnType primitiveCall() => base.Call<ReturnType>(name, args);
            UAJO       subclassCall()  => base.Call<UAJO>(name, args);
            return Jni.WrapCall(primitiveCall, subclassCall);
        }

    }

}