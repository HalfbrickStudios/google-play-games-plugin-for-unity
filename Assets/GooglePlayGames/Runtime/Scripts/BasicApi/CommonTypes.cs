// <copyright file="CommonTypes.cs" company="Google Inc.">
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

namespace GooglePlayGames.BasicApi {

    public enum DataSource {
        ReadCacheOrNetwork = 0,
        ReadNetworkOnly    = 1,
    }

    public enum ResponseStatus {
        InternalError         = -2,
        LicenseCheckFailed    = -1,
        NotAuthorized         = -3,
        ResolutionRequired    = -6,
        Success               =  1,
        SuccessWithStale      =  2,
        Timeout               = -5,
        VersionUpdateRequired = -4,
    }

    public enum UIStatus {
        InternalError         =  -2,
        NetworkError          = -20,
        NotAuthorized         =  -3,
        Timeout               =  -5,
        UiBusy                = -12,
        UserClosedUI          =  -6,
        Valid                 =   1,
        VersionUpdateRequired =  -4,
    }

    public enum LeaderboardStart {
        PlayerCentered = 2,
        TopScores      = 1,
    }

    public enum LeaderboardTimeSpan {
        AllTime = 3,
        Daily   = 1,
        Weekly  = 2,
    }

    public enum LeaderboardCollection {
        Public = 1,
        Social = 2,
    }

    public enum FriendsListVisibilityStatus {
        NetworkError       = -4,
        NotAuthorized      = -5,
        ResolutionRequired =  2,
        Unavailable        =  3,
        Unknown            =  0,
        Visible            =  1,
    }

    public enum LoadFriendsStatus {
        Completed          =  1,
        LoadMore           =  2,
        InternalError      = -4,
        NetworkError       = -6,
        NotAuthorized      = -5,
        ResolutionRequired = -3,
        Unknown            =  0,
    }

    public static class CommonTypesUtil {

        public static bool StatusIsSuccess(ResponseStatus status) => ((int) status) > 0;

    }

}