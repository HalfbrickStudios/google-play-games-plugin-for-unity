#if UNITY_ANDROID

using System;
using System.Collections.Generic;

using US = UnityEngine.SocialPlatforms.IScore;

using GPGS = GooglePlayGames.PlayGamesScore;

using AA    = GooglePlayGames.Api.Achievement;
using ACRS  = GooglePlayGames.Api.SavedGame.ConflictResolutionStrategy;
using AE    = GooglePlayGames.Api.Events.Event;
using AEV   = GooglePlayGames.Api.Events.EventVisibility;
using AIE   = GooglePlayGames.Api.Events.IEvent;
using AFLVS = GooglePlayGames.Api.FriendsListVisibilityStatus;
using ALC   = GooglePlayGames.Api.LeaderboardCollection;
using ALSD  = GooglePlayGames.Api.LeaderboardScoreData;
using ALTS  = GooglePlayGames.Api.LeaderboardTimeSpan;
using AP    = GooglePlayGames.Api.Player;
using APP   = GooglePlayGames.Api.PlayerProfile;
using APS   = GooglePlayGames.Api.PlayerStats;
using ARS   = GooglePlayGames.Api.ResponseStatus;
using ASGMU = GooglePlayGames.Api.SavedGame.SavedGameMetadataUpdate;
using ASGRS = GooglePlayGames.Api.SavedGame.SavedGameRequestStatus;
using ASPC  = GooglePlayGames.Api.ScorePageCursor;
using ASPD  = GooglePlayGames.Api.ScorePageDirection;
using ASUS  = GooglePlayGames.Api.SavedGame.SelectUiStatus;

using JABI   = GooglePlayGames.Android.Java.AchievementBuffer.Instance;
using JAI    = GooglePlayGames.Android.Java.Achievement.Instance;
using JAL    = GooglePlayGames.Android.Java.ArrayList;
using JA     = GooglePlayGames.Android.Java.Achievement;
using JBF    = GooglePlayGames.Android.Java.BitmapFactory;
using JEBI   = GooglePlayGames.Android.Java.EventBuffer.Instance;
using JEI    = GooglePlayGames.Android.Java.Event.Instance;
using JLSBI  = GooglePlayGames.Android.Java.LeaderboardScoreBuffer.Instance;
using JLSI   = GooglePlayGames.Android.Java.LeaderboardScore.Instance;
using JLSsI  = GooglePlayGames.Android.Java.LeaderboardsClient.LeaderboardScores.Instance;
using JLV    = GooglePlayGames.Android.Java.LeaderboardVariant;
using JLVI   = GooglePlayGames.Android.Java.LeaderboardVariant.Instance;
using JPFLVS = GooglePlayGames.Android.Java.Player.FriendsListVisibilityStatus;
using JPBI   = GooglePlayGames.Android.Java.PlayerBuffer.Instance;
using JPD    = GooglePlayGames.Android.Java.PageDirection;
using JPFS   = GooglePlayGames.Android.Java.Player.PlayerFriendStatus;
using JPI    = GooglePlayGames.Android.Java.Player.Instance;
using JPSI   = GooglePlayGames.Android.Java.PlayerStats.Instance;
using JSC    = GooglePlayGames.Android.Java.SnapshotsClient;
using JSMC   = GooglePlayGames.Android.Java.SnapshotMetadataChange;
using JSMCI  = GooglePlayGames.Android.Java.SnapshotMetadataChange.Instance;

namespace GooglePlayGames.Utils {

    internal static partial class Utility {

        private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, 0);

