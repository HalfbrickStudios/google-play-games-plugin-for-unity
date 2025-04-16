// <copyright file="PlayGamesHelperObject.cs" company="Google Inc.">
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
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UGO = UnityEngine.GameObject;
using UMO = UnityEngine.MonoBehaviour;

using GPGHO = GooglePlayGames.Utils.PlayGamesHelperObject;

namespace GooglePlayGames.Utils {

    public sealed class PlayGamesHelperObject : UMO {

        private static          bool               s_dummy          = false;
        private static volatile bool               s_empty          = true;
        private static readonly List<Action<bool>> s_focusCallbacks = new();
        private static          GPGHO              s_instance       = null;
        private static readonly List<Action<bool>> s_pauseCallbacks = new();
        private static readonly List<Action>       s_queue          = new();

        private readonly List<Action> m_queue = new();

        public static void AddFocusCallback(Action<bool> callback)
        {
            if (s_focusCallbacks.Contains(callback)) return;
            s_focusCallbacks.Add(callback);
        }

        public static void AddPauseCallback(Action<bool> callback)
        {
            if (s_pauseCallbacks.Contains(callback)) return;
            s_pauseCallbacks.Add(callback);
        }

        public static void CreateObject()
        {
            if (s_instance != null) return;
            if (Application.isPlaying) {
                var obj = new UGO("GpgQueueRunner");
                DontDestroyOnLoad(obj);
                s_instance = obj.AddComponent<GPGHO>();
            } else {
                s_instance = new();
                s_dummy    = true;
            }
        }

        public static bool RemoveFocusCallback(Action<bool> callback) => s_focusCallbacks.Remove(callback);

        public static bool RemovePauseCallback(Action<bool> callback) => s_pauseCallbacks.Remove(callback);

        public static void RunOnUiThread(Action runnable)
        {
            Misc.CheckNotNull(runnable, nameof(runnable));
            if (s_dummy) return;
            lock (s_queue) {
                s_queue.Add(runnable);
                s_empty = false;
            }
        }

        public static void RunOnUiThread(IEnumerator coroutine)
        {
            if (s_instance == null) return;
            RunOnUiThread(() => s_instance.StartCoroutine(coroutine));
        }

        #region MonoBehaviour implementation

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnDisable()
        {
            if (s_instance != this) return;
            s_instance = null;
        }

        private void Update()
        {
            if (s_dummy || s_empty) return;
            m_queue.Clear();
            lock (s_queue) {
                m_queue.AddRange(s_queue);
                s_queue.Clear();
                s_empty = true;
            }
            m_queue.ForEach(it => it?.Invoke());
        }

        private void OnApplicationFocus(bool focused)
        {
            foreach (var callback in s_focusCallbacks) {
                try {
                    callback?.Invoke(focused);
                } catch (Exception e) {
                    Logger.e($"Exception in OnApplicationFocus: {e.Message}\n" + e.StackTrace);
                }
            }
        }

        private void OnApplicationPause(bool paused)
        {
            foreach (var callback in s_pauseCallbacks) {
                try {
                    callback?.Invoke(paused);
                } catch (Exception e) {
                    Logger.e($"Exception in OnApplicationPause: {e.Message}\n" + e.StackTrace);
                }
            }
        }

        #endregion MonoBehaviour implementation

    }

}