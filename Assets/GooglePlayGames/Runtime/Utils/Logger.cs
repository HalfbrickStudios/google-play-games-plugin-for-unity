// <copyright file="Logger.cs" company="Google Inc.">
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

using System.Diagnostics.CodeAnalysis;

using UnityEngine;

namespace GooglePlayGames.Utils {

    // TODO: Make internal, there's no reason to expose rather than make it compatible with the sample test
    public static class Logger {

        private static readonly string Tag = "[GPG]";

        public static bool DebugLogEnabled   { get; set; }
        public static bool TraceLogEnabled   { get; set; }
        public static bool WarningLogEnabled { get; set; }

        static Logger()
        {
#if DEBUG
    #if GOOGLE_PLAY_GAMES_VERBOSE
            DebugLogEnabled   = true;
    #else
            DebugLogEnabled   = false;
    #endif
    #if GOOGLE_PLAY_GAMES_TRACE
            TraceLogEnabled   = true;
    #else
            TraceLogEnabled   = false;
    #endif
            WarningLogEnabled = true;
#else
            DebugLogEnabled   = false;
            TraceLogEnabled   = false;
            WarningLogEnabled = true;
#endif
        }

        [SuppressMessage("Style", "IDE1006", Justification = "Keep same API as other well-known loggers")]
        public static void d(string message)
        {
            if (!DebugLogEnabled) return;
            var text = ToLogMessage(prefix: ">>>", type: "DEBUG", message);
            Convert.RunUiAction(() => Debug.Log(text));
        }

        [SuppressMessage("Style", "IDE1006", Justification = "Keep same API as other well-known loggers")]
        public static void e(string message)
        {
            var text = ToLogMessage(prefix: "***", type: "ERROR", message);
            Convert.RunUiAction(() => Debug.LogError(text));
        }

        [SuppressMessage("Style", "IDE1006", Justification = "Keep same API as other well-known loggers")]
        public static void t(string message)
        {
            if (!TraceLogEnabled) return;
            var text = ToLogMessage(prefix: "###", type: "TRACE", message);
            Convert.RunUiAction(() => Debug.Log(text));
        }

        [SuppressMessage("Style", "IDE1006", Justification = "Keep same API as other well-known loggers")]
        public static void w(string message)
        {
            if (!WarningLogEnabled) return;
            var text = ToLogMessage(prefix: "!!!", type: "WARNING", message);
            Convert.RunUiAction(() => Debug.LogWarning(text));
        }

        [SuppressMessage("Style", "IDE1006", Justification = "Keep same API as other well-known loggers")]
        public static string describe(byte[] bytes) => bytes == null ? "(null)" : $"byte[{bytes.Length}]";

        private static string ToLogMessage(string prefix, string type, string message)
        {
            // string timeString = null;
            // try {
            //     timeString = DateTime.Now.ToString("MM/dd/yy H:mm:ss zzz");
            // } catch (Exception) {
            //     Convert.RunUiAction(() => Debug.LogWarning($"!!! {Tag} ERROR: Failed to format DateTime.Now"));
            //     timeString = string.Empty;
            // }
            // return $"{prefix} {Tag} {timeString} {type}: {message}";
            return $"{prefix} {Tag} [{type}]: {message}";
        }

    }

}