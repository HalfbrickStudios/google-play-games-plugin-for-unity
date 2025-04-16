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

using GooglePlayGames.Utils;

namespace GooglePlayGames.Api.SavedGame {

    public readonly struct SavedGameMetadataUpdate {

        private SavedGameMetadataUpdate(Builder builder)
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

            public Builder WithUpdatedDescription(string description)
            {
                IsDescriptionUpdated = true;
                UpdatedDescription   = Misc.CheckNotNull(description);
                return this;
            }

            public Builder WithUpdatedPngCoverImage(byte[] newPngCoverImage)
            {
                IsCoverImageUpdated  = true;
                UpdatedPngCoverImage = newPngCoverImage;
                return this;
            }

            public Builder WithUpdatedPlayedTime(TimeSpan newPlayedTime)
            {
                if (newPlayedTime.TotalMilliseconds > ulong.MaxValue) {
                    throw new InvalidOperationException("Timespans longer than ulong.MaxValue milliseconds are not allowed");
                }
                UpdatedPlayedTime = newPlayedTime;
                return this;
            }

            public readonly SavedGameMetadataUpdate Build() => new(this);

        }

    }

}