        public static AA ToAndroidAchievement(JAI jAchievement)
        {
            var jState = jAchievement.JGetState();
            var jType  = jAchievement.JGetType();
            var result = new AA {
                Description      =                                   jAchievement.GetDescription(),
                Id               =                                   jAchievement.GetAchievementId(),
                IsIncremental    = ToAndroidAchievementIsIncremental(jType),
                IsRevealed       = ToAndroidAchievementIsRevealed   (jState),
                IsUnlocked       = ToAndroidAchievementIsUnlocked   (jState),
                LastModifiedTime = ToAndroidDateTime                (jAchievement.GetLastUpdatedTimestamp()),
                Name             =                                   jAchievement.GetName(),
                Points           = ToAndroidAchievementPoints       (jAchievement.GetXpValue()),
                RevealedImageUrl =                                   jAchievement.GetRevealedImageUrl(),
                UnlockedImageUrl =                                   jAchievement.GetUnlockedImageUrl(),
            };
            if (result.IsIncremental) {
                result.CurrentSteps = jAchievement.GetCurrentSteps();
                result.TotalSteps   = jAchievement.GetTotalSteps();
            }
            return result;
        }

        public static IEnumerable<AA> ToAndroidAchievement(JABI jAchievements)
        {
            var count  = jAchievements.GetCount();
            var result = new AA[count];
            for (var i = 0; i < count; i += 1) {
                using var jAchievement = jAchievements.JGet(i);
                result[i] = ToAndroidAchievement(jAchievement);
            }
            return result;
        }

        public static bool ToAndroidAchievementIsIncremental(int jType) => jType == JA.TYPE_INCREMENTAL;

        public static bool ToAndroidAchievementIsRevealed(int jState) => jState == JA.STATE_REVEALED;

        public static bool ToAndroidAchievementIsUnlocked(int jState) => jState == JA.STATE_UNLOCKED;

        public static int ToAndroidAchievementPoints(ulong jXp) => (int)jXp;

        public static DateTime ToAndroidDateTime(long milliseconds) => UnixEpoch.AddMilliseconds(milliseconds);

        public static AIE ToAndroidEvent(JEI jEvent)
        {
            var id           = jEvent.GetEventId();
            var name         = jEvent.GetName();
            var description  = jEvent.GetDescription();
            var imageUrl     = jEvent.GetIconImageUrl();
            var currentCount = jEvent.GetValue();
            var visibility   = ToAndroidEventVisibility(jEvent.IsVisible());
            return new AE(id, name, description, imageUrl, (ulong)currentCount, visibility);
        }

        public static IEnumerable<AIE> ToAndroidEvent(JEBI jEvents)
        {
            var count = jEvents.GetCount();
            var result = new AIE[count];
            for (var i = 0; i < count; i += 1) {
                using var jEvent = jEvents.JGet(i);
                result[i] = ToAndroidEvent(jEvent);
            }
            return result;
        }

        public static AEV ToAndroidEventVisibility(bool jIsVisible) => jIsVisible ? AEV.Revealed : AEV.Hidden;

        public static AFLVS ToAndroidFriendsListVisibilityStatus(int jStatus)
        {
            if (jStatus == JPFLVS.UNKNOWN)             return AFLVS.Unknown;
            if (jStatus == JPFLVS.VISIBLE)             return AFLVS.Visible;
            if (jStatus == JPFLVS.REQUEST_REQUIRED)    return AFLVS.ResolutionRequired;
            if (jStatus == JPFLVS.FEATURE_UNAVAILABLE) return AFLVS.Unavailable;
            return AFLVS.Unknown;
        }

        public static ALC ToAndroidLeaderboardCollection(int jCollection)
        {
            if (jCollection == JLV.COLLECTION_PUBLIC)  return ALC.Public;
            if (jCollection == JLV.COLLECTION_FRIENDS) return ALC.Social;
            return ALC.Public;
        }

        public static ALSD ToAndroidLeaderboardScoreData(JLSsI jScores, string id, ARS? status = null, ALC? collection = null, ALTS? span = null)
        {
            var result = null as ALSD;
            using (var jBuffer = jScores.JGetScores()) {
                result = ToAndroidLeaderboardScoreData(jBuffer, id, status);
                if (collection != null && span != null) {
                    result.NextPageCursor     = new ASPC(jBuffer, id, (ALC)collection, (ALTS)span, ASPD.Forward);
                    result.PreviousPageCursor = new ASPC(jBuffer, id, (ALC)collection, (ALTS)span, ASPD.Backward);
                }
            }
            using (var jLeaderboard = jScores.JGetLeaderboard()) {
                result.Title = jLeaderboard.GetDisplayName();
            }
            return result;
        }

