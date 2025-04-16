#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JOI  = GooglePlayGames.Android.Java.Object.Instance;
using JPSI = GooglePlayGames.Android.Java.PlayerStats.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class PlayerStats {

        public static readonly string ClassName               =  "PlayerStats";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JPSI MakeInstance() => new();
        
        public static JPSI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public float GetAverageSessionLength()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getAverageSessionLength()");
                return Call<float>("getAverageSessionLength");
            }

            public float GetChurnProbability()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getChurnProbability()");
                return Call<float>("getChurnProbability");
            }

            public int GetDaysSinceLastPlayed()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getDaysSinceLastPlayed()");
                return Call<int>("getDaysSinceLastPlayed");
            }

            public float GetHighSpenderProbability()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getHighSpenderProbability()");
                return Call<float>("getHighSpenderProbability");
            }

            public int GetNumberOfPurchases()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getNumberOfPurchases()");
                return Call<int>("getNumberOfPurchases");
            }

            public int GetNumberOfSessions()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getNumberOfSessions()");
                return Call<int>("getNumberOfSessions");
            }

            public float GetSessionPercentile()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getSessionPercentile()");
                return Call<float>("getSessionPercentile");
            }

            public float GetSpendPercentile()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getSpendPercentile()");
                return Call<float>("getSpendPercentile");
            }

            public float GetSpendProbability()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getSpendProbability()");
                return Call<float>("getSpendProbability");
            }

            public float GetTotalSpendNext28Days()
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getTotalSpendNext28Days()");
                return Call<float>("getTotalSpendNext28Days");
            }

        }

    }

}

#endif