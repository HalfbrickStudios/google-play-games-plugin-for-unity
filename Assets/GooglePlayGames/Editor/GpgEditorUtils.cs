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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using UnityEditor;
using UnityEngine;

using static GooglePlayGames.Editor.GpgEditorStrings;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace GooglePlayGames.Editor {

    // Utility class to perform various tasks in the editor
    public static class GpgEditorUtils {

        // Property keys for project settings
        public   const string KEY_ANDROID_BUNDLE_ID  = "and.BundleId";
        public   const string KEY_ANDROID_RESOURCE   = "and.ResourceData";
        public   const string KEY_ANDROID_SETUP_DONE = "android.SetupDone";
        public   const string KEY_APP_ID             = "proj.AppId";
        public   const string KEY_CLASS_DIRECTORY    = "proj.classDir";
        public   const string KEY_CLASS_NAME         = "proj.ConstantsClassName";
        internal const string KEY_LAST_UPGRADE       = "lastUpgrade";
        public   const string KEY_NEARBY_SETUP_DONE  = "android.NearbySetupDone";
        internal const string KEY_PLUGIN_VERSION     = "proj.pluginVersion";
        public   const string KEY_SERVICE_ID         = "App.NearbdServiceId";
        private  const string KEY_TOKEN_PERMISSION   = "proj.tokenPermissions";
        public   const string KEY_WEB_CLIENT_ID      = "and.ClientId";

        // Constants for token replacement
        private const string PLACEHOLDER_APP_ID              = "__APP_ID__";
        private const string PLACEHOLDER_CLASS_NAME          = "__Class__";
        private const string PLACEHOLDER_CONSTANTS           = "__Constant_Properties__";
        private const string PLACEHOLDER_NAMESPACE_END       = "__NameSpaceEnd__";
        private const string PLACEHOLDER_NAMESPACE_START     = "__NameSpaceStart__";
        private const string PLACEHOLDER_NEARBY_PERMISSIONS  = "__NEARBY_PERMISSIONS__";
        private const string PLACEHOLDER_PLUGIN_VERSION      = "__PLUGIN_VERSION__";
        private const string PLACEHOLDER_REQUIRE_GOOGLE_PLUS = "__REQUIRE_GOOGLE_PLUS__";
        private const string PLACEHOLDER_SERVICE_ID          = "__NEARBY_SERVICE_ID__";
        private const string PLACEHOLDER_SERVICE_ID_ELEMENT  = "__NEARBY_SERVICE_ELEMENT__";
        private const string PLACEHOLDER_WEB_CLIENT_ID       = "__WEB_CLIENTID__";

        // The game info file path, relative to the plugin root directory
        private const string GameInfoRelativePath = "Runtime/Scripts/GameInfo.cs";

        // The manifest path, relative to the plugin root directory
        private const string ManifestRelativePath = "../../Plugins/Android/GooglePlayGamesManifest.androidlib/AndroidManifest.xml";

        // The name of the package directory
        private const string RootDirectoryName = "GooglePlayGames";

        // Backing field for the root path of the Google Play Games package
        private static string s_rootPath = string.Empty;

        // Internal setter for the Google Play Games package root path
        private static string RootPathInternal {
            get => s_rootPath;
            set {
                if (value.Contains(RootDirectoryName + '@')) {
                    s_rootPath = value.Replace("Packages", "Library/PackageCache");
                }
            }
        }

        // Root path of the Google Play Games package
        public static string RootPath {
            get {
                if (!string.IsNullOrEmpty(RootPathInternal)) return RootPathInternal;

                var package = PackageInfo.FindForAssetPath("Packages/" + RootDirectoryName);
                if (!string.IsNullOrEmpty(package?.resolvedPath)) {
                    return RootPathInternal = SlashesToPlatformSeparator(package.resolvedPath);
                }

                var caches = Directory.GetDirectories("Library/PackageCache", $"{RootDirectoryName}*", SearchOption.TopDirectoryOnly);
                var packages = Directory.GetDirectories("Packages", RootDirectoryName, SearchOption.TopDirectoryOnly);
                var combined = caches.Concat(packages);
#if GOOGLE_PLAY_GAMES_PROJECT
                var assets = Directory.GetDirectories("Assets", RootDirectoryName, SearchOption.TopDirectoryOnly);
                combined = combined.Concat(assets);
#endif
                var matches = combined.ToList();
                switch (matches.Count) {
                    case 0:
                        Error(0x141, "cannot find the root path of the package");
                        throw new Exception($"Not a single directory named {RootDirectoryName} was found");
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
                            throw new Exception($"Within the listed packages, not a single directory named {RootDirectoryName} was found");
                        }
                        break;
                }
                return RootPathInternal;
            }
        }

        // The game info file path
        private static string GameInfoPath
        {
            get => SlashesToPlatformSeparator(Path.Combine(RootPath, GameInfoRelativePath));
        }

        // The manifest path
        private static string ManifestPath
        {
            get => SlashesToPlatformSeparator(Path.Combine(RootPath, ManifestRelativePath));
        }

        // The map of replacements for filling in code templates.
        private static readonly Dictionary<string, string> s_replacements = new() {
            {PLACEHOLDER_SERVICE_ID_ELEMENT, PLACEHOLDER_SERVICE_ID_ELEMENT}, // Put this element placeholder first, since it has embedded placeholder
            {PLACEHOLDER_SERVICE_ID,         KEY_SERVICE_ID},
            {PLACEHOLDER_APP_ID,             KEY_APP_ID},
            {PLACEHOLDER_CLASS_NAME,         KEY_CLASS_NAME},
            {PLACEHOLDER_WEB_CLIENT_ID,      KEY_WEB_CLIENT_ID},
            {PLACEHOLDER_PLUGIN_VERSION,     KEY_PLUGIN_VERSION},
            {PLACEHOLDER_NEARBY_PERMISSIONS, PLACEHOLDER_NEARBY_PERMISSIONS} // Causes the placeholder to be replaced with overridden value at runtime
        };

        // Replaces / in file path to be the OS-specific separator
        internal static string SlashesToPlatformSeparator(string path) => path.Replace('/', Path.DirectorySeparatorChar);

        // Reads a file
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

        // Reads an editor template file
        private static string ReadEditorTemplate(string name) => ReadFile(Path.Combine(RootPath, "Editor", $"{name}.txt"));

        // Writes a file
        private static void WriteFile(string file, string body)
        {
            file = SlashesToPlatformSeparator(file);
            var dir = Directory.GetParent(file);
            dir.Create();
            using var wr = new StreamWriter(file, false);
            wr.Write(body);
        }

        // Checks that a string is a valid Nearby Connections Service Id
        public static bool LooksLikeValidServiceId(string s)
        {
            if (s.Length < 3) return false;
            foreach (var c in s) {
                if (!char.IsLetterOrDigit(c) && c != '.') return false;
            }
            return true;
        }

        // Checks that a string is a valid App Id
        public static bool LooksLikeValidAppId(string s)
        {
            if (s.Length < 5) return false;
            foreach (var c in s) {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }

        // Checks that a string is a valid Client Id
        public static bool LooksLikeValidClientId(string s) => s.EndsWith(".googleusercontent.com");

        // Checks that a string is a valid Bundle Id
        private static bool LooksLikeValidBundleId(string s) => s.Length > 3;

        // Checks that a string is a valid Package Name
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

        // Determines if is setup done
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

        // Constructs a legal identifier from a string
        private static string MakeId(string key)
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

        // Displays a dialog with the given title and message
        public static void Alert(string title, string message) => EditorUtility.DisplayDialog(title, message, Ok);

        // Displays a dialog with the given message
        public static void Alert(string message) => Alert(Title, message);

        // Displays an error dialog with the given code and message
        public static void Error(int code, string message)
        {
            var hex = code.ToString("X");
            Alert($"{Title} Error", $"Code: 0x{hex}\nMessage: {message}");
        }

        // Gets the Android SDK path
        private static string GetAndroidSdkPath()
        {
            var path = EditorPrefs.GetString("AndroidSdkRoot");
#if UNITY_2019_1_OR_NEWER
            // Unity 2019.x added installation of the Android SDK in the AndroidPlayer directory
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

        // Determines if the Android SDK exists
        public static bool HasAndroidSdk()
        {
            var path = GetAndroidSdkPath();
            return path != null && path.Trim() != string.Empty && Directory.Exists(path);
        }

        // Gets the Unity major version
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

        // Checks for the Android Manifest file existence
        internal static bool AndroidManifestExists() => File.Exists(ManifestPath);

        // Generates an Android Manifest file
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

        // Writes the resource identifiers file
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
                var key = MakeId((string) ent.Key);
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

        // Updates the game info file
        public static void UpdateGameInfo()
        {
            var contents = ReadEditorTemplate("template-GameInfo");
            foreach (var ent in s_replacements) {
                var value = GPGSProjectSettings.Instance.Get(ent.Value);
                contents = contents.Replace(ent.Key, value);
            }
            WriteFile(GameInfoPath, contents);
        }

        // Checks the dependencies file and fixes repository paths if they are incorrect
        public static void CheckAndFixDependencies()
        {
            var dependencies = SlashesToPlatformSeparator(Path.Combine(RootPath, "Editor/GooglePlayGamesPluginDependencies.xml"));

            var xml = new XmlDocument();
            xml.Load(dependencies);

            var repos = xml.SelectNodes("//androidPackage[contains(@spec,'com.google.games')]//repository");
            foreach (XmlNode repo in repos) {
                if (!Directory.Exists(repo.InnerText)) {
                    var pos = repo.InnerText.IndexOf(RootDirectoryName);
                    if (pos != -1) {
                        var relative = repo.InnerText.Substring(pos + RootDirectoryName.Length + 1);
                        repo.InnerText = Path.Combine(RootPath, relative).Replace("\\", "/");
                    }
                }
            }

            xml.Save(dependencies);
        }

        // Checks the file containing the list of versioned assets and fixes paths to them if they are incorrect
        public static void CheckAndFixVersionedAssestsPaths()
        {
            var versions = Directory.GetFiles(RootPath, "GooglePlayGamesPlugin_v*.txt", SearchOption.AllDirectories);

            if (versions.Length == 1) {
                var temporal = Path.GetTempFileName();
                using (var sw = new StreamWriter(temporal)) {
                    using (var sr = new StreamReader(versions[0])) {
                        string line;
                        while ((line = sr.ReadLine()) != null) {
                            var index = line.IndexOf(RootDirectoryName);
                            if (index != -1) {
                                var relative = line.Substring(index + RootDirectoryName.Length + 1);
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

        // Ensures a directory exists
        public static void EnsureDirExists(string dir)
        {
            dir = SlashesToPlatformSeparator(dir);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }

        // Deletes a directory if exists
        private static void DeleteDirIfExists(string dir)
        {
            dir = SlashesToPlatformSeparator(dir);
            if (Directory.Exists(dir)) {
                Directory.Delete(dir, true);
            }
        }

        // Gets the Google Play Services library version
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

        // Enables EDM4U flags (formerly Google Play Resolver)
        public static void EnableExternalDependencyResolverFlags(bool? enable = null, bool? verbose = null)
        {
            if (enable  != null) Google.VersionHandler.Enabled               = (bool)enable;
            if (verbose != null) Google.VersionHandler.VerboseLoggingEnabled = (bool)verbose;
        }

        // Updates versioned assets of EDM4U (formerly Google Play Resolver)
        public static void UpdateExternalDependencyResolverAssets(bool force = true)
        {
            Google.VersionHandler.UpdateVersionedAssets(forceUpdate: force);
        }

        // Resolves the dependencies using the EDM4U (formerly Google Play Resolver)
        public static object ResolveExternalDependencies()
        {
            var assembly = "Google.JarResolver";
            var klass = "GooglePlayServices.PlayServicesResolver";
            var method = "MenuResolve";
            return Google.VersionHandler.InvokeStaticMethod(Google.VersionHandler.FindClass(assembly, klass), method, null);
        }

    }

}
