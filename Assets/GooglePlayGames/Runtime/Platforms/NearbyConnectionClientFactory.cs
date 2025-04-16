// <copyright file="NearbyConnectionClientFactory.cs" company="Google Inc.">
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

using UnityEngine;

using Logger = GooglePlayGames.Utils.Logger;

using ADNCC = GooglePlayGames.Api.Nearby.DummyNearbyConnectionClient;
using AINCC = GooglePlayGames.Api.Nearby.INearbyConnectionClient;

#if UNITY_ANDROID
using ANCC = GooglePlayGames.Android.NearbyConnectionClient;
#endif

namespace GooglePlayGames {

    public static class NearbyConnectionClientFactory {

        public static void Create(Action<AINCC> callback)
        {
            if (Application.isEditor) {
                Logger.d("Creating INearbyConnection in editor, using DummyClient.");
                callback?.Invoke(new ADNCC());
            }
#if UNITY_ANDROID
            Logger.d("Creating Android INearbyConnection Client");
            callback?.Invoke(new ANCC());
#else
            Logger.d("Cannot create INearbyConnection for unknown platform, returning DummyClient");
            callback?.Invoke(new ADNCC());
#endif
        }

    }

}