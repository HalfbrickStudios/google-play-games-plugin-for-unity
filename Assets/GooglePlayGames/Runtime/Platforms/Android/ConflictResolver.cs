#if UNITY_ANDROID

using System;

using GooglePlayGames.Android.Java.Extensions;
using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

using AICR  = GooglePlayGames.Api.SavedGame.IConflictResolver;
using AISGM = GooglePlayGames.Api.SavedGame.ISavedGameMetadata;
using ASGMU = GooglePlayGames.Api.SavedGame.SavedGameMetadataUpdate;
using ASGRS = GooglePlayGames.Api.SavedGame.SavedGameRequestStatus;

using ASGC = GooglePlayGames.Android.SavedGameClient;
using ASM  = GooglePlayGames.Android.SnapshotMetadata;

using JSCI  = GooglePlayGames.Android.Java.SnapshotsClient.SnapshotConflict.Instance;
using JSsCI = GooglePlayGames.Android.Java.SnapshotsClient.Instance;

namespace GooglePlayGames.Android {

    internal class ConflictResolver : AICR {

        private readonly Action<ASGRS, AISGM> m_callback;
        private readonly JSCI                 m_conflict;
        private readonly ASM                  m_original;
        private readonly Action               m_retry;
        private readonly ASGC                 m_saveClient;
        private readonly JSsCI                m_jSnapClient;
        private readonly ASM                  m_unmerged;

        internal ConflictResolver(ASGC saveClient, JSsCI jSnapClient, JSCI conflict, ASM original, ASM unmerged, Action<ASGRS, AISGM> callback, Action retry)
        {
            m_callback    = Misc.CheckNotNull(callback);
            m_conflict    = Misc.CheckNotNull(conflict);
            m_jSnapClient = Misc.CheckNotNull(jSnapClient);
            m_original    = Misc.CheckNotNull(original);
            m_retry       = Misc.CheckNotNull(retry);
            m_saveClient  = saveClient;
            m_unmerged    = Misc.CheckNotNull(unmerged);
        }

        public void ResolveConflict(AISGM metadata, ASGMU update, byte[] bytes)
        {
            var casted = metadata as ASM;
            if (casted != m_original && casted != m_unmerged) {
                Logger.e("Caller attempted to choose a version of the metadata that was not part of the conflict");
                m_callback(ASGRS.BadInputError, null);
                return;
            }

            using var jContents = m_conflict.JGetResolutionSnapshotContents();
            if (!jContents.WriteBytes(bytes)) {
                Logger.e("Can't update snapshot contents during conflict resolution.");
                m_callback(ASGRS.BadInputError, null);
                return;
            }

            using var jChange = Utility.ToJavaSnapshotMetadataChangeUpdate(update);
            var conflict = m_conflict.GetConflictId();
            var snapshot = casted.JSnapshotMetadata.GetSnapshotId();
            using var jTask = m_jSnapClient.JResolveConflict(conflict, snapshot, jChange, jContents);
            jTask.JAddOnSuccessListener(jData => m_retry()).JAddOnFailureListener(exception => {
                Logger.d("ResolveConflict failed: " + exception.JToString());
                m_callback(m_saveClient.JPlayClient.GetSavedGameRequestStatus(), null);
            });
        }

        public void ChooseMetadata(AISGM metadata)
        {
            var casted = metadata as ASM;
            if (casted != m_original && casted != m_unmerged) {
                Logger.e("Caller attempted to choose a version of the metadata that was not part of the conflict");
                m_callback(ASGRS.BadInputError, null);
                return;
            }

            using var jTask = m_jSnapClient.JResolveConflict(m_conflict.GetConflictId(), casted.JSnapshot);
            jTask.JAddOnSuccessListener(jData => m_retry()).JAddOnFailureListener(jException => {
                Logger.d("ChooseMetadata failed: " + jException.JToString());
                m_callback(m_saveClient.JPlayClient.GetSavedGameRequestStatus(), null);
            });
        }
    }

}

#endif