// <copyright file="GPGSProjectSettings.cs" company="Google Inc.">
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

// Keep this file even on unsupported configurations.

using System.Collections.Generic;
using System.IO;

#if UNITY_2017_3_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine;
#endif

namespace GooglePlayGames.Editor {

    public class GPGSProjectSettings {

        private static GPGSProjectSettings s_instance = null;

        public static GPGSProjectSettings Instance
        {
            get => s_instance ??= new GPGSProjectSettings();
        }

        private bool m_dirty = false;
        private readonly string m_file;
        private Dictionary<string, string> m_dict = new();

        private GPGSProjectSettings()
        {
            m_file = GpgUtils.SlashesToPlatformSeparator("ProjectSettings/GooglePlayGameSettings.txt");

            var files = new[] {
                m_file,
                GpgUtils.SlashesToPlatformSeparator(Path.Combine(GpgUtils.RootPath, "Editor/projsettings.txt")),
                GpgUtils.SlashesToPlatformSeparator("Assets/Editor/projsettings.txt")
            };

            StreamReader rd = null;
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

        public string Get(string key, Dictionary<string, string> overrides)
        {
            if (overrides.ContainsKey(key)) {
                return overrides[key];
            } else if (m_dict.ContainsKey(key)) {
#if UNITY_2017_3_OR_NEWER
                return UnityWebRequest.UnEscapeURL(m_dict[key]);
#else
                return WWW.UnEscapeURL(m_dict[key]);
#endif
            } else {
                return string.Empty;
            }
        }

        public string Get(string key, string defaultValue)
        {
            if (m_dict.ContainsKey(key)) {
#if UNITY_2017_3_OR_NEWER
                return UnityWebRequest.UnEscapeURL(m_dict[key]);
#else
                return WWW.UnEscapeURL(m_dict[key]);
#endif
            } else {
                return defaultValue;
            }
        }

        public string Get(string key) => Get(key, string.Empty);

        public bool GetBool(string key, bool defaultValue) => Get(key, defaultValue ? "true" : "false").Equals("true");

        public bool GetBool(string key) => Get(key, "false").Equals("true");

        public void Set(string key, string val)
        {
#if UNITY_2017_3_OR_NEWER
            var escaped = UnityWebRequest.EscapeURL(val);
#else
            var escaped = WWW.EscapeURL(val);
#endif
            m_dict[key] = escaped;
            m_dirty = true;
        }

        public void Set(string key, bool val) => Set(key, val ? "true" : "false");

        public void Save()
        {
            var args = System.Environment.GetCommandLineArgs();
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

        public static void Reload() => s_instance = new GPGSProjectSettings();

    }

}
