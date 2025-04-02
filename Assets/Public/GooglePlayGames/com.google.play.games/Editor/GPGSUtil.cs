// <copyright file="GPGSUtil.cs" company="Google Inc.">
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
// Keep this even on unsupported configurations.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace GooglePlayGames.Editor {

    /// <summary>
    /// Utility class to perform various tasks in the editor.
    /// </summary>
    public static class GPGSUtil {

        /// <summary>Property key for project settings.</summary>
        public const string SERVICEIDKEY = "App.NearbdServiceId";

        /// <summary>Property key for project settings.</summary>
        public const string APPIDKEY = "proj.AppId";

        /// <summary>Property key for project settings.</summary>
        public const string CLASSDIRECTORYKEY = "proj.classDir";

        /// <summary>Property key for project settings.</summary>
        public const string CLASSNAMEKEY = "proj.ConstantsClassName";

        /// <summary>Property key for project settings.</summary>
        public const string WEBCLIENTIDKEY = "and.ClientId";

        /// <summary>Property key for project settings.</summary>
        public const string ANDROIDRESOURCEKEY = "and.ResourceData";

        /// <summary>Property key for project settings.</summary>
        public const string ANDROIDSETUPDONEKEY = "android.SetupDone";

        /// <summary>Property key for project settings.</summary>
        public const string ANDROIDBUNDLEIDKEY = "and.BundleId";

        /// <summary>Property key for plugin version.</summary>
        public const string PLUGINVERSIONKEY = "proj.pluginVersion";

        /// <summary>Property key for nearby settings done.</summary>
        public const string NEARBYSETUPDONEKEY = "android.NearbySetupDone";

        /// <summary>Property key for project settings.</summary>
        public const string LASTUPGRADEKEY = "lastUpgrade";

        /// <summary>Constant for token replacement</summary>
        private const string SERVICEIDPLACEHOLDER = "__NEARBY_SERVICE_ID__";

        private const string SERVICEID_ELEMENT_PLACEHOLDER = "__NEARBY_SERVICE_ELEMENT__";

        private const string NEARBY_PERMISSIONS_PLACEHOLDER = "__NEARBY_PERMISSIONS__";

        /// <summary>Constant for token replacement</summary>
        private const string APPIDPLACEHOLDER = "__APP_ID__";

        /// <summary>Constant for token replacement</summary>
        private const string CLASSNAMEPLACEHOLDER = "__Class__";

        /// <summary>Constant for token replacement</summary>
        private const string WEBCLIENTIDPLACEHOLDER = "__WEB_CLIENTID__";

        /// <summary>Constant for token replacement</summary>
        private const string PLUGINVERSIONPLACEHOLDER = "__PLUGIN_VERSION__";

        /// <summary>Constant for require google plus token replacement</summary>
        private const string REQUIREGOOGLEPLUSPLACEHOLDER = "__REQUIRE_GOOGLE_PLUS__";

        /// <summary>Property key for project settings.</summary>
        private const string TOKENPERMISSIONKEY = "proj.tokenPermissions";

        /// <summary>Constant for token replacement</summary>
        private const string NAMESPACESTARTPLACEHOLDER = "__NameSpaceStart__";

        /// <summary>Constant for token replacement</summary>
        private const string NAMESPACEENDPLACEHOLDER = "__NameSpaceEnd__";

        /// <summary>Constant for token replacement</summary>
        private const string CONSTANTSPLACEHOLDER = "__Constant_Properties__";

        /// <summary>
        /// The game info file path, relative to the plugin root directory.  This is a generated file.
        /// </summary>
        private const string GameInfoRelativePath = "Runtime/Scripts/GameInfo.cs";

        /// <summary>
        /// The manifest path, relative to the plugin root directory.
        /// </summary>
        /// <remarks>The Games SDK requires additional metadata in the AndroidManifest.xml
        ///     file. </remarks>
        private const string ManifestRelativePath = "../../Plugins/Android/GooglePlayGamesManifest.androidlib/AndroidManifest.xml";

        private const string RootFolderName = "com.google.play.games";

        /// <summary>
        /// The root path of the Google Play Games plugin
        /// </summary>
        private static string m_rootPath = "";

        /// <summary>
        /// The root path of the Google Play Games plugin
        /// </summary>
        public static string RootPath {
            get {
                if (string.IsNullOrEmpty(m_rootPath)) {
                    var assets = Directory.GetDirectories("Assets", RootFolderName, SearchOption.AllDirectories);
#if UNITY_2018_4_OR_NEWER
                    var packages = Directory.GetDirectories("Packages", RootFolderName, SearchOption.AllDirectories);
                    var length = packages.Length;
                    Array.Resize(ref packages, length + assets.Length);
                    Array.Copy(assets, 0, packages, length, assets.Length);
#else
                    var packages = assets; // For older Unity versions, only search in Assets
#endif
                    switch (packages.Length) {
                        case 0:
                            // Patch 1: handle the case when the plugin is installed from the package cache
                            var cache = Directory.GetDirectories("Library/PackageCache", $"{RootFolderName}*", SearchOption.TopDirectoryOnly);
                            if (cache.Length == 1) {
                                m_rootPath = SlashesToPlatformSeparator(cache[0]);
                                break;
                            } else {
                                // Patch 2: handle the case when the plugin is installed locally
                                var info = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/" + RootFolderName);
                                if (info != null) {
                                    m_rootPath = SlashesToPlatformSeparator(info.resolvedPath);
                                    break;
                                } else {
                                    Alert($"Plugin error (1): {RootFolderName} folder was renamed or not found");
                                    throw new Exception($"{RootFolderName} folder was renamed or not found");
                                }
                            }
                        case 1:
                            m_rootPath = SlashesToPlatformSeparator(packages[0]);
                            break;
                        default:
                            for (var i = 0; i < packages.Length; i += 1) {
                                var info = SlashesToPlatformSeparator(Path.Combine(packages[i], GameInfoRelativePath));
                                if (File.Exists(info)) {
                                    m_rootPath = SlashesToPlatformSeparator(packages[i]);
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(m_rootPath)) {
                                Alert($"Plugin error (2): {RootFolderName} folder was renamed");
                                throw new Exception($"{RootFolderName} folder was renamed");
                            }
                            break;
                    }
                }

                if (m_rootPath.Contains(RootFolderName + '@')) {
                    m_rootPath = m_rootPath.Replace("Packages", "Library/PackageCache");
                }
                return m_rootPath;
            }
        }

        /// <summary>
        /// The game info file path.  This is a generated file.
        /// </summary>
        private static string GameInfoPath
        {
            get => SlashesToPlatformSeparator(Path.Combine(RootPath, GameInfoRelativePath));
        }

        /// <summary>
        /// The manifest path.
        /// </summary>
        /// <remarks>The Games SDK requires additional metadata in the AndroidManifest.xml
        ///     file. </remarks>
        private static string ManifestPath
        {
            get => SlashesToPlatformSeparator(Path.Combine(RootPath, ManifestRelativePath));
        }

        /// <summary>
        /// The map of replacements for filling in code templates.  The
        /// key is the string that appears in the template as a placeholder,
        /// the value is the key into the GPGSProjectSettings.
        /// </summary>
        private static readonly Dictionary<string, string> s_replacements = new() {
            // Put this element placeholder first, since it has embedded placeholder
            {SERVICEID_ELEMENT_PLACEHOLDER, SERVICEID_ELEMENT_PLACEHOLDER},
            {SERVICEIDPLACEHOLDER, SERVICEIDKEY},
            {APPIDPLACEHOLDER, APPIDKEY},
            {CLASSNAMEPLACEHOLDER, CLASSNAMEKEY},
            {WEBCLIENTIDPLACEHOLDER, WEBCLIENTIDKEY},
            {PLUGINVERSIONPLACEHOLDER, PLUGINVERSIONKEY},
            // Causes the placeholder to be replaced with overridden value at runtime.
            {NEARBY_PERMISSIONS_PLACEHOLDER, NEARBY_PERMISSIONS_PLACEHOLDER}
        };

        /// <summary>
        /// Replaces / in file path to be the os specific separator.
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="path">Path with correct separators.</param>
        public static string SlashesToPlatformSeparator(string path) => path.Replace("/", Path.DirectorySeparatorChar.ToString());

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <returns>The file contents.  The slashes are corrected.</returns>
        /// <param name="filePath">File path.</param>
        public static string ReadFile(string filePath)
        {
            filePath = SlashesToPlatformSeparator(filePath);
            if (!File.Exists(filePath)) {
                Alert("Plugin error: file not found: " + filePath);
                return null;
            }
            using var sr = new StreamReader(filePath);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Reads the editor template.
        /// </summary>
        /// <returns>The editor template contents.</returns>
        /// <param name="name">Name of the template in the editor directory.</param>
        public static string ReadEditorTemplate(string name)
        {
            var file = string.Format("Editor{0}{1}.txt", Path.DirectorySeparatorChar, name);
            var path = Path.Combine(RootPath, file);
            return ReadFile(path);
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="file">File path - the slashes will be corrected.</param>
        /// <param name="body">Body of the file to write.</param>
        public static void WriteFile(string file, string body)
        {
            file = SlashesToPlatformSeparator(file);
            var dir = Directory.GetParent(file);
            dir.Create();
            using var wr = new StreamWriter(file, false);
            wr.Write(body);
        }

        /// <summary>
        /// Validates the string to be a valid nearby service id.
        /// </summary>
        /// <returns><c>true</c>, if like valid service identifier was looksed, <c>false</c> otherwise.</returns>
        /// <param name="s">string to test.</param>
        public static bool LooksLikeValidServiceId(string s)
        {
            if (s.Length < 3) return false;
            foreach (var c in s) {
                if (!char.IsLetterOrDigit(c) && c != '.') return false;
            }
            return true;
        }

        /// <summary>
        /// Looks the like valid app identifier.
        /// </summary>
        /// <returns><c>true</c>, if valid app identifier, <c>false</c> otherwise.</returns>
        /// <param name="s">the string to test.</param>
        public static bool LooksLikeValidAppId(string s)
        {
            if (s.Length < 5) return false;
            foreach (var c in s) {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }

        /// <summary>
        /// Looks the like valid client identifier.
        /// </summary>
        /// <returns><c>true</c>, if valid client identifier, <c>false</c> otherwise.</returns>
        /// <param name="s">the string to test.</param>
        public static bool LooksLikeValidClientId(string s) => s.EndsWith(".googleusercontent.com");

        /// <summary>
        /// Looks the like a valid bundle identifier.
        /// </summary>
        /// <returns><c>true</c>, if valid bundle identifier, <c>false</c> otherwise.</returns>
        /// <param name="s">the string to test.</param>
        public static bool LooksLikeValidBundleId(string s) => s.Length > 3;

        /// <summary>
        /// Looks like a valid package.
        /// </summary>
        /// <returns><c>true</c>, if  valid package name, <c>false</c> otherwise.</returns>
        /// <param name="s">the string to test.</param>
        public static bool LooksLikeValidPackageName(string s)
        {
            if (string.IsNullOrEmpty(s)) throw new Exception("cannot be empty");

            var parts = s.Split(new char[] {'.'});
            foreach (var part in parts) {
                var bytes = part.ToCharArray();
                for (var i = 0; i < bytes.Length; i += 1) {
                    if (i == 0 && !char.IsLetter(bytes[i])) {
                        throw new Exception("each part must start with a letter");
                    } else if (char.IsWhiteSpace(bytes[i])) {
                        throw new Exception("cannot contain spaces");
                    } else if (!char.IsLetterOrDigit(bytes[i]) && bytes[i] != '_') {
                        throw new Exception("must be alphanumeric or _");
                    }
                }
            }
            return parts.Length >= 1;
        }

        /// <summary>
        /// Determines if is setup done.
        /// </summary>
        /// <returns><c>true</c> if is setup done; otherwise, <c>false</c>.</returns>
        public static bool IsSetupDone()
        {
            var done = true;
#if UNITY_ANDROID
            done = GPGSProjectSettings.Instance.GetBool(ANDROIDSETUPDONEKEY, false);
            if (File.Exists(GameInfoPath)) {
                var contents = ReadFile(GameInfoPath);
                if (contents.Contains(APPIDPLACEHOLDER)) {
                    Debug.Log("GameInfo not initialized with AppId; run Window > Google Play Games > Setup > Android Setup...");
                    return false;
                }
            } else {
                Debug.Log("GameInfo.cs does not exist.  Run Window > Google Play Games > Setup > Android Setup...");
                return false;
            }
#endif
            return done;
        }

        /// <summary>
        /// Makes legal identifier from string.
        /// Returns a legal C# identifier from the given string.  The transformations are:
        ///   - spaces => underscore _
        ///   - punctuation => empty string
        ///   - leading numbers are prefixed with underscore.
        /// </summary>
        /// <returns>the id</returns>
        /// <param name="key">Key to convert to an identifier.</param>
        public static string MakeIdentifier(string key)
        {
            if (string.IsNullOrEmpty(key)) return "_";
            var invalid = key.Trim().Replace(' ', '_');
            var acc = string.Empty;
            foreach (var c in invalid) {
                if (char.IsLetterOrDigit(c) || c == '_') {
                    acc += c;
                }
            }
            return acc;
        }

        /// <summary>
        /// Displays an error dialog.
        /// </summary>
        /// <param name="s">the message</param>
        public static void Alert(string s) => Alert(GPGSStrings.Error, s);

        /// <summary>
        /// Displays a dialog with the given title and message.
        /// </summary>
        /// <param name="title">the title.</param>
        /// <param name="message">the message.</param>
        public static void Alert(string title, string message) => EditorUtility.DisplayDialog(title, message, GPGSStrings.Ok);

        /// <summary>
        /// Gets the android sdk path.
        /// </summary>
        /// <returns>The android sdk path.</returns>
        public static string GetAndroidSdkPath()
        {
            var path = EditorPrefs.GetString("AndroidSdkRoot");
#if UNITY_2019_1_OR_NEWER
            // Unity 2019.x added installation of the Android SDK in the AndroidPlayer directory
            // so fallback to searching for it there.
            if (string.IsNullOrEmpty(path) || EditorPrefs.GetBool("SdkUseEmbedded")) {
                var player = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None);
                if (!string.IsNullOrEmpty(player)) {
                    var sdk = Path.Combine(player, "SDK");
                    if (Directory.Exists(sdk)) {
                        path = sdk;
                    }
                }
            }
#endif
            if (path != null && (path.EndsWith("/") || path.EndsWith("\\"))) {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }

        /// <summary>
        /// Determines if the android sdk exists.
        /// </summary>
        /// <returns><c>true</c> if  android sdk exists; otherwise, <c>false</c>.</returns>
        public static bool HasAndroidSdk()
        {
            var path = GetAndroidSdkPath();
            return path != null && path.Trim() != string.Empty && Directory.Exists(path);
        }

        /// <summary>
        /// Gets the unity major version.
        /// </summary>
        /// <returns>The unity major version.</returns>
        public static int GetUnityMajorVersion()
        {
            var version = 0;
#if UNITY_5
            var major = Application.unityVersion.Split('.')[0];
            int.TryParse(major, out version);
#elif UNITY_4_6
            version = 4;
#endif
            return version;
        }

        /// <summary>
        /// Checks for the android manifest file exsistance.
        /// </summary>
        /// <returns><c>true</c>, if the file exists <c>false</c> otherwise.</returns>
        public static bool AndroidManifestExists() => File.Exists(ManifestPath);

        /// <summary>
        /// Generates the android manifest.
        /// </summary>
        public static void GenerateAndroidManifest()
        {
            var content = ReadEditorTemplate("template-AndroidManifest");
            var extend = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(GPGSProjectSettings.Instance.Get(SERVICEIDKEY))) {
                extend[NEARBY_PERMISSIONS_PLACEHOLDER] = string.Join("\n", new[] {
                        "<!-- Required for Nearby Connections -->\n",
                        "<uses-permission android:name=\"android.permission.BLUETOOTH\" />",
                        "<uses-permission android:name=\"android.permission.BLUETOOTH_ADMIN\" />",
                        "<uses-permission android:name=\"android.permission.ACCESS_WIFI_STATE\" />",
                        "<uses-permission android:name=\"android.permission.CHANGE_WIFI_STATE\" />",
                        "<uses-permission android:name=\"android.permission.ACCESS_COARSE_LOCATION\" />",
                    }.Select(it => $"        {it}")
                );
                extend[SERVICEID_ELEMENT_PLACEHOLDER] = string.Join("\n", new[] {
                        "<!-- Required for Nearby Connections API -->\n",
                        "<meta-data android:name=\"com.google.android.gms.nearby.connection.SERVICE_ID\" android:value=\"__NEARBY_SERVICE_ID__\" />",
                    }.Select(it => $"             {it}")
                );
            } else {
                extend[NEARBY_PERMISSIONS_PLACEHOLDER] = "";
                extend[SERVICEID_ELEMENT_PLACEHOLDER] = "";
            }

            foreach (var entry in s_replacements) {
                var value = GPGSProjectSettings.Instance.Get(entry.Value, extend);
                content = content.Replace(entry.Key, value);
            }

            WriteFile(ManifestPath, content);
            UpdateGameInfo();
        }

        /// <summary>
        /// Writes the resource identifiers file.  This file contains the
        /// resource ids copied (downloaded?) from the play game app console.
        /// </summary>
        /// <param name="classDirectory">Class directory.</param>
        /// <param name="className">Class name.</param>
        /// <param name="resourceKeys">Resource keys.</param>
        public static void WriteResourceIds(string classDirectory, string className, Hashtable resourceKeys)
        {
            if (string.IsNullOrEmpty(classDirectory)) classDirectory = "Assets";
            
            var parts = className.Split('.');
            var constants = string.Empty;
            var @namespace = string.Empty;
            for (var i = 0; i < parts.Length - 1; i += 1) {
                classDirectory += "/" + parts[i];
                if (@namespace != string.Empty) {
                    @namespace += ".";
                }
                @namespace += parts[i];
            }

            EnsureDirExists(classDirectory);
            foreach (DictionaryEntry ent in resourceKeys) {
                var key = MakeIdentifier((string) ent.Key);
                constants += $"        public const string {key} = \"{ent.Value}\"; // <GPGSID>\n";
            }

            var contents = ReadEditorTemplate("template-Constants");
            if (@namespace != string.Empty) {
                contents = contents.Replace(NAMESPACESTARTPLACEHOLDER, "namespace " + @namespace + "\n{");
            } else {
                contents = contents.Replace(NAMESPACESTARTPLACEHOLDER, string.Empty);
            }

            contents = contents.Replace(CLASSNAMEPLACEHOLDER, parts[parts.Length - 1]);
            contents = contents.Replace(CONSTANTSPLACEHOLDER, constants);
            if (@namespace != string.Empty) {
                contents = contents.Replace(NAMESPACEENDPLACEHOLDER, "}");
            } else {
                contents = contents.Replace(NAMESPACEENDPLACEHOLDER, string.Empty);
            }

            var file = Path.Combine(classDirectory, parts[^1] + ".cs");
            WriteFile(file, contents);
        }

        /// <summary>
        /// Updates the game info file.  This is a generated file containing the
        /// app and client ids.
        /// </summary>
        public static void UpdateGameInfo()
        {
            var contents = ReadEditorTemplate("template-GameInfo");
            foreach (var ent in s_replacements) {
                var value = GPGSProjectSettings.Instance.Get(ent.Value);
                contents = contents.Replace(ent.Key, value);
            }
            WriteFile(GameInfoPath, contents);
        }

        /// <summary>
        /// Checks the dependencies file and fixes repository paths
        /// if they are incorrect (for example if the user moved plugin
        /// into some subdirectory). This is a generated file containing
        /// the list of dependencies that are needed for the plugin to work.
        /// </summary>
        public static void CheckAndFixDependencies()
        {
            var dependencies = SlashesToPlatformSeparator(Path.Combine(RootPath, "Editor/GooglePlayGamesPluginDependencies.xml"));

            var xml = new XmlDocument();
            xml.Load(dependencies);

            var repos = xml.SelectNodes("//androidPackage[contains(@spec,'com.google.games')]//repository");
            foreach (XmlNode repo in repos) {
                if (!Directory.Exists(repo.InnerText)) {
                    var pos = repo.InnerText.IndexOf(RootFolderName);
                    if (pos != -1) {
                        var relative = repo.InnerText.Substring(pos + RootFolderName.Length + 1);
                        repo.InnerText = Path.Combine(RootPath, relative).Replace("\\", "/");
                    }
                }
            }

            xml.Save(dependencies);
        }

        /// <summary>
        /// Checks the file containing the list of versioned assets and fixes
        /// paths to them if they are incorrect (for example if the user moved
        /// plugin into some subdirectory). This is a generated file.
        /// </summary>
        public static void CheckAndFixVersionedAssestsPaths()
        {
            var versions = Directory.GetFiles(RootPath, "GooglePlayGamesPlugin_v*.txt", SearchOption.AllDirectories);

            if (versions.Length == 1) {
                var temporal = Path.GetTempFileName();
                using (var sw = new StreamWriter(temporal)) {
                    using (var sr = new StreamReader(versions[0])) {
                        string line;
                        while ((line = sr.ReadLine()) != null) {
                            var index = line.IndexOf(RootFolderName);
                            if (index != -1) {
                                var relative = line.Substring(index + RootFolderName.Length + 1);
                                line = Path.Combine(RootPath, relative).Replace("\\", "/");
                            }
                            sw.WriteLine(line);
                        }
                    }
                    sw.Flush();
                }

                try {
                    File.Copy(temporal, versions[0], true);
                } finally {
                    File.Delete(temporal);
                }
            }
        }

        /// <summary>
        /// Ensures the dir exists.
        /// </summary>
        /// <param name="dir">Directory to check.</param>
        public static void EnsureDirExists(string dir)
        {
            dir = SlashesToPlatformSeparator(dir);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// Deletes the dir if exists.
        /// </summary>
        /// <param name="dir">Directory to delete.</param>
        public static void DeleteDirIfExists(string dir)
        {
            dir = SlashesToPlatformSeparator(dir);
            if (Directory.Exists(dir)) {
                Directory.Delete(dir, true);
            }
        }

        /// <summary>
        /// Gets the Google Play Services library version.  This is only
        /// needed for Unity versions less than 5.
        /// </summary>
        /// <returns>The GPS version.</returns>
        /// <param name="libProjPath">Lib proj path.</param>
        private static int GetGPSVersion(string libProjPath)
        {
            var path = libProjPath + "/res/values/version.xml";
            using var reader = new XmlTextReader(new StreamReader(path));

            var resource = false;
            var version = -1;

            while (reader.Read()) {
                if (reader.Name == "resources") {
                    resource = true;
                }
                if (resource && reader.Name == "integer") {
                    if ("google_play_services_version".Equals(reader.GetAttribute("name"))) {
                        reader.Read();
                        Debug.Log("Read version string: " + reader.Value);
                        version = Convert.ToInt32(reader.Value);
                    }
                }
            }
            return version;
        }

    }

}
