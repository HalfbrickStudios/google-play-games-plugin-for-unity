#if UNITY_ANDROID

using GooglePlayGames.Utils;

using UAJC = UnityEngine.AndroidJavaClass;
using UAJO = UnityEngine.AndroidJavaObject;

namespace GooglePlayGames.Android {

    internal class JavaClass : UAJC {

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

#endif