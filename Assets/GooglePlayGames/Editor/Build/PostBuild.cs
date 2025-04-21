// <copyright file="GpgEditorBuildCheck.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.  All Rights Reserved.
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

#if UNITY_EDITOR && UNITY_ANDROID

using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Callbacks;
using UnityEngine;

using static GooglePlayGames.Editor.Strings;
using static GooglePlayGames.Editor.Utils;
using static GooglePlayGames.Editor.UI.Utils;
using GooglePlayGames.Config;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace GooglePlayGames.Editor.Build {

    public class PostBuild : IPostGenerateGradleAndroidProject {

        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            path = Path.Combine(path, "src", "main", "AndroidManifest.xml");
            if (!File.Exists(path)) {
                Debug.LogError($"GPG: Manifest file does not exist @ {path}");
                return;
            }

            var text = File.ReadAllText(path);
            if (string.IsNullOrEmpty(text)) {
                Debug.LogError($"GPG: Manifest file is empty @ {path}");
                return;
            }

            var document = new XmlDocument();
            document.LoadXml(text);

            var manifest = document.SelectSingleNode("/manifest");
            if (manifest == null) {
                Debug.LogError($"GPG: Manifest node not found in AndroidManifest.xml @ {path}");
                return;
            }

            var application = manifest.SelectSingleNode("/application");
            if (application == null) {
                Debug.LogError($"GPG: Application node not found in AndroidManifest.xml @ {path}");
                return;
            }


            var name = "com.google.android.gms.games.unityVersion";
            var node = application.SelectSingleNode($"/meta-data[@android:name='{name}']");
            if (node != null) {
                Debug.LogWarning($"GPG: Metadata with {name} already exists in AndroidManifest.xml, overwritting it!");
                node.ParentNode.RemoveChild(node);
            }
            var element = document.CreateElement("meta-data");
            element.SetAttribute("android:name", name);
            element.SetAttribute("android:value", $"\\u003{Version.VersionString}");
            application.AppendChild(element);


            name = "com.google.games.bridge.NativeBridgeActivity";
            node = application.SelectSingleNode($"/activity[@android:name='{name}']");
            if (node != null) {
                Debug.LogWarning($"GPG: Activity with {name} already exists in AndroidManifest.xml, overwritting it!");
                node.ParentNode.RemoveChild(node);
            }
            element = document.CreateElement("activity");
            element.SetAttribute("android:name", name);
            element.SetAttribute("android:theme", "@android:style/Theme.Translucent.NoTitleBar.Fullscreen");
            application.AppendChild(element);


            if (GameInformation.HasInstance) {
                var info = GameInformation.Instance;
                if (info.HasAppId) {
                    name = "com.google.android.gms.games.APP_ID";
                    node = application.SelectSingleNode($"/meta-data[@android:name='{name}']");
                    if (node != null) {
                        Debug.LogWarning($"GPG: Metadata with {name} already exists in AndroidManifest.xml, overwritting it!");
                        node.ParentNode.RemoveChild(node);
                    }
                    element = document.CreateElement("meta-data");
                    element.SetAttribute("android:name", name);
                    element.SetAttribute("android:value", $"\\u003{info.AppId}");
                    application.AppendChild(element);
                }


                if (info.HasNearbyId) {
                    name = "com.google.android.gms.nearby.connection.SERVICE_ID";
                    node = application.SelectSingleNode($"/meta-data[@android:name='{name}']");
                    if (node != null) {
                        Debug.LogWarning($"GPG: Metadata with {name} already exists in AndroidManifest.xml, overwritting it!");
                        node.ParentNode.RemoveChild(node);
                    }
                    element = document.CreateElement("meta-data");
                    element.SetAttribute("android:name", name);
                    element.SetAttribute("android:value", info.NearbyId);
                    application.AppendChild(element);


                    var permissions = new[] { "ACCESS_COARSE_LOCATION", "CHANGE_WIFI_STATE", "BLUETOOTH", "BLUETOOTH_ADMIN", "CHANGE_WIFI_STATE"};
                    foreach (var it in permissions) {
                        name = $"android.permission.{it}";
                        node = manifest.SelectSingleNode($"/uses-permission[@android:name='{name}']");
                        if (node != null) {
                            element = document.CreateElement("uses-permission");
                            element.SetAttribute("android:name", name);
                            manifest.AppendChild(element);
                        }
                    }
                }
            } else {
                Debug.LogWarning("GPG: GameInformation instance not found, therefore the AndroidManifest.xml file won't be patched");
            }

            File.WriteAllText(path, document.OuterXml);
        }

        [PostProcessBuild(99999)]
        [SuppressMessage("Style", "IDE0060", Justification = "Required by the Unity's signature")]
        internal static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var done = ProjectSettings.Instance.GetBool(KEY_ANDROID_SETUP_DONE, false);
            if (done) return;
            Alert($"Warning: The {Title} package was not configured; Google Play Game Services will not work correctly");
        }

    }

}

#endif