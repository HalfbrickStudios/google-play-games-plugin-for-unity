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

using GooglePlayGames.OurUtils;

namespace GooglePlayGames.BasicApi.SavedGame {

    public readonly struct SavedGameMetadataUpdate {

        private SavedGameMetadataUpdate(Builder builder)
        {
            IsDescriptionUpdated = builder.IsDescriptionUpdated;
            UpdatedDescription   = builder.UpdatedDescription;
            IsCoverImageUpdated  = builder.IsCoverImageUpdated;
            UpdatedPngCoverImage = builder.UpdatedPngCoverImage;
            UpdatedPlayedTime    = builder.UpdatedPlayedTime;
        }

        public bool      IsCoverImageUpdated  { get; }
        public bool      IsDescriptionUpdated { get; }
        public bool      IsPlayedTimeUpdated  => UpdatedPlayedTime.HasValue;
        public string    UpdatedDescription   { get; }
        public TimeSpan? UpdatedPlayedTime    { get; }
        public byte[]    UpdatedPngCoverImage { get; }

        public struct Builder {

            internal bool      IsCoverImageUpdated  { get; private set; }
            internal bool      IsDescriptionUpdated { get; private set; }
            internal string    UpdatedDescription   { get; private set; }
            internal TimeSpan? UpdatedPlayedTime    { get; private set; }
            internal byte[]    UpdatedPngCoverImage { get; private set; }

            public Builder WithUpdatedDescription(string description)
            {
                UpdatedDescription = Misc.CheckNotNull(description);
                IsDescriptionUpdated = true;
                return this;
            }

            public Builder WithUpdatedPngCoverImage(byte[] newPngCoverImage)
            {
                IsCoverImageUpdated = true;
                UpdatedPngCoverImage = newPngCoverImage;
                return this;
            }

            public Builder WithUpdatedPlayedTime(TimeSpan newPlayedTime)
            {
                if (newPlayedTime.TotalMilliseconds > ulong.MaxValue) {
                    var reason = "Timespans longer than ulong.MaxValue milliseconds are not allowed";
                    throw new InvalidOperationException(reason);
                }
                UpdatedPlayedTime = newPlayedTime;
                return this;
            }

            public readonly SavedGameMetadataUpdate Build() => new(this);

        }

    }

}