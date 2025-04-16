// <copyright file="AndroidTokenClient.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc.
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
//  limitations under the License.
// </copyright>

using System;

using ACC   = GooglePlayGames.Api.SavedGame.ConflictCallback;
using AICR  = GooglePlayGames.Api.SavedGame.IConflictResolver;
using AISGM = GooglePlayGames.Api.SavedGame.ISavedGameMetadata;

namespace GooglePlayGames.Utils {

    internal static partial class Utility {

        internal static void RunUiAction(Action callback)
        {
            if (callback == null) return;
            PlayGamesHelperObject.RunOnUiThread(() => callback.Invoke());
        }

        internal static void RunUiAction<T1>(Action<T1> callback, T1 arg1)
        {
            if (callback == null) return;
            PlayGamesHelperObject.RunOnUiThread(() => callback.Invoke(arg1));
        }

        internal static void RunUiAction<T1, T2>(Action<T1, T2> callback, T1 arg1, T2 arg2)
        {
            if (callback == null) return;
            PlayGamesHelperObject.RunOnUiThread(() => callback.Invoke(arg1, arg2));
        }

        internal static void RunUiAction<T1, T2, T3>(Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            if (callback == null) return;
            PlayGamesHelperObject.RunOnUiThread(() => callback.Invoke(arg1, arg2, arg3));
        }

        internal static void RunUiDelegate(ACC callback, AICR arg1, AISGM arg2, byte[] arg3, AISGM arg4, byte[] arg5)
        {
            if (callback == null) return;
            PlayGamesHelperObject.RunOnUiThread(() => callback.Invoke(arg1, arg2, arg3, arg4, arg5));
        }

        public static Action<T1> ToUiAction<T1>(Action<T1> callback)
        {
            return (arg1) => RunUiAction(callback, arg1);
        }

        public static Action<T1, T2> ToUiAction<T1, T2>(Action<T1, T2> callback)
        {
            return (arg1, arg2) => RunUiAction(callback, arg1, arg2);
        }

        internal static ACC ToUiDelegate(ACC callback)
        {
            return (arg1, arg2, arg3, arg4, arg5) => RunUiDelegate(callback, arg1, arg2, arg3, arg4, arg5);
        }

    }

}