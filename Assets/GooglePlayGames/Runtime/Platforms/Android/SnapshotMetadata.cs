#if UNITY_ANDROID

using System;

using GooglePlayGames.Utils;

using UAJO = UnityEngine.AndroidJavaObject;

using AISGM = GooglePlayGames.Api.SavedGame.ISavedGameMetadata;

using ASM = GooglePlayGames.Android.SnapshotMetadata;

using JSCI = GooglePlayGames.Android.Java.SnapshotContents.Instance;
using JSI  = GooglePlayGames.Android.Java.Snapshot.Instance;
using JSMI = GooglePlayGames.Android.Java.SnapshotMetadata.Instance;

namespace GooglePlayGames.Android {

    internal class SnapshotMetadata : AISGM {

        public SnapshotMetadata(JSI jSnapshot)
        {
            JSnapshot         = jSnapshot;
            JSnapshotMetadata = jSnapshot.JGetMetadata();
            JSnapshotContents = jSnapshot.JGetSnapshotContents();
        }

        public SnapshotMetadata(JSMI jSnapshotMetadata, JSCI jSnapshotContents)
        {
            JSnapshot         = null;
            JSnapshotMetadata = jSnapshotMetadata;
            JSnapshotContents = jSnapshotContents;
        }

        public JSI  JSnapshot         { get; }
        public JSMI JSnapshotMetadata { get; }
        public JSCI JSnapshotContents { get; }

        public bool? IsClosed() => JSnapshotContents?.IsClosed();

        #region Backward compatibility

        [Obsolete("Use JSnapshot instead")]         public UAJO JavaSnapshot => JSnapshot;
        [Obsolete("Use JSnapshotMetadata instead")] public UAJO JavaMetadata => JSnapshotMetadata;
        [Obsolete("Use JSnapshotContents instead")] public UAJO JavaContents => JSnapshotContents;

        #endregion Backward compatibility

        #region ISavedGameMetadata implementation

        public string   CoverImageUrl        => JSnapshotMetadata.GetCoverImageUrl();
        public string   Description          => JSnapshotMetadata.GetDescription();
        public string   Filename             => JSnapshotMetadata.GetUniqueName();
        public bool     IsOpen               => JSnapshotContents != null && !JSnapshotContents.IsClosed();
        public TimeSpan PlayedTime           => TimeSpan.FromMilliseconds(JSnapshotMetadata.GetPlayedTime());
        public DateTime LastModifiedDateTime => Utility.ToAndroidDateTime(JSnapshotMetadata.GetLastModifiedTimestamp());

        #endregion ISavedGameMetadata implementation

        #region Object implementation

        private bool m_stringify = false;

        public override string ToString()
        {
            if (m_stringify) return $"SnapshotMetadata(...)";
            try {
                m_stringify = true;
                return $"SnapshotMetadata(JSnapshot: {JSnapshot}, JSnapshotContents: {JSnapshotContents}, JSnapshotMetadata: {JSnapshotMetadata})";
            } finally {
                m_stringify = false;
            }
        }

        public override int GetHashCode() => HashCode.Combine(JSnapshot, JSnapshotContents, JSnapshotMetadata);

        public override bool Equals(object other)
        {
            if (other is not ASM it) return false;
            return Utility.Equals(JSnapshot,         it.JSnapshot)         &&
                   Utility.Equals(JSnapshotContents, it.JSnapshotContents) &&
                   Utility.Equals(JSnapshotMetadata, it.JSnapshotMetadata);
        }

        #endregion Object implementation

    }

}

#endif