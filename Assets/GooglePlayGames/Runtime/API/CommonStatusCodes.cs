// <copyright file="CommonStatusCodes.cs" company="Google Inc.">
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

#if UNITY_ANDROID

namespace GooglePlayGames.BasicApi {

    public enum CommonStatusCodes {
        ApiNotConnected              =   17,
        AuthApiAccessForbidden       = 3001,
        AuthApiClientError           = 3002,
        AuthApiInvalidCredentials    = 3000,
        AuthApiServerError           = 3003,
        AuthTokenError               = 3004,
        AuthUrlResolution            = 3005,
        Canceled                     =   16,
        DeveloperError               =   10,
        Error                        =   13,
        InternalError                =    8,
        Interrupted                  =   14,
        InvalidAccount               =    5,
        LicenseCheckFailed           =   11,
        NetworkError                 =    7,
        ResolutionRequired           =    6,
        ServiceDisabled              =    3,
        ServiceInvalid               =    9,
        ServiceMissing               =    1,
        ServiceVersionUpdateRequired =    2,
        SignInRequired               =    4,
        Success                      =    0,
        SuccessCached                =   -1,
        Timeout                      =   15,
    }

}

#endif