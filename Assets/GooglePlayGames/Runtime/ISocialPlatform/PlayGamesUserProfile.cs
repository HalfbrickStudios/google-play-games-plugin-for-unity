// <copyright file="PlayGamesUserProfile.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.  All Rights Reserved.
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

#if UNITY_ANDROID

using System;
using System.Collections;

using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.Networking;
#endif
using UnityEngine.SocialPlatforms;

using GooglePlayGames.Utils;

namespace GooglePlayGames {

    public class PlayGamesUserProfile : IUserProfile {

        private          Texture2D m_image          = null;
        private volatile bool      m_imageIsLoading = false;

        internal PlayGamesUserProfile(string displayName, string playerId, string avatarUrl)
        {
            m_imageIsLoading = false;
            
            SetAvatarUrl(avatarUrl);
            Id = playerId;
            IsFriend = false;
            UserName = displayName;
        }

        internal PlayGamesUserProfile(string displayName, string playerId, string avatarUrl, bool isFriend)
        {
            m_imageIsLoading = false;
            
            AvatarURL = avatarUrl;
            Id        = playerId;
            IsFriend  = isFriend;
            UserName  = displayName;
        }

        public string AvatarURL { get; private set; }
        public string UserName  { get; private set; }
        public string Id        { get; private set; }
        public bool   IsFriend  { get; private set; }
        
        public Texture2D Image
        {
            get {
                if (!m_imageIsLoading && m_image == null && !string.IsNullOrEmpty(AvatarURL)) {
                    Utils.Logger.d("Starting to load image: " + AvatarURL);
                    m_imageIsLoading = true;
                    PlayGamesHelperObject.RunCoroutine(LoadImage());
                }
                return m_image;
            }
            private set => m_image = value;
        }

        public string    GameId => Id;
        public UserState State  => UserState.Online;

        internal IEnumerator LoadImage()
        {
            if (!string.IsNullOrEmpty(AvatarURL)) {
#if UNITY_2017_2_OR_NEWER
                var www = UnityWebRequestTexture.GetTexture(AvatarURL);
                www.SendWebRequest();
#else
                var www = new WWW(AvatarURL);
#endif
                while (!www.isDone) yield return null;
                if (www.error == null) {
#if UNITY_2017_2_OR_NEWER
                    Image = DownloadHandlerTexture.GetContent(www);
#else
                    Image = www.texture;
#endif
                } else {
                    Image = Texture2D.blackTexture;
                    Utils.Logger.e("Error downloading image: " + www.error);
                }
                m_imageIsLoading = false;
            } else {
                Utils.Logger.e("No URL found.");
                Image = Texture2D.blackTexture;
                m_imageIsLoading = false;
            }
        }

        protected void ResetIdentity(string displayName, string playerId, string avatarUrl)
        {
            m_imageIsLoading = false;
            
            if (AvatarURL != avatarUrl) {
                Image = null;
                SetAvatarUrl(avatarUrl);
            }
            Id             = playerId;
            IsFriend       = false;
            UserName       = displayName;
        }

        private void SetAvatarUrl(string avatarUrl)
        {
            AvatarURL = avatarUrl;
            if (!avatarUrl.StartsWith("https") && avatarUrl.StartsWith("http")) {
                AvatarURL = avatarUrl.Insert(4, "s");
            }
        }

        #region Backward compatibility layer

        [Obsolete("Use IsFriend instead")]
        public bool Friend { get => IsFriend; private set => IsFriend = value; }

        [Obsolete("Use Id instead")]
        public string gameId => Id;

        #endregion Backward compatibility layer

        #region IUserProfile implementation

        [Obsolete("Use Id instead")]       public string    id       => Id;
        [Obsolete("Use Image instead")]    public Texture2D image    => Image;
        [Obsolete("Use IsFriend instead")] public bool      isFriend => IsFriend;
        [Obsolete("Use State instead")]    public UserState state    => State;
        [Obsolete("Use UserName instead")] public string    userName => UserName;

        #endregion

        #region Object implementation

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not PlayGamesUserProfile other) return false;
            return StringComparer.Ordinal.Equals(Id, other.Id);
        }

        public override int GetHashCode() => typeof(PlayGamesUserProfile).GetHashCode() ^ Id.GetHashCode();

        public override string ToString() => $"[Player: '{UserName}' (id {Id})]";

        #endregion Object implementation

    }

}

#endif