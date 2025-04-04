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

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace GooglePlayGames.Editor {

    /// <summary>
    /// Utility class to perform various tasks in the editor.
    /// </summary>
    public static class GpgUtils {

        /// <summary>Property key for project settings.</summary>
        public const string KEY_SERVICE_ID = "App.NearbdServiceId";

        /// <summary>Property key for project settings.</summary>
        public const string KEY_APP_ID = "proj.AppId";

        /// <summary>Property key for project settings.</summary>
        public const string KEY_CLASS_DIRECTORY = "proj.classDir";

        /// <summary>Property key for project settings.</summary>
        public const string KEY_CLASS_NAME = "proj.ConstantsClassName";

        /// <summary>Property key for project settings.</summary>
        public const string KEY_WEB_CLIENT_ID = "and.ClientId";

        /// <summary>Property key for project settings.</summary>
        public const string KEY_ANDROID_RESOURCE = "and.ResourceData";

        /// <summary>Property key for project settings.</summary>
        public const string KEY_ANDROID_SETUP_DONE = "android.SetupDone";

        /// <summary>Property key for project settings.</summary>
        public const string KEY_ANDROID_BUNDLE_ID = "and.BundleId";

        /// <summary>Property key for plugin version.</summary>
        internal const string KEY_PLUGIN_VERSION = "proj.pluginVersion";

        /// <summary>Property key for nearby settings done.</summary>
        public const string KEY_NEARBY_SETUP_DONE = "android.NearbySetupDone";

        /// <summary>Property key for project settings.</summary>
        internal const string KEY_LAST_UPGRADE = "lastUpgrade";

        /// <summary>Constant for token replacement</summary>
        private const string PLACEHOLDER_SERVICE_ID = "__NEARBY_SERVICE_ID__";

        private const string PLACEHOLDER_SERVICE_ID_ELEMENT = "__NEARBY_SERVICE_ELEMENT__";

        private const string PLACEHOLDER_NEARBY_PERMISSIONS = "__NEARBY_PERMISSIONS__";

        /// <summary>Constant for token replacement</summary>
        private const string PLACEHOLDER_APP_ID = "__APP_ID__";

        /// <summary>Constant for token replacement</summary>
        private const string PLACEHOLDER_CLASS_NAME = "__Class__";

        /// <summary>Constant for token replacement</summary>
        private const string PLACEHOLDER_WEB_CLIENT_ID = "__WEB_CLIENTID__";

        /// <summary>Constant for token replacement</summary>
        private const string PLACEHOLDER_PLUGIN_VERSION = "__PLUGIN_VERSION__";

        /// <summary>Constant for require google plus token replacement</summary>
        private const string PLACEHOLDER_REQUIRE_GOOGLE_PLUS = "__REQUIRE_GOOGLE_PLUS__";

        /// <summary>Property key for project settings.</summary>
        private const string KEY_TOKEN_PERMISSION = "proj.tokenPermissions";

        /// <summary>Constant for token replacement</summary>
        private const string PLACEHOLDER_NAMESPACE_START = "__NameSpaceStart__";

        /// <summary>Constant for token replacement</summary>
        private const string PLACEHOLDER_NAMESPACE_END = "__NameSpaceEnd__";

        /// <summary>Constant for token replacement</summary>
        private const string PLACEHOLDER_CONSTANTS = "__Constant_Properties__";

        /// <summary>
        /// The game info file path, relative to the plugin root directory.
        /// This is a generated file.
        /// </summary>
        private const string GameInfoRelativePath = "Runtime/Scripts/GameInfo.cs";

        /// <summary>
        /// The manifest path, relative to the plugin root directory.
        /// </summary>
        /// <remarks>The Games SDK requires additional metadata in the AndroidManifest.xml file.</remarks>
        private const string ManifestRelativePath = "../../Plugins/Android/GooglePlayGamesManifest.androidlib/AndroidManifest.xml";

        private const string RootFolderName = "GooglePlayGames";

        /// <summary>
        /// The root path of the Google Play Games plugin
        /// </summary>
        private static string s_rootPath = string.Empty;

        /// <summary>
        /// The root path of the Google Play Games plugin
        /// </summary>
        private static string RootPathInternal {
            get => s_rootPath;
            set {
                if (value.Contains(RootFolderName + '@')) {
                    s_rootPath = value.Replace("Packages", "Library/PackageCache");
                }
            }
        }

        /// <summary>
        /// The root path of the Google Play Games plugin
        /// </summary>
        public static string RootPath {
            get {
                if (!string.IsNullOrEmpty(RootPathInternal)) return RootPathInternal;

                var package = PackageInfo.FindForAssetPath("Packages/" + RootFolderName);
                if (!string.IsNullOrEmpty(package?.resolvedPath)) {
                    return RootPathInternal = SlashesToPlatformSeparator(package.resolvedPath);
                }

                var caches = Directory.GetDirectories("Library/PackageCache", $"{RootFolderName}*", SearchOption.TopDirectoryOnly);
                var packages = Directory.GetDirectories("Packages", RootFolderName, SearchOption.TopDirectoryOnly);
                var combined = caches.Concat(packages);
#if GOOGLE_PLAY_GAMES_PROJECT
                var assets = Directory.GetDirectories("Assets", RootFolderName, SearchOption.TopDirectoryOnly);
                combined = combined.Concat(assets);
#endif
                var matches = combined.ToList();
                switch (matches.Count) {
                    case 0:
                        Error(0x141, "cannot find the root path of the package");
                        throw new Exception($"Not a single directory named {RootFolderName} was found");
                    case 1:
                        RootPathInternal = SlashesToPlatformSeparator(matches.First());
                        break;
                    default:
                        foreach (var it in matches) {
                            var info = SlashesToPlatformSeparator(Path.Combine(it, GameInfoRelativePath));
                            if (File.Exists(info)) {
                                RootPathInternal = SlashesToPlatformSeparator(it);
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(RootPathInternal)) {
                            Error(0x142, "cannot find the root path of the package");
                            throw new Exception($"Within the listed packages, not a single directory named {RootFolderName} was found");
                        }
                        break;
                }
                return RootPathInternal;
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
            {PLACEHOLDER_SERVICE_ID_ELEMENT, PLACEHOLDER_SERVICE_ID_ELEMENT},
            {PLACEHOLDER_SERVICE_ID, KEY_SERVICE_ID},
            {PLACEHOLDER_APP_ID, KEY_APP_ID},
            {PLACEHOLDER_CLASS_NAME, KEY_CLASS_NAME},
            {PLACEHOLDER_WEB_CLIENT_ID, KEY_WEB_CLIENT_ID},
            {PLACEHOLDER_PLUGIN_VERSION, KEY_PLUGIN_VERSION},
            // Causes the placeholder to be replaced with overridden value at runtime.
            {PLACEHOLDER_NEARBY_PERMISSIONS, PLACEHOLDER_NEARBY_PERMISSIONS}
        };

        /// <summary>
        /// Replaces / in file path to be the os specific separator.
        /// </summary>
        /// <returns>The path.</returns>
        /// <param name="path">Path with correct separators.</param>
        internal static string SlashesToPlatformSeparator(string path) => path.Replace('/', Path.DirectorySeparatorChar);

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <returns>The file contents.  The slashes are corrected.</returns>
        /// <param name="path">File path.</param>
        private static string ReadFile(string path)
        {
            path = SlashesToPlatformSeparator(path);
            if (!File.Exists(path)) {
                Error(0x143, "file not found @ " + path);
                return null;
            }
            using var sr = new StreamReader(path);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// Reads the editor template.
        /// </summary>
        /// <returns>The editor template contents.</returns>
        /// <param name="name">Name of the template in the editor directory.</param>
        private static string ReadEditorTemplate(string name) => ReadFile(Path.Combine(RootPath, "Editor", $"{name}.txt"));

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="file">File path - the slashes will be corrected.</param>
        /// <param name="body">Body of the file to write.</param>
        private static void WriteFile(string file, string body)
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
        private static bool LooksLikeValidBundleId(string s) => s.Length > 3;

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
        private static bool IsSetupDone()
        {
            var done = true;
#if UNITY_ANDROID
            done = GPGSProjectSettings.Instance.GetBool(KEY_ANDROID_SETUP_DONE, false);
            if (File.Exists(GameInfoPath)) {
                var contents = ReadFile(GameInfoPath);
                if (contents.Contains(PLACEHOLDER_APP_ID)) {
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
        /// Returns a legal C# identifier from the given string.
        /// The transformations are:
        ///   - spaces => underscore _
        ///   - punctuation => empty string
        ///   - leading numbers are prefixed with underscore.
        /// </summary>
        /// <returns>the id</returns>
        /// <param name="key">Key to convert to an identifier.</param>
        private static string MakeIdentifier(string key)
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
        /// Displays a dialog with the given title and message.
        /// </summary>
        /// <param name="title">the title.</param>
        /// <param name="message">the message.</param>
        public static void Alert(string title, string message) => EditorUtility.DisplayDialog(title, message, GpgEditorStrings.Ok);

        /// <summary>
        /// Displays an error dialog.
        /// </summary>
        /// <param name="message">the message</param>
        private static void Alert(string message) => Alert(GpgEditorStrings.Title, message);

        /// <summary>
        /// Displays a dialog with the given title and message.
        /// </summary>
        /// <param name="title">the title.</param>
        /// <param name="message">the message.</param>
        public static void Error(int code, string message)
        {
            var hex = code.ToString("X");
            Alert($"{GpgEditorStrings.Title} Error", $"Code: 0x{hex}\nMessage: {message}");
        }

        /// <summary>
        /// Gets the android sdk path.
        /// </summary>
        /// <returns>The android sdk path.</returns>
        private static string GetAndroidSdkPath()
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
                path = path[..^1];
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
        private static int GetUnityMajorVersion()
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
        internal static bool AndroidManifestExists() => File.Exists(ManifestPath);

        /// <summary>
        /// Generates the android manifest.
        /// </summary>
        public static void GenerateAndroidManifest()
        {
            var content = ReadEditorTemplate("template-AndroidManifest");
            var extend = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(GPGSProjectSettings.Instance.Get(KEY_SERVICE_ID))) {
                extend[PLACEHOLDER_NEARBY_PERMISSIONS] = string.Join("\n", new[] {
                        "<!-- Required for Nearby Connections -->\n",
                        "<uses-permission android:name=\"android.permission.BLUETOOTH\" />",
                        "<uses-permission android:name=\"android.permission.BLUETOOTH_ADMIN\" />",
                        "<uses-permission android:name=\"android.permission.ACCESS_WIFI_STATE\" />",
                        "<uses-permission android:name=\"android.permission.CHANGE_WIFI_STATE\" />",
                        "<uses-permission android:name=\"android.permission.ACCESS_COARSE_LOCATION\" />",
                    }.Select(it => $"        {it}")
                );
                extend[PLACEHOLDER_SERVICE_ID_ELEMENT] = string.Join("\n", new[] {
                        "<!-- Required for Nearby Connections API -->\n",
                        "<meta-data android:name=\"com.google.android.gms.nearby.connection.SERVICE_ID\" android:value=\"__NEARBY_SERVICE_ID__\" />",
                    }.Select(it => $"             {it}")
                );
            } else {
                extend[PLACEHOLDER_NEARBY_PERMISSIONS] = string.Empty;
                extend[PLACEHOLDER_SERVICE_ID_ELEMENT] = string.Empty;
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
                contents = contents.Replace(PLACEHOLDER_NAMESPACE_START, "namespace " + @namespace + "\n{");
            } else {
                contents = contents.Replace(PLACEHOLDER_NAMESPACE_START, string.Empty);
            }

            contents = contents.Replace(PLACEHOLDER_CLASS_NAME, parts[parts.Length - 1]);
            contents = contents.Replace(PLACEHOLDER_CONSTANTS, constants);
            if (@namespace != string.Empty) {
                contents = contents.Replace(PLACEHOLDER_NAMESPACE_END, "}");
            } else {
                contents = contents.Replace(PLACEHOLDER_NAMESPACE_END, string.Empty);
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
        private static void DeleteDirIfExists(string dir)
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
