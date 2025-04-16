#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JEI  = GooglePlayGames.Android.Java.Exception.Instance;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JOCL = GooglePlayGames.Android.Java.OnCompleteListener;
using JOFL = GooglePlayGames.Android.Java.OnFailureListener;
using JOSL = GooglePlayGames.Android.Java.OnSuccessListener;
using JT   = GooglePlayGames.Android.Java.Task;
using JTI  = GooglePlayGames.Android.Java.Task.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Task {

        public static readonly string ClassName               =  "Task";
        public static readonly string PackageName             =  "com.google.android.gms.tasks";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JTI MakeInstance() => new(FullyQualifiedClassName, inherit: false);

        public static JTI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), inherit: false);

        internal class Instance : JOI {

            internal Instance(IntPtr pointer, bool inherit = true) : base(pointer)
            {
                if (!inherit) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance(string fullyQualifiedClassName, bool inherit = true) : base(fullyQualifiedClassName)
            {
                if (!inherit) Logger.t($"JNI: Creating instance of {fullyQualifiedClassName}");
            }

            public JTI JAddOnSuccessListener(JOSL.Proxy<JOI> jListener)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.addOnSuccessListener(OnSuccessListener)");
                Misc.CheckNotNull(jListener, nameof(jListener));
                return Call<UAJO>("addOnSuccessListener", jListener) as JTI;
            }

            public JEI JGetException()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getException()");
                return Call<UAJO>("getException") as JEI;
            }

            public bool JIsCanceled()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.isCanceled()");
                return Call<bool>("isCanceled");
            }

            public bool JIsSuccessful()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.isSuccessful()");
                return Call<bool>("isSuccessful");
            }

        }

        public static Instance<TResult> MakeInstance<TResult>() => new();

        public static Instance<TResult> WrapInstance<TResult>(UAJO jObject) => new(jObject.GetRawObject());

        internal sealed class Instance<TResult> : JTI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}<TResult>");
            }

            public Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}<TResult>");
            }

            public Instance<TResult> JAddOnCompleteListener(JOCL.Proxy<TResult> jListener)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<TResult>.addOnCompleteListener(OnCompleteListener)");
                Misc.CheckNotNull(jListener, nameof(jListener));
                return Call<UAJO>("addOnCompleteListener", jListener) as Instance<TResult>;
            }

            public Instance<TResult> JAddOnFailureListener(JOFL.Proxy jListener)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<TResult>.addOnFailureListener(OnFailureListener)");
                Misc.CheckNotNull(jListener, nameof(jListener));
                return Call<UAJO>("addOnFailureListener", jListener) as Instance<TResult>;
            }

            public Instance<TResult> JAddOnSuccessListener(JOSL.Proxy<TResult> jListener)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<TResult>.addOnSuccessListener(OnSuccessListener)");
                Misc.CheckNotNull(jListener, nameof(jListener));
                return Call<UAJO>("addOnSuccessListener", jListener) as Instance<TResult>;
            }

            public TResult JGetResult()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}<TResult>.getResult()");
                return Call<TResult>("getResult");
            }

        }

    }

    internal static class TaskExtensions {

        public static JT.Instance<TResult> JAddOnCompleteListener<TResult>(this JT.Instance<TResult> self, Action<JT.Instance<TResult>> callback)
        {
            var jListener = JOCL.MakeProxy<TResult>(it => callback(it));
            using (self.JAddOnCompleteListener(jListener)) {
                return self;
            }
        }

        public static JT.Instance<TResult> JAddOnFailureListener<TResult>(this JT.Instance<TResult> self, Action<JEI> callback)
        {
            var jListener = JOFL.MakeProxy(it => callback(it));
            using (self.JAddOnFailureListener(jListener)) {
                return self;
            }
        }

        public static JTI JAddOnSuccessListener(this JTI self, Action callback)
        {
            return JAddOnSuccessListener(self, dispose: true, callback);
        }

        public static JTI JAddOnSuccessListener(this JTI self, bool dispose, Action callback)
        {
            var jListener = JOSL.MakeProxy<JOI>(dispose, _ => callback());
            using (self.JAddOnSuccessListener(jListener)) {
                return self;
            }
        }

        public static JT.Instance<TResult> JAddOnSuccessListener<TResult>(this JT.Instance<TResult> self, Action<TResult> callback)
        {
            return JAddOnSuccessListener(self, dispose: true, callback);
        }

        public static JT.Instance<TResult> JAddOnSuccessListener<TResult>(this JT.Instance<TResult> self, bool dispose, Action<TResult> callback)
        {
            var jListener = JOSL.MakeProxy<TResult>(dispose, it => callback(it));
            using (self.JAddOnSuccessListener(jListener)) {
                return self;
            }
        }

    }

}

#endif