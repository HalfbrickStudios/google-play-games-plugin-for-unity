#if UNITY_ANDROID

using System;

using GooglePlayGames.OurUtils;

using UAJO = UnityEngine.AndroidJavaObject;

using JC = GooglePlayGames.JavaClass;

using JP   = GooglePlayGames.Android.Java.Payload;
using JPC  = GooglePlayGames.Android.Java.Payload.Class;
using JPI  = GooglePlayGames.Android.Java.Payload.Instance;
using JPTC = GooglePlayGames.Android.Java.Payload.Type.Class;
using JOI  = GooglePlayGames.Android.Java.Object.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class Payload {

        public static readonly string ClassName               =  "Payload";
        public static readonly string PackageName             =  "com.google.android.gms.nearby.connection";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        internal static class Type {

            public static readonly string ClassName               = $"{JP.ClassName}";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static int BYTES  => JPTC.Instance.BYTES;
            public static int FILE   => JPTC.Instance.FILE;
            public static int STREAM => JPTC.Instance.STREAM;

            public static JPTC MakeClass() => new();

            internal sealed class Class : JC {

                private static JPTC s_instance = null;

                public static JPTC Instance => s_instance ??= new JPTC();

                internal Class() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
                }

                public int BYTES
                {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.BYTES");
                        return GetStatic<int>("BYTES");
                    }
                }

                public int FILE {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.FILE");
                        return GetStatic<int>("FILE");
                    }
                }

                public int STREAM
                {
                    get {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.STREAM");
                        return GetStatic<int>("STREAM");
                    }
                }

            }

        }

        public static JPC MakeClass() => new();

        public static JPI JFromBytes(byte[] bytes) => JPC.Instance.JFromBytes(bytes);

        internal sealed class Class : JC {

            private static JPC s_instance = null;

            public static JPC Instance => s_instance ??= new JPC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JPI JFromBytes(byte[] bytes)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.fromBytes(byte[])");
                Misc.CheckNotNull(bytes, nameof(bytes));
                return CallStatic<JPI>("fromBytes", bytes);
            }

        }

        public static JPI MakeInstance() => new();

        public static JPI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            public Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public byte[] AsBytes()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.asBytes()");
                return Call<byte[]>("asBytes");
            }

            public int JGetType()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getType()");
                return Call<int>("getType");
            }

        }

    }

}

#endif