        public static ALSD ToAndroidLeaderboardScoreData(JLSBI jScores, string id, ARS? status = null)
        {
            var result = status == null ? new ALSD(id) : new ALSD(id, (ARS)status);
            var count  = jScores.GetCount();
            for (var i = 0; i < count; i += 1) {
                var score = null as GPGS;
                using (var jScore = jScores.JGet(i)) {
                    score = (GPGS)ToAndroidPlayerGameScore(jScore);
                }
                score.LeaderboardId = id;
                result.AddScore(score);
            }
            return result;
        }

        public static ALTS ToAndroidLeaderboardTimeSpan(int jSpan)
        {
            if (jSpan == JLV.TIME_SPAN_ALL_TIME) return ALTS.AllTime;
            if (jSpan == JLV.TIME_SPAN_DAILY)    return ALTS.Daily;
            if (jSpan == JLV.TIME_SPAN_WEEKLY)   return ALTS.Weekly;
            return ALTS.AllTime;
        }

        public static AP ToAndroidPlayer(JPI jPlayer)
        {
            if (jPlayer == null) return null;
            var displayName = jPlayer.GetDisplayName();
            var playerId    = jPlayer.GetPlayerId();
            var avatarUrl   = jPlayer.GetIconImageUrl();
            return new AP(displayName, playerId, avatarUrl);
        }

        public static US ToAndroidPlayerGameScore(JLSI jScore, string leaderboardId = null)
        {
            var date          = ToAndroidDateTime(jScore.GetTimestampMillis());
            var rank          =            (ulong)jScore.GetRank();
            var score         =            (ulong)jScore.GetRawScore();
            var metadata      =                   jScore.GetScoreTag();
            using var jPlayer =                   jScore.JGetScoreHolder();
            return new GPGS(date, leaderboardId ?? string.Empty, rank, jPlayer.GetPlayerId(), score, metadata);
        }

        public static US ToAndroidPlayerGameScore(JLVI jVariant, string leaderboardId = null, string playerId = null)
        {
            var date     = ToAndroidDateTime(0);
            var rank     = (ulong)jVariant.GetPlayerRank();
            var score    = (ulong)jVariant.GetRawPlayerScore();
            var metadata =        jVariant.GetPlayerScoreTag();
            return new GPGS(date, leaderboardId, rank, playerId, score, metadata);
        }

        public static APP ToAndroidPlayerProfile(JPI jplayer)
        {
            if (jplayer == null) return null;
            var displayName  = jplayer.GetDisplayName();
            var playerId     = jplayer.GetPlayerId();
            var avatarUrl    = jplayer.GetIconImageUrl();
            var relationship = jplayer.JGetRelationshipInfo();
            var isFriend     = relationship.JGetFriendStatus() == JPFS.FRIEND;
            return new APP(displayName, playerId, avatarUrl, isFriend);
        }

        public static IEnumerable<APP> ToAndroidPlayerProfile(JPBI jPlayers)
        {
            var count = jPlayers.GetCount();
            var users = new APP[count];
            for (var i = 0; i < count; i += 1) {
                using var player = jPlayers.JGet(i);
                users[i] = ToAndroidPlayerProfile(player);
            }
            return users;
        }

        public static APS ToAndroidPlayerStats(JPSI jStats)
        {
            var purchases              = jStats.GetNumberOfPurchases();
            var sessionLength          = jStats.GetAverageSessionLength();
            var lastPlayedGap          = jStats.GetDaysSinceLastPlayed();
            var sessions               = jStats.GetNumberOfSessions();
            var sessionPercentile      = jStats.GetSessionPercentile();
            var spendPercentile        = jStats.GetSpendPercentile();
            var spendProbability       = jStats.GetSpendProbability();
            var churnProbability       = jStats.GetChurnProbability();
            var highSpenderProbability = jStats.GetHighSpenderProbability();
            var totalSpendNext28Days   = jStats.GetTotalSpendNext28Days();
            return new APS(purchases, sessionLength, lastPlayedGap, sessions, sessionPercentile, spendPercentile, spendProbability, churnProbability, highSpenderProbability, totalSpendNext28Days);
        }

