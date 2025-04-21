// <copyright file="GpgEditorUiAndroid.cs" company="Google Inc.">
// Copyright (C) Google Inc. All Rights Reserved.
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
using System.Collections;
using System.IO;
using System.Xml;

using UnityEditor;
using UnityEngine;

using static GooglePlayGames.Editor.Utils;
using static GooglePlayGames.Editor.UI.Strings;
using static GooglePlayGames.Editor.UI.Utils;

namespace GooglePlayGames.Editor.UI {

    internal sealed class MenuAndroid : EditorWindow {

        private string  m_class         = "GPGSIds";
        private string  m_config        = string.Empty;
        private string  m_constantsPath = "Assets";
        private Vector2 m_scroll;
        private string  m_webId         = string.Empty;

        #region MenuItem implementation

        [MenuItem("Google/Play Games/Setup/Android...", true)]
#if UNITY_ANDROID
        private static bool EnableAndroidMenuItem() => true;
#else
        private static bool EnableAndroidMenuItem() => false;
#endif

        [MenuItem("Google/Play Games/Setup/Android...", false, 1)]
        private static void MenuItemFileGPGSAndroidSetup()
        {
            var window = GetWindow(typeof(MenuAndroid), true, AndroidSetup.Title);
            window.minSize = new Vector2(500, 400);
        }

        #endregion MenuItem implementation

        #region EditorWindow implementation

        private void OnEnable()
        {
            var settings = ProjectSettings.Instance;
            m_constantsPath = settings.Get(KEY_CLASS_DIRECTORY, m_constantsPath);
            m_class = settings.Get(KEY_CLASS_NAME, m_class);
            m_config = settings.Get(KEY_ANDROID_RESOURCE);
            m_webId = settings.Get(KEY_WEB_CLIENT_ID);
        }

        private void OnGUI()
        {
            GUI.skin.label.wordWrap = true;
            GUILayout.BeginVertical();

            var link = new GUIStyle(GUI.skin.label);
            link.normal.textColor = new Color(0f, 0f, 1f);

            GUILayout.Space(10);
            GUILayout.Label(AndroidSetup.Blurb);
            if (GUILayout.Button("Open Play Games Console", link, GUILayout.ExpandWidth(false))) {
                Application.OpenURL("https://play.google.com/apps/publish");
            }

            var last = GUILayoutUtility.GetLastRect();
            last.y += last.height - 2;
            last.x += 3;
            last.width -= 6;
            last.height = 2;

            GUI.Box(last, string.Empty);

            GUILayout.Space(15);
            GUILayout.Label("Constants class name", EditorStyles.boldLabel);
            GUILayout.Label("Enter the fully qualified name of the class to create containing the constants");
            GUILayout.Space(10);

            m_constantsPath = EditorGUILayout.TextField("Directory to save constants", m_constantsPath, GUILayout.MinWidth(480));

            m_class = EditorGUILayout.TextField("Constants class name", m_class, GUILayout.MinWidth(480));

            GUILayout.Label("Resources Definition", EditorStyles.boldLabel);
            GUILayout.Label("Paste in the Android Resources from the Play Console");
            GUILayout.Space(10);

            m_scroll = GUILayout.BeginScrollView(m_scroll);
            m_config = EditorGUILayout.TextArea(m_config, GUILayout.MinWidth(475), GUILayout.Height(Screen.height));
            GUILayout.EndScrollView();
            GUILayout.Space(10);

            GUILayout.Label(Setup.WebClientIdTitle, EditorStyles.boldLabel);
            GUILayout.Label(AndroidSetup.WebClientIdBlurb);

            m_webId = EditorGUILayout.TextField(Setup.ClientId, m_webId, GUILayout.MinWidth(450));

            GUILayout.Space(10);

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Setup.SetupButton, GUILayout.Width(100))) {
                try {
                    if (LooksLikeValidPackageName(m_class)) {
                        DoSetup();
                        return;
                    }
                } catch (Exception e) {
                    Error(0x314, "Invalid classname: " + e.Message);
                    Debug.LogException(e);
                }
            }

            if (GUILayout.Button("Cancel", GUILayout.Width(100))) {
                Close();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.EndVertical();
        }

        #endregion EditorWindow implementation

        private static void CheckBundleId()
        {
            var packageName = ProjectSettings.Instance.Get(KEY_ANDROID_BUNDLE_ID, string.Empty);
            var currentId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);

            if (string.IsNullOrEmpty(packageName)) {
                Debug.Log("NULL package!!");
                return;
            }

