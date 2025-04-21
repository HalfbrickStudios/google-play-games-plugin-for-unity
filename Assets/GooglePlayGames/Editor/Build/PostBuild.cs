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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;

using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Callbacks;
using UnityEngine;

using GooglePlayGames.Config;

using static GooglePlayGames.Editor.Strings;
using static GooglePlayGames.Editor.Utils;
using static GooglePlayGames.Editor.UI.Utils;

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

            var manifest = document.SelectSingleNode("manifest");
            if (manifest == null) {
                Debug.LogError($"GPG: Manifest node not found in AndroidManifest.xml @ {path}");
                return;
            }
            if (manifest.Attributes["xmlns:android"] == null) {
                Debug.LogWarning($"GPG: Manifest is missing the xmlns:android namespace");
                var attribute = document.CreateAttribute("xmlns:android");
                attribute.Value = "http://schemas.android.com/apk/res/android";
                manifest.Attributes.Append(attribute);
            }
            var android = manifest.Attributes["xmlns:android"].Value;

            var application = manifest.SelectSingleNode("application");
            if (application == null) {
                Debug.LogError($"GPG: Application node not found in AndroidManifest.xml @ {path}");
                return;
            }

            var manager = new XmlNamespaceManager(document.NameTable);
            if (!manager.HasNamespace("android")) {
                manager.AddNamespace("android", android);
            }
            if (!manager.HasNamespace("android")) {
                Debug.LogError($"GPG: android XML namespace not found in AndroidManifest.xml @ {path}");
                return;
            }


            var name = "com.google.android.gms.games.unityVersion";
            var node = application.SelectNodes($"meta-data[@android:name='{name}']", manager).OfType<XmlNode>().ToList();
            if (node.Count > 0) {
                Debug.LogWarning($"GPG: Metadata with {name} already exists in AndroidManifest.xml, overwritting it!");
                foreach (var it in node) {
                    it.ParentNode.RemoveChild(it);
                }
            }
            var element = document.CreateElement("meta-data");
            element.SetAttribute("name", android, name);
            element.SetAttribute("value", android, $"\\u003{Version.VersionString}");
            application.AppendChild(element);


            name = "com.google.games.bridge.NativeBridgeActivity";
            node = application.SelectNodes($"activity[@android:name='{name}']", manager).OfType<XmlNode>().ToList();
            if (node.Count > 0) {
                Debug.LogWarning($"GPG: Activity with {name} already exists in AndroidManifest.xml, overwritting it!");
                foreach (var it in node) {
                    it.ParentNode.RemoveChild(it);
                }
            }
            element = document.CreateElement("activity");
            element.SetAttribute("name", android, name);
            element.SetAttribute("theme", android, "@android:style/Theme.Translucent.NoTitleBar.Fullscreen");
            application.AppendChild(element);


            if (GameInformation.HasInstance) {
                var info = GameInformation.Instance;
                if (info.HasAppId) {
                    name = "com.google.android.gms.games.APP_ID";
                    node = application.SelectNodes($"meta-data[@android:name='{name}']", manager).OfType<XmlNode>().ToList();
                    if (node.Count > 0) {
                        Debug.LogWarning($"GPG: Metadata with {name} already exists in AndroidManifest.xml, overwritting it!");
                        foreach (var it in node) {
                            it.ParentNode.RemoveChild(it);
                        }
                    }
                    element = document.CreateElement("meta-data");
                    element.SetAttribute("name", android, name);
                    element.SetAttribute("value", android, $"\\u003{info.AppId}");
                    application.AppendChild(element);
                }


                if (info.HasNearbyId) {
                    name = "com.google.android.gms.nearby.connection.SERVICE_ID";
                    node = application.SelectNodes($"meta-data[@android:name='{name}']", manager).OfType<XmlNode>().ToList();
                    if (node.Count > 0) {
                        Debug.LogWarning($"GPG: Metadata with {name} already exists in AndroidManifest.xml, overwritting it!");
                        foreach (var it in node) {
                            it.ParentNode.RemoveChild(it);
                        }
                    }
                    element = document.CreateElement("meta-data");
                    element.SetAttribute("name", android, name);
                    element.SetAttribute("value", android, info.NearbyId);
                    application.AppendChild(element);


                    var permissions = new[] { "ACCESS_COARSE_LOCATION", "CHANGE_WIFI_STATE", "BLUETOOTH", "BLUETOOTH_ADMIN", "CHANGE_WIFI_STATE"};
                    foreach (var it in permissions) {
                        name = $"android.permission.{it}";
                        node = application.SelectNodes($"uses-permission[@android:name='{name}']", manager).OfType<XmlNode>().ToList();
                        if (node.Count > 0) {
                            element = document.CreateElement("uses-permission");
                            element.SetAttribute("name", android, name);
                            manifest.AppendChild(element);
                        }
                    }
                }
            } else {
                Debug.LogWarning("GPG: GameInformation instance not found, therefore the AndroidManifest.xml file will be partially patched");
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