        public static ARS ToAndroidResponseStatus(bool jIsStale) => jIsStale ? ARS.SuccessWithStale : ARS.Success;

        public static ASGRS ToAndroidSavedGameRequestStatus(bool jIsAuthenticated) => jIsAuthenticated ? ASGRS.InternalError : ASGRS.AuthenticationError;

        public static ASUS ToAndroidSelectUiStatus(int jStatus) => (ASUS)jStatus; // No Gods or Kings. Only Crash.

        public static int ToJavaLeaderboardVariantCollection(ALC collection) => collection switch
        {
            ALC.Public => JLV.COLLECTION_PUBLIC,
            ALC.Social => JLV.COLLECTION_FRIENDS,
            _          => JLV.COLLECTION_PUBLIC,
        };

        public static int ToJavaLeaderboardVariantTimeSpan(ALTS span) => span switch
        {
            ALTS.AllTime => JLV.TIME_SPAN_ALL_TIME,
            ALTS.Daily   => JLV.TIME_SPAN_DAILY,
            ALTS.Weekly  => JLV.TIME_SPAN_WEEKLY,
            _            => JLV.TIME_SPAN_ALL_TIME,
        };

        public static int ToJavaPageDirection(ASPD direction) => direction switch
        {
            ASPD.Backward => JPD.PREV,
            ASPD.Forward  => JPD.NEXT,
            _             => JPD.NONE,
        };

        public static int ToJavaResolutionPolicy(ACRS policy)
        {
            if (policy == ACRS.UseLastKnownGood)     return JSC.RESOLUTION_POLICY_LAST_KNOWN_GOOD;
            if (policy == ACRS.UseMostRecentlySaved) return JSC.RESOLUTION_POLICY_MOST_RECENTLY_MODIFIED;
            if (policy == ACRS.UseLongestPlaytime)   return JSC.RESOLUTION_POLICY_LONGEST_PLAYTIME;
            if (policy == ACRS.UseManual)            return JSC.RESOLUTION_POLICY_MANUAL;
            return JSC.RESOLUTION_POLICY_MOST_RECENTLY_MODIFIED;
        }

        public static JSMCI ToJavaSnapshotMetadataChangeUpdate(ASGMU update)
        {
            var jBuilder = JSMC.MakeBuilder();
            if (update.IsCoverImageUpdated) {
                using var jBitmap = JBF.JDecodeByteArray(update.UpdatedPngCoverImage, offset: 0, update.UpdatedPngCoverImage.Length);
                using (jBuilder.JSetCoverImage(jBitmap)) {
                    // No-op
                }
            }
            if (update.IsDescriptionUpdated) {
                using (jBuilder.JSetDescription(update.UpdatedDescription)) {
                    // No-op
                }
            }
            if (update.IsPlayedTimeUpdated) {
                var time = ToInt64(update.UpdatedPlayedTime.Value.TotalMilliseconds);
                using (jBuilder.JSetPlayedTimeMillis(time)) {
                    // No-op
                }
            }
            return jBuilder.JBuild();
        }

        public static JAL.Instance<string> ToJavaStringList(IEnumerable<string> list)
        {
            var converted = JAL.MakeInstance<string>();
            foreach (var item in list) {
                converted.Add(item);
            }
            return converted;
        }

        // TODO: narrow the type to not use Call<T>
        // internal static List<string> ToAndroidStringList(UAJO jstrings)
        // {
        //     if (jstrings == null) new List<string>();
        //     var size      = jstrings.Call<int>("size");
        //     var converted = new List<string>(size);
        //     for (var i = 0; i < size; i += 1) {
        //         var item = jstrings.Call<string>("get", i);
        //         converted.Add(item);
        //     }
        //     return converted;
        // }

    }

}

#endif