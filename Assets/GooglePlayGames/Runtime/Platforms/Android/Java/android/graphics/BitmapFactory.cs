#if UNITY_ANDROID

using GooglePlayGames.Utils;

using JC = GooglePlayGames.Android.JavaClass;

using JBFC = GooglePlayGames.Android.Java.BitmapFactory.Class;
using JBI = GooglePlayGames.Android.Java.Bitmap.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class BitmapFactory {

        public static readonly string ClassName               =  "BitmapFactory";
        public static readonly string PackageName             =  "android.graphics";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JBFC MakeClass() => new();

        public static JBI JDecodeByteArray(byte[] bytes, int offset, int length) => JBFC.Instance.JDecodeByteArray(bytes, offset, length);

        internal sealed class Class : JC {

            private static JBFC s_instance = null;

            public static JBFC Instance => s_instance ??= new JBFC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JBI JDecodeByteArray(byte[] bytes, int offset, int length)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.decodeByteArray(byte[], int, int)");
                Misc.CheckNotNull(bytes, nameof(bytes));
                return CallStatic<JBI>("decodeByteArray", bytes, offset, length);
            }

        }

    }

}

#endif