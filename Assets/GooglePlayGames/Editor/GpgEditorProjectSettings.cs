// <copyright file="GpgEditorProjectSettings.cs" company="Google Inc.">
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

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine.Networking;

using static GooglePlayGames.Editor.GpgEditorUtils;

namespace GooglePlayGames.Editor {

    public class GpgEditorProjectSettings {

        private static GpgEditorProjectSettings s_instance = null;

        public static GpgEditorProjectSettings Instance => s_instance ??= new GpgEditorProjectSettings();

        private bool m_dirty = false;
        private readonly string m_file;
        private Dictionary<string, string> m_dict = new();

        private GpgEditorProjectSettings()
        {
            m_file = SlashesToPlatformSeparator("ProjectSettings/GooglePlayGameSettings.txt");

            var files = new[] {
                m_file,
                SlashesToPlatformSeparator(Path.Combine(RootPath, "Editor/projsettings.txt")),
                SlashesToPlatformSeparator("Assets/Editor/projsettings.txt")
            };

            var rd = null as StreamReader;
            foreach (var it in files) {
                if (File.Exists(it)) {
                    rd = new StreamReader(it);
                    break;
                }
            }
            if (rd == null) return;

            using (rd) {
                while (!rd.EndOfStream) {
                    var line = rd.ReadLine();
                    if (line == null) break;
                    line = line.Trim();
                    if (line.Length == 0) break;
                    var parts = line.Split('=', 2);
                    if (parts.Length >= 2) {
                        m_dict[parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
        }

        public string Get(string key) => Get(key, string.Empty);

        public string Get(string key, string defaultValue)
        {
            if (m_dict.ContainsKey(key)) {
                return UnityWebRequest.UnEscapeURL(m_dict[key]);
            } else {
                return defaultValue;
            }
        }

        public string Get(string key, Dictionary<string, string> overrides)
        {
            if (overrides.ContainsKey(key)) {
                return overrides[key];
            } else if (m_dict.ContainsKey(key)) {
                return UnityWebRequest.UnEscapeURL(m_dict[key]);
            } else {
                return string.Empty;
            }
        }

        public bool GetBool(string key, bool defaultValue) => Get(key, defaultValue ? "true" : "false").Equals("true");

        public void Set(string key, bool val) => Set(key, val ? "true" : "false");

        public void Set(string key, string val)
        {
            var escaped = UnityWebRequest.EscapeURL(val);
            m_dict[key] = escaped;
            m_dirty = true;
        }

        public void Save()
        {
            var args = Environment.GetCommandLineArgs();
            foreach (var arg in args) {
                if (arg == "-g.building") {
                    m_dirty = false;
                    break;
                }
            }
            if (!m_dirty) return;

            using var sw = new StreamWriter(m_file, false);
            foreach (var key in m_dict.Keys) {
                sw.WriteLine(key + "=" + m_dict[key]);
            }
            m_dirty = false;
        }

    }

}

#endif