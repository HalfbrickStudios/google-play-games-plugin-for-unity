// <copyright file="SavedGameMetadataUpdate.cs" company="Google Inc.">
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
using System.Linq;

using GooglePlayGames.Utils;

using ASGMU  = GooglePlayGames.Api.SavedGame.SavedGameMetadataUpdate;
using ASGMUB = GooglePlayGames.Api.SavedGame.SavedGameMetadataUpdate.Builder;

namespace GooglePlayGames.Api.SavedGame {

    public readonly struct SavedGameMetadataUpdate {

        private SavedGameMetadataUpdate(ASGMUB builder)
        {
            IsCoverImageUpdated  = builder.IsCoverImageUpdated;
            IsDescriptionUpdated = builder.IsDescriptionUpdated;
            UpdatedDescription   = builder.UpdatedDescription;
            UpdatedPlayedTime    = builder.UpdatedPlayedTime;
            UpdatedPngCoverImage = builder.UpdatedPngCoverImage;
        }

        public bool      IsCoverImageUpdated  { get; }
        public bool      IsDescriptionUpdated { get; }
        public string    UpdatedDescription   { get; }
        public TimeSpan? UpdatedPlayedTime    { get; }
        public byte[]    UpdatedPngCoverImage { get; }

        public bool IsPlayedTimeUpdated => UpdatedPlayedTime.HasValue;

        public struct Builder {

            internal bool      IsCoverImageUpdated  { get; private set; }
            internal bool      IsDescriptionUpdated { get; private set; }
            internal string    UpdatedDescription   { get; private set; }
            internal TimeSpan? UpdatedPlayedTime    { get; private set; }
            internal byte[]    UpdatedPngCoverImage { get; private set; }

            public ASGMUB WithUpdatedDescription(string value)
            {
                IsDescriptionUpdated = true;
                UpdatedDescription   = Misc.CheckNotNull(value);
                return this;
            }

            public ASGMUB WithUpdatedPngCoverImage(byte[] value)
            {
                IsCoverImageUpdated  = true;
                UpdatedPngCoverImage = value;
                return this;
            }

            public ASGMUB WithUpdatedPlayedTime(TimeSpan value)
            {
                if (value.TotalMilliseconds > ulong.MaxValue) {
                    throw new InvalidOperationException("Timespans longer than ulong.MaxValue milliseconds are not allowed");
                }
                UpdatedPlayedTime = value;
                return this;
            }

            public readonly ASGMU Build() => new(this);

            #region Object implementation

            public override readonly string ToString() => $"SavedGameMetadataUpdate.Builder(IsCoverImageUpdated: {IsCoverImageUpdated}, IsDescriptionUpdated: {IsDescriptionUpdated}, UpdatedDescription: {UpdatedDescription}, UpdatedPlayedTime: {UpdatedPlayedTime}, UpdatedPngCoverImage: bytes[{UpdatedPngCoverImage.Length}])";

            public override readonly int GetHashCode() => HashCode.Combine(IsCoverImageUpdated, IsDescriptionUpdated, UpdatedDescription, UpdatedPlayedTime, UpdatedPngCoverImage);

            public override readonly bool Equals(object other)
            {
                if (other is not ASGMUB it) return false;
                return IsCoverImageUpdated          ==     it.IsCoverImageUpdated  &&
                       IsDescriptionUpdated         ==     it.IsDescriptionUpdated &&
                       UpdatedDescription          .Equals(it.UpdatedDescription)  &&
                       UpdatedPlayedTime           .Equals(it.UpdatedPlayedTime)   &&
                       UpdatedPngCoverImage.SequenceEqual (it.UpdatedPngCoverImage);
            }

            #endregion Object implementation

        }

        #region Object implementation

        public override string ToString() => $"SavedGameMetadataUpdate(IsCoverImageUpdated: {IsCoverImageUpdated}, IsDescriptionUpdated: {IsDescriptionUpdated}, UpdatedDescription: {UpdatedDescription}, UpdatedPlayedTime: {UpdatedPlayedTime}, UpdatedPngCoverImage: bytes[{UpdatedPngCoverImage.Length}])";

        public override int GetHashCode() => HashCode.Combine(IsCoverImageUpdated, IsDescriptionUpdated, UpdatedDescription, UpdatedPlayedTime, UpdatedPngCoverImage);

        public override bool Equals(object other)
        {
            if (other is not ASGMU it) return false;
            return IsCoverImageUpdated          ==     it.IsCoverImageUpdated  &&
                   IsDescriptionUpdated         ==     it.IsDescriptionUpdated &&
                   UpdatedDescription          .Equals(it.UpdatedDescription)  &&
                   UpdatedPlayedTime           .Equals(it.UpdatedPlayedTime)   &&
                   UpdatedPngCoverImage.SequenceEqual (it.UpdatedPngCoverImage);
        }

        #endregion Object implementation

    }

}