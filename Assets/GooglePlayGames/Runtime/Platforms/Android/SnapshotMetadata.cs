#if UNITY_ANDROID

using System;

using UnityEngine;

using GooglePlayGames.Api.SavedGame;

using JSCI = GooglePlayGames.Android.Java.SnapshotContents.Instance;
using JSI  = GooglePlayGames.Android.Java.Snapshot.Instance;
using JSMI = GooglePlayGames.Android.Java.SnapshotMetadata.Instance;

namespace GooglePlayGames.Android {

    internal class SnapshotMetadata : ISavedGameMetadata {

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

        #region ISavedGameMetadata implementation

        public string   CoverImageUrl        => JSnapshotMetadata.GetCoverImageUrl();
        public string   Description          => JSnapshotMetadata.GetDescription();
        public string   Filename             => JSnapshotMetadata.GetUniqueName();
        public bool     IsOpen               => JSnapshotContents != null && !JSnapshotContents.IsClosed();
        public TimeSpan PlayedTime           => TimeSpan.FromMilliseconds(JSnapshotMetadata.GetPlayedTime());
        public DateTime LastModifiedDateTime => Convert.ToAndroidDateTime(JSnapshotMetadata.GetLastModifiedTimestamp());

        #endregion ISavedGameMetadata implementation

        #region Backward compatibility

        [Obsolete("Use JSnapshot instead")]         public AndroidJavaObject JavaSnapshot => JSnapshot;
        [Obsolete("Use JSnapshotMetadata instead")] public AndroidJavaObject JavaMetadata => JSnapshotMetadata;
        [Obsolete("Use JSnapshotContents instead")] public AndroidJavaObject JavaContents => JSnapshotContents;

        #endregion Backward compatibility

    }

}

#endif