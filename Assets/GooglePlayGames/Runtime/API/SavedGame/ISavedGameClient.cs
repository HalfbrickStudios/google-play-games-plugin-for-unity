// <copyright file="ISavedGameClient.cs" company="Google Inc.">
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
using System.Collections.Generic;

using ACRS  = GooglePlayGames.Api.SavedGame.ConflictResolutionStrategy;
using ADS   = GooglePlayGames.Api.DataSource;
using AICR  = GooglePlayGames.Api.SavedGame.IConflictResolver;
using AISGM = GooglePlayGames.Api.SavedGame.ISavedGameMetadata;
using ASGMU = GooglePlayGames.Api.SavedGame.SavedGameMetadataUpdate;
using ASGRS = GooglePlayGames.Api.SavedGame.SavedGameRequestStatus;
using ASUS  = GooglePlayGames.Api.SavedGame.SelectUiStatus;

namespace GooglePlayGames.Api.SavedGame {

    public delegate void ConflictCallback(AICR resolver, AISGM original, byte[] originalData, AISGM unmerged, byte[] unmergedData);

    public enum ConflictResolutionStrategy {
        UseLastKnownGood     = 4,
        UseLongestPlaytime   = 0,
        UseManual            = 3,
        UseMostRecentlySaved = 5,
        UseOriginal          = 1,
        UseUnmerged          = 2,
    }

    public interface IConflictResolver {

        void ChooseMetadata(AISGM metadata);

        void ResolveConflict(AISGM metadata, ASGMU update, byte[] data);

    }

    public interface ISavedGameClient {

        void CommitUpdate(AISGM metadata, ASGMU update, byte[] data, Action<ASGRS, AISGM> callback);

        void Delete(AISGM metadata);

        void FetchAllSavedGames(ADS source, Action<ASGRS, List<AISGM>> callback);

        void OpenWithAutomaticConflictResolution(string filename, ADS source, ACRS strategy, Action<ASGRS, AISGM> callback);

        void OpenWithManualConflictResolution(string filename, ADS source, bool prefetchData, ConflictCallback onConflict, Action<ASGRS, AISGM> onCompleted);

        void ReadBinaryData(AISGM metadata, Action<ASGRS, byte[]> callback);

        void ShowSelectSavedGameUI(string title, uint entries, bool showCreate, bool showDelete, Action<ASUS, AISGM> callback);

    }

    public enum SavedGameRequestStatus {
        Success             =  1,
        TimeoutError        = -1,
        InternalError       = -2,
        AuthenticationError = -3,
        BadInputError       = -4,
    }

    public enum SelectUiStatus {
        SavedGameSelected   =  1,
        UserClosedUI        =  2,
        InternalError       = -1,
        TimeoutError        = -2,
        AuthenticationError = -3,
        BadInputError       = -4,
        UiBusy              = -5,
    }

}