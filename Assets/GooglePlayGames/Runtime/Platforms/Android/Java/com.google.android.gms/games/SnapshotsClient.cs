#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using JC = GooglePlayGames.JavaClass;

using JAD   = GooglePlayGames.Android.Java.AnnotatedData;
using JDOC  = GooglePlayGames.Android.Java.SnapshotsClient.DataOrConflict;
using JOI   = GooglePlayGames.Android.Java.Object.Instance;
using JT    = GooglePlayGames.Android.Java.Task;
using JSBI  = GooglePlayGames.Android.Java.SnapshotMetadataBuffer.Instance;
using JSI   = GooglePlayGames.Android.Java.Snapshot.Instance;
using JSCI  = GooglePlayGames.Android.Java.SnapshotsClient.SnapshotConflict.Instance;
using JSCsI = GooglePlayGames.Android.Java.SnapshotContents.Instance;
using JSsCC = GooglePlayGames.Android.Java.SnapshotsClient.Class;
using JSsCI = GooglePlayGames.Android.Java.SnapshotsClient.Instance;
using JSMI  = GooglePlayGames.Android.Java.SnapshotMetadata.Instance;
using JSMCI = GooglePlayGames.Android.Java.SnapshotMetadataChange.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class SnapshotsClient {

        public static readonly string ClassName               =  "SnapshotsClient";
        public static readonly string PackageName             =  "com.google.android.gms.games";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        internal static class DataOrConflict {

            public static readonly string ClassName               = $"{SnapshotsClient.ClassName}$DataOrConflict";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static Instance<T> MakeInstance<T>() => new();
            
            public static Instance<T> WrapInstance<T>(UAJO jObject) => new(jObject.GetRawObject());

            internal sealed class Instance<T> : JOI {

                internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
                {
                    if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
                }

                internal Instance() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
                }

                public JSCI JGetConflict()
                {
                    Logger.t($"JNI: Call {FullyQualifiedClassName}.getConflict()");
                    return Call<JSCI>("getConflict");
                }

                public T JGetData()
                {
                    Logger.t($"JNI: Call {FullyQualifiedClassName}.getData()");
                    return Call<T>("getData");
                }

                public bool IsConflict()
                {
                    Logger.t($"JNI: Call {FullyQualifiedClassName}.isConflict()");
                    return Call<bool>("isConflict");
                }

            }

        }

        internal static class SnapshotConflict {

            public static readonly string ClassName               = $"{SnapshotsClient.ClassName}SnapshotConflict";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static JSCI MakeInstance() => new();
            
            public static JSCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

            internal sealed class Instance : JOI {

                internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
                {
                    if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
                }

                public Instance() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
                }

                public string GetConflictId()
                {
                    Logger.t($"JNI: Call {FullyQualifiedClassName}.getConflictId()");
                    return Call<string>("getConflictId");
                }

                public JSI JGetConflictingSnapshot()
                {
                    Logger.t($"JNI: Call {FullyQualifiedClassName}.getConflictingSnapshot()");
                    return Call<JSI>("getConflictingSnapshot");
                }

                public JSI JGetSnapshot()
                {
                    Logger.t($"JNI: Call {FullyQualifiedClassName}.getSnapshot()");
                    return Call<JSI>("getSnapshot");
                }

                public JSCsI JGetResolutionSnapshotContents()
                {
                    Logger.t($"JNI: Call {FullyQualifiedClassName}.getResolutionSnapshotContents()");
                    return Call<JSCsI>("getResolutionSnapshotContents");
                }

            }

        }

        public static int RESOLUTION_POLICY_HIGHEST_PROGRESS       => JSsCC.Instance.RESOLUTION_POLICY_HIGHEST_PROGRESS;
        public static int RESOLUTION_POLICY_LAST_KNOWN_GOOD        => JSsCC.Instance.RESOLUTION_POLICY_LAST_KNOWN_GOOD;
        public static int RESOLUTION_POLICY_LONGEST_PLAYTIME       => JSsCC.Instance.RESOLUTION_POLICY_LONGEST_PLAYTIME;
        public static int RESOLUTION_POLICY_MANUAL                 => JSsCC.Instance.RESOLUTION_POLICY_MANUAL;
        public static int RESOLUTION_POLICY_MOST_RECENTLY_MODIFIED => JSsCC.Instance.RESOLUTION_POLICY_MOST_RECENTLY_MODIFIED;

        public static JSsCC MakeClass() => new();

        internal sealed class Class : JC {

            private static JSsCC s_instance = null;

            public static JSsCC Instance => s_instance ??= new JSsCC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public int RESOLUTION_POLICY_HIGHEST_PROGRESS
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.RESOLUTION_POLICY_HIGHEST_PROGRESS");
                    return GetStatic<int>("RESOLUTION_POLICY_HIGHEST_PROGRESS");
                }
            }

            public int RESOLUTION_POLICY_LAST_KNOWN_GOOD
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.RESOLUTION_POLICY_LAST_KNOWN_GOOD");
                    return GetStatic<int>("RESOLUTION_POLICY_LAST_KNOWN_GOOD");
                }
            }

            public int RESOLUTION_POLICY_LONGEST_PLAYTIME
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.RESOLUTION_POLICY_LONGEST_PLAYTIME");
                    return GetStatic<int>("RESOLUTION_POLICY_LONGEST_PLAYTIME");
                }
            }

            public int RESOLUTION_POLICY_MANUAL
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.RESOLUTION_POLICY_MANUAL");
                    return GetStatic<int>("RESOLUTION_POLICY_MANUAL");
                }
            }

            public int RESOLUTION_POLICY_MOST_RECENTLY_MODIFIED
            {
                get {
                    Logger.t($"JNI: Reading {FullyQualifiedClassName}.RESOLUTION_POLICY_MOST_RECENTLY_MODIFIED");
                    return GetStatic<int>("RESOLUTION_POLICY_MOST_RECENTLY_MODIFIED");
                }
            }

        }

        public static JSsCI MakeInstance() => new();
        
        public static JSsCI WrapInstance(UAJO jObject) => new(jObject.GetRawObject(), noLog: false);

        internal sealed class Instance : JOI {

            internal Instance(IntPtr pointer, bool noLog = true) : base(pointer)
            {
                if (!noLog) Logger.t($"JNI: Wrapping instance of {FullyQualifiedClassName}");
            }

            internal Instance() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Creating instance of {FullyQualifiedClassName}");
            }

            public JT.Instance<JSMI> JCommitAndClose(JSI jSnapshot, JSMCI jChange)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.commitAndClose(Snapshot, SnapshotMetadataChange)");
                return Call<UAJO>("commitAndClose", jSnapshot, jChange) as JT.Instance<JSMI>;
            }

            public JT.Instance<string> JDelete(JSMI jSnapshotMetadata)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.delete(SnapshotMetadata)");
                return Call<UAJO>("delete", jSnapshotMetadata) as JT.Instance<string>;
            }

            public JT.Instance<JDOC.Instance<JSI>> JOpen(string filename, bool create, int jPolicy)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.open(string, bool, ResolutionPolicy)");
                return Call<UAJO>("open", filename, create, jPolicy) as JT.Instance<JDOC.Instance<JSI>>;
            }

            public JT.Instance<JAD.Instance<JSBI>> JLoad(bool reload)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.load(bool)");
                return Call<UAJO>("load", reload) as JT.Instance<JAD.Instance<JSBI>>;
            }

            public JT.Instance<JDOC.Instance<JSI>> JResolveConflict(string conflict, string snapshot, JSMCI jChange, JSCsI jContents)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.resolveConflict(string, string, SnapshotMetadataChange, SnapshotContents)");
                return Call<UAJO>("resolveConflict", conflict, snapshot, jChange, jContents) as JT.Instance<JDOC.Instance<JSI>>;
            }

            public JT.Instance<JDOC.Instance<JSI>> JResolveConflict(string conflict, JSI jSnapshot)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.resolveConflict(string, Snapshot)");
                return Call<UAJO>("resolveConflict", conflict, jSnapshot) as JT.Instance<JDOC.Instance<JSI>>;
            }

        }

    }

}

#endif