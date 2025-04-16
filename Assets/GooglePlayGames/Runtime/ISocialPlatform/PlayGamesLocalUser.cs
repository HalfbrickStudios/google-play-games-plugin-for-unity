// <copyright file="PlayGamesLocalUser.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

using System;

using UILU = UnityEngine.SocialPlatforms.ILocalUser;
using UIUP = UnityEngine.SocialPlatforms.IUserProfile;
using UUS  = UnityEngine.SocialPlatforms.UserState;

using GPGP  = GooglePlayGames.PlayGamesPlatform;
using GPGUP = GooglePlayGames.PlayGamesUserProfile;

using ACSC = GooglePlayGames.Api.CommonStatusCodes;
using APS  = GooglePlayGames.Api.PlayerStats;
using ASIS = GooglePlayGames.Api.SignInStatus;

namespace GooglePlayGames {

    public sealed class PlayGamesLocalUser : GPGUP, UILU {

        private readonly GPGP m_platform;
        private          APS  m_stats;

        internal PlayGamesLocalUser(GPGP platform) : base(userName: "localUser", userId: string.Empty, avatar: string.Empty)
        {
            m_platform = platform;
            m_stats    = null;
        }

        public     UIUP[] Friends         => m_platform.GetFriends();
        public     bool   IsAuthenticated => m_platform.IsAuthenticated();
        public new bool   IsFriend        => true;
        public     bool   IsUnderage      => true;
        public new UUS    State           => UUS.Online;

        public new string AvatarUrl
        {
            get {
                var result = string.Empty;
                if (IsAuthenticated) {
                    result = m_platform.GetUserImageUrl();
                    if (!base.Id.Equals(result)) {
                        ResetIdentity(m_platform.GetUserDisplayName(), m_platform.GetUserId(), result);
                    }
                }
                return result;
            }
        }

        public new string Id {
            get {
                var result = string.Empty;
                if (IsAuthenticated) {
                    result = m_platform.GetUserId();
                    if (!base.Id.Equals(result)) {
                        ResetIdentity(m_platform.GetUserDisplayName(), result, m_platform.GetUserImageUrl());
                    }
                }
                return result;
            }
        }

        public new string UserName
        {
            get {
                var result = string.Empty;
                if (IsAuthenticated) {
                    result = m_platform.GetUserDisplayName();
                    if (!base.UserName.Equals(result)) {
                        ResetIdentity(result, m_platform.GetUserId(), m_platform.GetUserImageUrl());
                    }
                }
                return result;
            }
        }

        public void GetStats(Action<ACSC, APS> callback)
        {
            if (m_stats == null || !m_stats.IsValid) {
                m_platform.GetPlayerStats((rc, stats) => callback?.Invoke(rc, m_stats = stats));
            } else {
                callback?.Invoke(ACSC.Success, m_stats);
            }
        }

        #region Backward compatibility layer

        [Obsolete("Use AvatarUrl instead")]
        public new string AvatarURL => AvatarUrl;

        [Obsolete("Use IsFriend instead")]
        public new bool Friend => IsFriend;

        #endregion Backward compatibility layer

        #region IUserProfile implementation

        [Obsolete("Use Id instead")]       public new string id       => Id;
        [Obsolete("Use IsFriend instead")] public new bool   isFriend => IsFriend;
        [Obsolete("Use State instead")]    public new UUS    state    => State;
        [Obsolete("Use UserName instead")] public new string userName => UserName;

        #endregion IUserProfile implementation

        #region ILocalUser implementation

        [Obsolete("Use Friends         instead")] public UIUP[] friends       => Friends;
        [Obsolete("Use IsAuthenticated instead")] public bool   authenticated => IsAuthenticated;
        [Obsolete("Use IsUnderage      instead")] public bool   underage      => IsUnderage;

        public void Authenticate(Action<bool>         callback) => m_platform.Authenticate(it => callback?.Invoke(it == ASIS.Success));
        public void Authenticate(Action<bool, string> callback) => m_platform.Authenticate(it => callback?.Invoke(it == ASIS.Success, it.ToString()));
        public void LoadFriends (Action<bool>         callback) => m_platform.LoadFriends(this, callback);

        #endregion ILocalUser implementation

    }

}