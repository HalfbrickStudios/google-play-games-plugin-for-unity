using System;
using System.Reflection;

using UAJO = UnityEngine.AndroidJavaObject;

using JO = GooglePlayGames.JavaObject;

namespace GooglePlayGames {

    internal static class Jni {

        public static bool IsStrictSubclass(Type child, Type parent) => child != parent && child.IsSubclassOf(parent);

        public static void DowncastParameters(params object[] args)
        {
            for (var i = 0; i < args.Length; i += 1) {
                var arg = args[i];
                if (IsStrictSubclass(arg.GetType(), typeof(UAJO))) {
                    var casted = arg as UAJO;
                    args[i] = new UAJO(casted.GetRawObject());
                }
            }
        }

        public static bool TryWrap<WrappingSubType>(UAJO jObject, out WrappingSubType result)
        {
            var type        = typeof(WrappingSubType);
            var flags       = BindingFlags.Instance | BindingFlags.NonPublic;
            var binder      = null as Binder;
            var intpType    = typeof(IntPtr);
            var modifiers   = null as ParameterModifier[];
            var boolType    = typeof(bool);
            var constructor = type.GetConstructor(flags, binder, new[] { intpType }, modifiers);
            if (constructor != null) {
                result = (WrappingSubType)constructor.Invoke(new object[] { jObject.GetRawObject() });
                return true;
            }

            constructor = type.GetConstructor(flags, binder, new[] { intpType, boolType }, modifiers);
            if (constructor != null) {
                result = (WrappingSubType)constructor.Invoke(new object[] { jObject.GetRawObject(), true });
                return true;
            }

            result = default;
            return false;
        }

        public static WrappingSubType Wrap<WrappingSubType>(UAJO jObject)
        {
            if (TryWrap(jObject, out WrappingSubType result)) {
                return result;
            }
            throw new NotImplementedException($"No wrapping constructor found for {typeof(WrappingSubType)} with IntPtr (and optionally a bool) as parameters");
        }

        public delegate ReturnType WrappedCallDelegate<ReturnType>();

        public static ReturnType WrapCall<ReturnType>(WrappedCallDelegate<ReturnType> nativeCall, WrappedCallDelegate<UAJO> wrappedCall)
        {
            var type = typeof(ReturnType);
            if (IsStrictSubclass(type, typeof(JO))) {
                var jObject = wrappedCall();
                return Wrap<ReturnType>(jObject);
            } else if (IsStrictSubclass(type, typeof(UAJO))) {
                throw new NotImplementedException("You are using a subclass of AndroidJavaObject that does not expose a (string, bool) constructor; this is a bug!");
            } else {
                return nativeCall();
            }
        }

    }

}