            if (string.IsNullOrEmpty(currentId) || currentId == "com.Company.ProductName") {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, packageName);
            } else if (currentId != packageName) {
                const string title = "Set Bundle Identifier?";
                var message = $"The server configuration is using {packageName}, but the player settings is set to {currentId}.\n" +
                              $"Set the Bundle Identifier to {packageName}?";
                if (EditorUtility.DisplayDialog(title, message, Ok, Cancel)) {
                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, packageName);
                }
            }
        }

        private static bool ParseResources(string classDirectory, string className, string res)
        {
            var resourceKeys = new Hashtable();
            var appId = null as string;
            
            using (var reader = new XmlTextReader(new StringReader(res))) {
                var inResource = false;
                var lastProp = null as string;
                while (reader.Read()) {
                    if (reader.Name == "resources") inResource = true;
                    if (!inResource) continue;
                
                    if (reader.Name == "string") {
                        lastProp = reader.GetAttribute("name");
                        continue;
                    }

                    if (!string.IsNullOrEmpty(lastProp)) {
                        if (reader.HasValue) {
                            if (lastProp == "app_id") {
                                appId = reader.Value;
                                ProjectSettings.Instance.Set(KEY_APP_ID, appId);
                            } else if (lastProp == "package_name") {
                                ProjectSettings.Instance.Set(KEY_ANDROID_BUNDLE_ID, reader.Value);
                            } else {
                                resourceKeys[lastProp] = reader.Value;
                            }
                            lastProp = null;
                        }
                    }
                }
            }

            if (resourceKeys.Count > 0) {
                // TODO: Check if this is necessary at all
                // WriteResourceIds(classDirectory, className, resourceKeys);
            }

            return appId != null;
        }

        private static bool PerformSetup(string webClientId, string appId, string nearbyServiceId)
        {
            if (!string.IsNullOrEmpty(webClientId)) {
                if (!LooksLikeValidClientId(webClientId)) {
                    Error(0x311, Setup.ClientIdError);
                    return false;
                }
                var serverAppId = webClientId.Split('-')[0];
                if (!serverAppId.Equals(appId)) {
                    Error(0x312, Setup.AppIdMismatch);
                    return false;
                }
            }

            if (!LooksLikeValidAppId(appId) && string.IsNullOrEmpty(nearbyServiceId)) {
                Error(0x313, Setup.AppIdError);
                return false;
            }

#if UNITY_ANDROID
            if (nearbyServiceId != null) {
                if (!MenuNearbyConnection.PerformSetup(nearbyServiceId, true)) {
                    return false;
                }
            }
#endif

            ProjectSettings.Instance.Set(KEY_APP_ID, appId);
            ProjectSettings.Instance.Set(KEY_WEB_CLIENT_ID, webClientId);
            ProjectSettings.Instance.Save();
            UpdateGameInfo();

            if (!HasAndroidSdk()) {
                Debug.LogError("Android SDK not found.");
                EditorUtility.DisplayDialog( AndroidSetup.SdkNotFound, AndroidSetup.SdkNotFoundBlurb, Ok);
                return false;
            }

            UpdateGameInfo();

            AssetDatabase.Refresh();
            ProjectSettings.Instance.Set(KEY_ANDROID_SETUP_DONE, true);
            ProjectSettings.Instance.Save();

            return true;
        }

        private static bool PerformSetup(string clientId, string classDirectory, string className, string resourceXmlData, string nearbyServiceId)
        {
            string appId;
            
            if (string.IsNullOrEmpty(resourceXmlData) && !string.IsNullOrEmpty(nearbyServiceId)) {
                appId = ProjectSettings.Instance.Get(KEY_APP_ID);
                return PerformSetup(clientId, appId, nearbyServiceId);
            }

            if (!ParseResources(classDirectory, className, resourceXmlData)) return false;
            
            ProjectSettings.Instance.Set(KEY_CLASS_DIRECTORY, classDirectory);
            ProjectSettings.Instance.Set(KEY_CLASS_NAME, className);
            ProjectSettings.Instance.Set(KEY_ANDROID_RESOURCE, resourceXmlData);

            CheckBundleId();

            CheckAndFixDependencies();
            CheckAndFixVersionedAssestsPaths();
            AssetDatabase.Refresh();

            EnableExternalDependencyResolverFlags(verbose: true);
            UpdateExternalDependencyResolverAssets(force: true);
            EnableExternalDependencyResolverFlags(enable: true);
            AssetDatabase.Refresh();

            ResolveExternalDependencies();

            appId = ProjectSettings.Instance.Get(KEY_APP_ID);
            return PerformSetup(clientId, appId, nearbyServiceId);
        }

        private void DoSetup()
        {
            if (!PerformSetup(m_webId, m_constantsPath, m_class, m_config, null)) {
                var message = "Invalid or missing XML resource data.\n" +
                              "Make sure the data is valid and contains the app_id element.";
                Error(0x315, message);
                return;
            }

            CheckBundleId();
            EditorUtility.DisplayDialog(Success, AndroidSetup.SetupComplete, Ok);
            ProjectSettings.Instance.Set(KEY_ANDROID_SETUP_DONE, true);
            Close();
        }

    }

}

#endif