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

using System;
using System.Collections;

using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

using GPGUP = GooglePlayGames.PlayGamesUserProfile;

using UDHT = UnityEngine.Networking.DownloadHandlerTexture;
using UIUP = UnityEngine.SocialPlatforms.IUserProfile;
using UT2D = UnityEngine.Texture2D;
using UUS  = UnityEngine.SocialPlatforms.UserState;
using UWRT = UnityEngine.Networking.UnityWebRequestTexture;

namespace GooglePlayGames {

    public class PlayGamesUserProfile : UIUP {

        private          UT2D m_image   = null;
        private volatile bool m_loading = false;

        internal PlayGamesUserProfile(string userName, string userId, string avatar)
        {
            m_loading = false;
            
            SetAvatarUrl(avatar);
            Id       = userId;
            IsFriend = false;
            UserName = userName;
        }

        internal PlayGamesUserProfile(string userName, string userId, string avatar, bool friend)
        {
            m_loading = false;
            
            AvatarUrl = avatar;
            Id        = userId;
            IsFriend  = friend;
            UserName  = userName;
        }

        public string AvatarUrl { get; private set; }
        public string Id        { get; private set; }
        public bool   IsFriend  { get; private set; }
        public string UserName  { get; private set; }
        
        public UT2D Image
        {
            get {
                if (!m_loading && m_image == null && !string.IsNullOrEmpty(AvatarUrl)) {
                    Logger.d("Starting to load image: " + AvatarUrl);
                    m_loading = true;
                    PlayGamesHelperObject.RunOnUiThread(LoadImage());
                }
                return m_image;
            }
            private set => m_image = value;
        }

        public string GameId => Id;
        public UUS    State  => UUS.Online;

        internal IEnumerator LoadImage()
        {
            if (!string.IsNullOrEmpty(AvatarUrl)) {
                var www = UWRT.GetTexture(AvatarUrl);
                www.SendWebRequest();
                while (!www.isDone) yield return null;
                if (www.error == null) {
                    Image = UDHT.GetContent(www);
                } else {
                    Image = UT2D.blackTexture;
                    Logger.e("Error downloading image: " + www.error);
                }
                m_loading = false;
            } else {
                Logger.e("No URL found.");
                Image = UT2D.blackTexture;
                m_loading = false;
            }
        }

        protected void ResetIdentity(string userName, string userId, string avatar)
        {
            m_loading = false;
            
            if (AvatarUrl != avatar) {
                Image = null;
                SetAvatarUrl(avatar);
            }
            Id       = userId;
            IsFriend = false;
            UserName = userName;
        }

        private void SetAvatarUrl(string value)
        {
            AvatarUrl = value;
            if (!value.StartsWith("https") && value.StartsWith("http")) {
                AvatarUrl = value.Insert(4, "s");
            }
        }

        #region Backward compatibility layer

        [Obsolete("Use IsFriend instead")]
        public string AvatarURL { get => AvatarUrl; private set => AvatarUrl = value; }

        [Obsolete("Use IsFriend instead")]
        public bool Friend { get => IsFriend; private set => IsFriend = value; }

        [Obsolete("Use Id instead")]
        public string gameId => Id;

        #endregion Backward compatibility layer

        #region IUserProfile implementation

        [Obsolete("Use Id instead")]       public string id       => Id;
        [Obsolete("Use Image instead")]    public UT2D   image    => Image;
        [Obsolete("Use IsFriend instead")] public bool   isFriend => IsFriend;
        [Obsolete("Use State instead")]    public UUS    state    => State;
        [Obsolete("Use UserName instead")] public string userName => UserName;

        #endregion

        #region Object implementation

        public override string ToString() => $"PlayGamesUserProfile(AvatarUrl: {AvatarUrl}, Id: {Id}, image: {m_image}, IsFriend: {IsFriend}, loading: {m_loading}, State: {State})";

        public override int GetHashCode() => HashCode.Combine(AvatarUrl, Id, m_image, IsFriend, m_loading, UserName);

        public override bool Equals(object other)
        {
            if (other is not GPGUP it) return false;
            return Utility.Equals(AvatarUrl, it.AvatarUrl) &&
                   Utility.Equals(Id,        it.Id)        &&
                   Utility.Equals(m_image,   it.m_image)   &&
                   Utility.Equals(IsFriend,  it.IsFriend)  &&
                   Utility.Equals(m_loading, it.m_loading) &&
                   Utility.Equals(UserName,  it.UserName);
        }

        #endregion Object implementation

    }

}