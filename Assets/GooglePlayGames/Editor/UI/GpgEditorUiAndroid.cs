// <copyright file="GPGSAndroidSetupUI.cs" company="Google Inc.">
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

using static GooglePlayGames.Editor.GpgEditorStrings;
using static GooglePlayGames.Editor.GpgEditorUtils;

namespace GooglePlayGames.Editor.UI {

    /// <summary>
    /// Google Play Game Services Setup dialog for Android.
    /// </summary>
    sealed internal class GpgEditorUiAndroid : EditorWindow {

        /// <summary>
        /// The configuration data from the play games console "resource data"
        /// </summary>
        private string m_config = string.Empty;

        /// <summary>
        /// The name of the class to generate containing the resource constants.
        /// </summary>
        private string m_class = "GPGSIds";

        /// <summary>
        /// The scroll position
        /// </summary>
        private Vector2 m_scroll;

        /// <summary>
        /// The directory for the constants class.
        /// </summary>
        private string m_constantsPath = "Assets";

        /// <summary>
        /// The web client identifier.
        /// </summary>
        private string m_webId = string.Empty;

        /// <summary>
        /// Menus the item for GPGS android setup.
        /// </summary>
        [MenuItem("Google/Play Games/Setup/Android...", false, 1)]
        private static void MenuItemFileGPGSAndroidSetup()
        {
            var window = GetWindow(typeof(GpgEditorUiAndroid), true, AndroidSetup.Title);
            window.minSize = new Vector2(500, 400);
        }

        [MenuItem("Google/Play Games/Setup/Android...", true)]
#if UNITY_ANDROID
        private static bool EnableAndroidMenuItem() => true;
#else
        private static bool EnableAndroidMenuItem() => false;
#endif

        /// <summary>
        /// Performs setup using the Android resources downloaded XML file
        /// from the play console.
        /// </summary>
        /// <returns><c>true</c>, if setup was performed, <c>false</c> otherwise.</returns>
        /// <param name="clientId">The web client id.</param>
        /// <param name="classDirectory">the directory to write the constants file to.</param>
        /// <param name="className">Fully qualified class name for the resource Ids.</param>
        /// <param name="resourceXmlData">Resource xml data.</param>
        /// <param name="nearbyServiceId">Nearby svc identifier.</param>
        private static bool PerformSetup(string clientId, string classDirectory, string className, string resourceXmlData, string nearbyServiceId)
        {
            string appId;
            
            if (string.IsNullOrEmpty(resourceXmlData) && !string.IsNullOrEmpty(nearbyServiceId)) {
                appId = GpgEditorProjectSettings.Instance.Get(KEY_APP_ID);
                return PerformSetup(clientId, appId, nearbyServiceId);
            }

            if (!ParseResources(classDirectory, className, resourceXmlData)) {
                return false;
            }
            
            GpgEditorProjectSettings.Instance.Set(KEY_CLASS_DIRECTORY, classDirectory);
            GpgEditorProjectSettings.Instance.Set(KEY_CLASS_NAME, className);
            GpgEditorProjectSettings.Instance.Set(KEY_ANDROID_RESOURCE, resourceXmlData);

            // check the bundle id and set it if needed.
            CheckBundleId();

            CheckAndFixDependencies();
            CheckAndFixVersionedAssestsPaths();
            AssetDatabase.Refresh();

            EnableExternalDependencyResolverFlags(verbose: true);
            UpdateExternalDependencyResolverAssets(force: true);
            EnableExternalDependencyResolverFlags(enable: true);
            AssetDatabase.Refresh();

            ResolveExternalDependencies();

            appId = GpgEditorProjectSettings.Instance.Get(KEY_APP_ID);
            return PerformSetup(clientId, appId, nearbyServiceId);
        }

        /// <summary>
        /// Provide static access to setup for facilitating automated builds.
        /// </summary>
        /// <param name="webClientId">The oauth2 client id for the game.  This is only
        /// needed if the ID Token or access token are needed.</param>
        /// <param name="appId">App identifier.</param>
        /// <param name="nearbyServiceId">Optional nearby connection serviceId</param>
        /// <returns>true if successful</returns>
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

            // check for valid app id
            if (!LooksLikeValidAppId(appId) && string.IsNullOrEmpty(nearbyServiceId)) {
                Error(0x313, Setup.AppIdError);
                return false;
            }

#if UNITY_ANDROID
            if (nearbyServiceId != null) {
                if (!GpgEditorUiNearbyConnection.PerformSetup(nearbyServiceId, true)) {
                    return false;
                }
            }
#endif

