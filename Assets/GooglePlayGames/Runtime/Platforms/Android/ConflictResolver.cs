#if UNITY_ANDROID

using System;

using GooglePlayGames.Android.Java.Extensions;
using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

using AICR  = GooglePlayGames.Api.SavedGame.IConflictResolver;
using AISGM = GooglePlayGames.Api.SavedGame.ISavedGameMetadata;
using ASGMU = GooglePlayGames.Api.SavedGame.SavedGameMetadataUpdate;
using ASGRS = GooglePlayGames.Api.SavedGame.SavedGameRequestStatus;

using ACR  = GooglePlayGames.Android.ConflictResolver;
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
            const string method = "IConflictResolver.ResolveConflict(ISavedGameMetadata, SavedGameMetadataUpdate, byte[])";
            Logger.t($"AND: Calling {method}");

            var casted = metadata as ASM;
            if (casted != m_original && casted != m_unmerged) {
                Logger.w("AND: Caller attempted to choose a version of the metadata that was not part of the conflict");
                m_callback?.Invoke(ASGRS.BadInputError, null);
                return;
            }

            using var jContents = m_conflict.JGetResolutionSnapshotContents();
            if (!jContents.WriteBytes(bytes)) {
                Logger.w("AND: Can't update snapshot contents during conflict resolution");
                m_callback?.Invoke(ASGRS.BadInputError, null);
                return;
            }

            using var jChange   = Utility.ToJavaSnapshotMetadataChangeUpdate(update);
                  var conflict  = m_conflict.GetConflictId();
            using var jMetadata = casted.JSnapshotMetadata;
                  var snapshot  = jMetadata.GetSnapshotId();
            using var jTask     = m_jSnapClient.JResolveConflict(conflict, snapshot, jChange, jContents);
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                m_retry();
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.w($"AND: {jException.JToString()}");
                m_callback?.Invoke(m_saveClient.JPlayClient.GetSavedGameRequestStatus(), null);
            });
        }

        public void ChooseMetadata(AISGM metadata)
        {
            const string method = "IConflictResolver.ChooseMetadata(ISavedGameMetadata)";
            Logger.t($"AND: Calling {method}");

            var casted = metadata as ASM;
            if (casted != m_original && casted != m_unmerged) {
                Logger.w("AND: Caller attempted to choose a version of the metadata that was not part of the conflict");
                m_callback?.Invoke(ASGRS.BadInputError, null);
                return;
            }

            using var jTask = m_jSnapClient.JResolveConflict(m_conflict.GetConflictId(), casted.JSnapshot);
            jTask.JAddOnSuccessListener(jData => {
                Logger.t($"AND: Success {method}");
                m_retry();
            }).JAddOnFailureListener(jException => {
                Logger.t($"AND: Failure {method}");
                Logger.w($"AND: {jException.JToString()}");
                m_callback?.Invoke(m_saveClient.JPlayClient.GetSavedGameRequestStatus(), null);
            });
        }

        #region Object implementation

        public override string ToString() => $"ConflictResolver(callback: {m_callback}, conflict: {m_conflict}, original: {m_original}, retry: {m_retry}, saveClient: {m_saveClient}, snapClient: {m_jSnapClient}, unmerged: {m_unmerged})";

        public override int GetHashCode() => HashCode.Combine(m_callback, m_conflict, m_original, m_retry, m_saveClient, m_jSnapClient, m_unmerged);

        public override bool Equals(object other)
        {
            if (other is not ACR it) return false;
            return m_callback   .Equals(it.m_callback)    &&
                   m_conflict   .Equals(it.m_conflict)    &&
                   m_original   .Equals(it.m_original)    &&
                   m_retry      .Equals(it.m_retry)       &&
                   m_saveClient .Equals(it.m_saveClient)  &&
                   m_jSnapClient.Equals(it.m_jSnapClient) &&
                   m_unmerged   .Equals(it.m_unmerged);
        }

        #endregion Object implementation

    }

}

#endif