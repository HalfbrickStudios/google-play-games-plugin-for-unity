// <copyright file="ISavedGameMetadata.cs" company="Google Inc.">
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

namespace GooglePlayGames.BasicApi.SavedGame {

    public interface ISavedGameMetadata {
        string   CoverImageUrl         { get; }
        string   Description           { get; }
        string   Filename              { get; }
        bool     IsOpen                { get; }
        DateTime LastModifiedDateTime  { get; }
        TimeSpan PlayedTime            { get; }

        #region Backward compatibility layer

        [Obsolete("Use CoverImageUrl instead")]
        string CoverImageURL => CoverImageUrl;
        
        [Obsolete("Use LastModifiedDateTime instead")]
        DateTime LastModifiedTimestamp => LastModifiedDateTime;

        [Obsolete("Use PlayedTime instead")]
        TimeSpan TotalTimePlayed => PlayedTime;

        #endregion Backward compatibility layer

    }

}