            GpgEditorProjectSettings.Instance.Set(KEY_APP_ID, appId);
            GpgEditorProjectSettings.Instance.Set(KEY_WEB_CLIENT_ID, webClientId);
            GpgEditorProjectSettings.Instance.Save();
            UpdateGameInfo();

            // check that Android SDK is there
            if (!HasAndroidSdk()) {
                Debug.LogError("Android SDK not found.");
                EditorUtility.DisplayDialog( AndroidSetup.SdkNotFound, AndroidSetup.SdkNotFoundBlurb, Ok);
                return false;
            }

            // Generate AndroidManifest.xml
            GenerateAndroidManifest();

            // refresh assets, and we're done
            AssetDatabase.Refresh();
            GpgEditorProjectSettings.Instance.Set(KEY_ANDROID_SETUP_DONE, true);
            GpgEditorProjectSettings.Instance.Save();

            return true;
        }

        /// <summary>
        /// Called when this object is enabled by Unity editor.
        /// </summary>
        private void OnEnable()
        {
            var settings = GpgEditorProjectSettings.Instance;
            m_constantsPath = settings.Get(KEY_CLASS_DIRECTORY, m_constantsPath);
            m_class = settings.Get(KEY_CLASS_NAME, m_class);
            m_config = settings.Get(KEY_ANDROID_RESOURCE);
            m_webId = settings.Get(KEY_WEB_CLIENT_ID);
        }

        /// <summary>
        /// Called when the GUI should be rendered.
        /// </summary>
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

            // Client ID field
            GUILayout.Label(Setup.WebClientIdTitle, EditorStyles.boldLabel);
            GUILayout.Label(AndroidSetup.WebClientIdBlurb);

            m_webId = EditorGUILayout.TextField(Setup.ClientId, m_webId, GUILayout.MinWidth(450));

            GUILayout.Space(10);

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Setup.SetupButton, GUILayout.Width(100))) {
                // check that the classname entered is valid
                try {
                    if (LooksLikeValidPackageName(m_class)) {
                        DoSetup();
                        return;
                    }
                } catch (Exception e) {
                    Error(0x314, "Invalid classname: " + e.Message);
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

        /// <summary>
        /// Starts the setup process.
        /// </summary>
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
            GpgEditorProjectSettings.Instance.Set(KEY_ANDROID_SETUP_DONE, true);
            Close();
        }

        /// <summary>
        /// Checks the bundle identifier.
        /// </summary>
        /// <remarks>
        /// Check the package id.  If one is set the gpgs properties,
        /// and the player settings are the default or empty, set it.
        /// if the player settings is not the default, then prompt before
        /// overwriting.
        /// </remarks>
        private static void CheckBundleId()
        {
            var packageName = GpgEditorProjectSettings.Instance.Get(KEY_ANDROID_BUNDLE_ID, string.Empty);
#if UNITY_5_6_OR_NEWER
            var currentId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#else
            var currentId = PlayerSettings.bundleIdentifier;
#endif

            if (string.IsNullOrEmpty(packageName)) {
                Debug.Log("NULL package!!");
                return;
            }

            if (string.IsNullOrEmpty(currentId) || currentId == "com.Company.ProductName") {
#if UNITY_5_6_OR_NEWER
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, packageName);
#else
                PlayerSettings.bundleIdentifier = packageName;
#endif
            } else if (currentId != packageName) {
                var title = "Set Bundle Identifier?";
                var message = $"The server configuration is using {packageName}, but the player settings is set to {currentId}.\n" +
                              $"Set the Bundle Identifier to {packageName}?";
                if (EditorUtility.DisplayDialog(title, message, Ok, Cancel)) {
#if UNITY_5_6_OR_NEWER
                    PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, packageName);
#else
                    PlayerSettings.bundleIdentifier = packageName;
#endif
                }
            }
        }

        /// <summary>
        /// Parses the resources xml and set the properties.  Also generates the
        /// constants file.
        /// </summary>
        /// <returns><c>true</c>, if resources was parsed, <c>false</c> otherwise.</returns>
        /// <param name="classDirectory">Class directory.</param>
        /// <param name="className">Class name.</param>
        /// <param name="res">Res. the data to parse.</param>
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
                                GpgEditorProjectSettings.Instance.Set(KEY_APP_ID, appId);
                            } else if (lastProp == "package_name") {
                                GpgEditorProjectSettings.Instance.Set(KEY_ANDROID_BUNDLE_ID, reader.Value);
                            } else {
                                resourceKeys[lastProp] = reader.Value;
                            }
                            lastProp = null;
                        }
                    }
                }
            }

            if (resourceKeys.Count > 0) {
                WriteResourceIds(classDirectory, className, resourceKeys);
            }

            return appId != null;
        }
    }
}

#endif