// <copyright file="GpgEditorUiNearbyConnection.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc. All Rights Reserved.
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

using UnityEngine;
using UnityEditor;

using static GooglePlayGames.Editor.GpgEditorStrings;
using static GooglePlayGames.Editor.GpgEditorUtils;

namespace GooglePlayGames.Editor.UI {

    internal sealed class GpgEditorUiNearbyConnection : EditorWindow {

        [MenuItem("Google/Play Games/Setup/Nearby Connections...", true)]
#if UNITY_ANDROID
        private static bool EnableNearbyMenuItem() => true;
#else
        private static bool EnableNearbyMenuItem() => false;
#endif

        [MenuItem("Google/Play Games/Setup/Nearby Connections...", false, 3)]
        private static void MenuItemNearbySetup()
        {
            var window = GetWindow(typeof(GpgEditorUiNearbyConnection), true, NearbyConnections.Title);
            window.minSize = new Vector2(400, 200);
        }

        /// Provide static access to setup for facilitating automated builds.
        /// <param name="id">nearby connections service id</param>
        /// <param name="android">true if building android</param>
        internal static bool PerformSetup(string id, bool android)
        {
            // check for valid app id
            if (!LooksLikeValidServiceId(id)) {
                var title = "Remove Nearby connection permissions?  ";
                var message = "The service Id is invalid.  It must follow package naming rules.  " +
                              "Do you want to remove the AndroidManifest entries for Nearby connections?";
                var dialog = EditorUtility.DisplayDialog(title, message, Yes, No);
                if (!dialog) return false;
                GpgEditorProjectSettings.Instance.Set(KEY_SERVICE_ID, null);
                GpgEditorProjectSettings.Instance.Save();
            } else {
                GpgEditorProjectSettings.Instance.Set(KEY_SERVICE_ID, id);
                GpgEditorProjectSettings.Instance.Save();
            }

            if (!android) return true;

            EnsureDirExists("Assets/Plugins");
            EnsureDirExists("Assets/Plugins/Android");

            GenerateAndroidManifest();

            GpgEditorProjectSettings.Instance.Set(KEY_NEARBY_SETUP_DONE, true);
            GpgEditorProjectSettings.Instance.Save();

            EnableExternalDependencyResolverFlags(verbose: true);
            UpdateExternalDependencyResolverAssets(force: true);
            EnableExternalDependencyResolverFlags(enable: true);
            AssetDatabase.Refresh();

            ResolveExternalDependencies();
            return true;
        }

        private string m_id = string.Empty;

        private void DoSetup()
        {
            if (!PerformSetup(m_id, true)) return;
            EditorUtility.DisplayDialog(Success, NearbyConnections.SetupComplete, Ok);
            Close();
        }

        #region EditorWindow implementation

        private void OnEnable()
        {
            m_id = GpgEditorProjectSettings.Instance.Get(KEY_SERVICE_ID);
        }

        private void OnGUI()
        {
            GUI.skin.label.wordWrap = true;
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Label(NearbyConnections.Blurb);
            GUILayout.Space(10);

            GUILayout.Label(Setup.NearbyServiceId, EditorStyles.boldLabel);
            GUILayout.Space(10);
            GUILayout.Label(Setup.NearbyServiceBlurb);
            m_id = EditorGUILayout.TextField(Setup.NearbyServiceId, m_id, GUILayout.Width(350));

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(Setup.SetupButton, GUILayout.Width(100))) DoSetup();
            if (GUILayout.Button(Cancel, GUILayout.Width(100))) Close();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.EndVertical();
        }

        #endregion EditorWindow implementation

    }

}

#endif