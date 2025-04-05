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

namespace GooglePlayGames.BasicApi.SavedGame {

    public enum ConflictResolutionStrategy {
        UseLastKnownGood     = 4,
        UseLongestPlaytime   = 0,
        UseManual            = 3,
        UseMostRecentlySaved = 5,
        UseOriginal          = 1,
        UseUnmerged          = 2,
    }

    public enum SavedGameRequestStatus {
        Success             =  1,
        TimeoutError        = -1,
        InternalError       = -2,
        AuthenticationError = -3,
        BadInputError       = -4,
    }

    public enum SelectUIStatus {
        SavedGameSelected   =  1,
        UserClosedUI        =  2,
        InternalError       = -1,
        TimeoutError        = -2,
        AuthenticationError = -3,
        BadInputError       = -4,
        UiBusy              = -5,
    }

    public delegate void ConflictCallback(IConflictResolver resolver, ISavedGameMetadata original, byte[] originalData, ISavedGameMetadata unmerged, byte[] unmergedData);

    public interface ISavedGameClient {

        void CommitUpdate(ISavedGameMetadata metadata, SavedGameMetadataUpdate updateForMetadata, byte[] updatedBinaryData, Action<SavedGameRequestStatus, ISavedGameMetadata> callback);

        void Delete(ISavedGameMetadata metadata);

        void FetchAllSavedGames(DataSource source, Action<SavedGameRequestStatus, List<ISavedGameMetadata>> callback);

        void OpenWithAutomaticConflictResolution(string filename, DataSource source, ConflictResolutionStrategy resolutionStrategy, Action<SavedGameRequestStatus, ISavedGameMetadata> callback);

        void OpenWithManualConflictResolution(string filename, DataSource source, bool prefetchDataOnConflict, ConflictCallback conflictCallback, Action<SavedGameRequestStatus, ISavedGameMetadata> completedCallback);

        void ReadBinaryData(ISavedGameMetadata metadata, Action<SavedGameRequestStatus, byte[]> completedCallback);

        void ShowSelectSavedGameUI(string uiTitle, uint maxDisplayedSavedGames, bool showCreateSaveUI, bool showDeleteSaveUI, Action<SelectUIStatus, ISavedGameMetadata> callback);

    }

    public interface IConflictResolver {

        void ChooseMetadata(ISavedGameMetadata chosenMetadata);

        void ResolveConflict(ISavedGameMetadata chosenMetadata, SavedGameMetadataUpdate metadataUpdate, byte[] updatedData);

    }

}