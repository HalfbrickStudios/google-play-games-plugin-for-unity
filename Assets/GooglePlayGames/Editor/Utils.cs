// <copyright file="GpgEditorUtils.cs" company="Google Inc.">
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using UnityEditor;
using UnityEngine;

using Google;
using GooglePlayGames.Config;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace GooglePlayGames.Editor {

    public static class Utils {

        public   const string KEY_ANDROID_BUNDLE_ID  = "and.BundleId";
        public   const string KEY_ANDROID_RESOURCE   = "and.ResourceData";
        public   const string KEY_ANDROID_SETUP_DONE = "android.SetupDone";
        public   const string KEY_APP_ID             = "proj.AppId";
        public   const string KEY_CLASS_DIRECTORY    = "proj.classDir";
        public   const string KEY_CLASS_NAME         = "proj.ConstantsClassName";
        internal const string KEY_LAST_UPGRADE       = "lastUpgrade";
        public   const string KEY_NEARBY_SETUP_DONE  = "android.NearbySetupDone";
        public   const string KEY_PLUGIN_VERSION     = "proj.pluginVersion";
        public   const string KEY_SERVICE_ID         = "App.NearbdServiceId";
        public   const string KEY_WEB_CLIENT_ID      = "and.ClientId";

        private const string PLACEHOLDER_APP_ID              = "__APP_ID__";
        private const string PLACEHOLDER_CLASS_NAME          = "__Class__";
        private const string PLACEHOLDER_CONSTANTS           = "__Constant_Properties__";
        private const string PLACEHOLDER_NAMESPACE_END       = "__NameSpaceEnd__";
        private const string PLACEHOLDER_NAMESPACE_START     = "__NameSpaceStart__";
        private const string PLACEHOLDER_NEARBY_PERMISSIONS  = "__NEARBY_PERMISSIONS__";
        private const string PLACEHOLDER_PLUGIN_VERSION      = "__PLUGIN_VERSION__";
        private const string PLACEHOLDER_SERVICE_ID          = "__NEARBY_SERVICE_ID__";
        private const string PLACEHOLDER_SERVICE_ID_ELEMENT  = "__NEARBY_SERVICE_ELEMENT__";
        private const string PLACEHOLDER_WEB_CLIENT_ID       = "__WEB_CLIENTID__";

        private const string GAME_INFO_RELATIVE_PATH = "Runtime/Scripts/GameInfo.cs";
        private const string MANIFEST_RELATIVE_PATH  = "../../Plugins/Android/GooglePlayGamesManifest.androidlib/AndroidManifest.xml";
        private const string ROOT_DIRECTORY_NAME     = "GooglePlayGames";
        private const string ROOT_PACKAGE_NAME       = "com.halfbrick.google.play-games";

        private static string s_rootPath = string.Empty;

        private static readonly Dictionary<string, string> s_replacements = new() {
            {PLACEHOLDER_SERVICE_ID_ELEMENT, PLACEHOLDER_SERVICE_ID_ELEMENT}, // Put this element placeholder first, since it has embedded placeholder
            {PLACEHOLDER_SERVICE_ID,         KEY_SERVICE_ID},
            {PLACEHOLDER_APP_ID,             KEY_APP_ID},
            {PLACEHOLDER_CLASS_NAME,         KEY_CLASS_NAME},
            {PLACEHOLDER_WEB_CLIENT_ID,      KEY_WEB_CLIENT_ID},
            {PLACEHOLDER_PLUGIN_VERSION,     KEY_PLUGIN_VERSION},
            {PLACEHOLDER_NEARBY_PERMISSIONS, PLACEHOLDER_NEARBY_PERMISSIONS} // Causes the placeholder to be replaced with overridden value at runtime
        };

        private static string GameInfoPath => SlashesToPlatformSeparator(Path.Combine(RootPath, GAME_INFO_RELATIVE_PATH));
        private static string ManifestPath => SlashesToPlatformSeparator(Path.Combine(RootPath, MANIFEST_RELATIVE_PATH));

        private static string RootPathInternal {
            get => s_rootPath;
            set {
                if (value.Contains(ROOT_DIRECTORY_NAME + '@')) {
                    s_rootPath = value.Replace("Packages", "Library/PackageCache");
                } else {
                    s_rootPath = value;
                }
            }
        }

        public static string RootPath {
            get {
                if (!string.IsNullOrEmpty(RootPathInternal)) return RootPathInternal;

                var package = PackageInfo.FindForAssetPath("Packages/" + ROOT_PACKAGE_NAME);
                if (!string.IsNullOrEmpty(package?.resolvedPath)) {
                    return RootPathInternal = SlashesToPlatformSeparator(package.resolvedPath);
                }

                var caches   = Directory.GetDirectories("Library/PackageCache", $"{ROOT_PACKAGE_NAME}*", SearchOption.TopDirectoryOnly);
                var packages = Directory.GetDirectories("Packages", ROOT_PACKAGE_NAME, SearchOption.TopDirectoryOnly);
                var combined = caches.Concat(packages);
#if GOOGLE_PLAY_GAMES_PROJECT
                var assets   = Directory.GetDirectories("Assets", ROOT_DIRECTORY_NAME, SearchOption.TopDirectoryOnly);
                combined     = combined.Concat(assets);
#endif
                var matches = combined.ToList();
                switch (matches.Count) {
                    case 0:
                        // Error(0x141, "cannot find the root path of the package");
                        throw new Exception($"Not a single directory named {ROOT_DIRECTORY_NAME} or {ROOT_PACKAGE_NAME} was found");
                    case 1:
                        RootPathInternal = SlashesToPlatformSeparator(matches.First());
                        break;
                    default:
                        foreach (var it in matches) {
                            var info = SlashesToPlatformSeparator(Path.Combine(it, GAME_INFO_RELATIVE_PATH));
                            if (File.Exists(info)) {
                                RootPathInternal = SlashesToPlatformSeparator(it);
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(RootPathInternal)) {
                            // Error(0x142, "cannot find the root path of the package");
                            throw new Exception($"Within the listed packages, not a single directory named {ROOT_DIRECTORY_NAME} was found");
                        }
                        break;
                }
                return RootPathInternal;
            }
        }

        internal static bool AndroidManifestExists() => File.Exists(ManifestPath);

        public static void CheckAndFixDependencies()
        {
            var dependencies = SlashesToPlatformSeparator(Path.Combine(RootPath, "Editor/GooglePlayGamesDependencies.xml"));

            var xml = new XmlDocument();
            xml.Load(dependencies);

            var repos = xml.SelectNodes("//androidPackage[contains(@spec,'com.google.games')]//repository");
            foreach (XmlNode repo in repos) {
                if (!Directory.Exists(repo.InnerText)) {
                    var pos = repo.InnerText.IndexOf(ROOT_DIRECTORY_NAME);
                    if (pos != -1) {
                        var relative = repo.InnerText.Substring(pos + ROOT_DIRECTORY_NAME.Length + 1);
                        repo.InnerText = Path.Combine(RootPath, relative).Replace("\\", "/");
                    }
                }
            }

            xml.Save(dependencies);
        }

        public static void CheckAndFixVersionedAssestsPaths()
        {
            var versions = Directory.GetFiles(RootPath, "GooglePlayGamesPlugin_v*.txt", SearchOption.AllDirectories);

            if (versions.Length == 1) {
                var temporal = Path.GetTempFileName();
                using (var sw = new StreamWriter(temporal)) {
                    using (var sr = new StreamReader(versions[0])) {
                        string line;
                        while ((line = sr.ReadLine()) != null) {
                            var index = line.IndexOf(ROOT_DIRECTORY_NAME);
                            if (index != -1) {
                                var relative = line.Substring(index + ROOT_DIRECTORY_NAME.Length + 1);
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

        public static void EnableExternalDependencyResolverFlags(bool? enable = null, bool? verbose = null)
        {
            if (enable  != null) VersionHandler.Enabled               = (bool)enable;
            if (verbose != null) VersionHandler.VerboseLoggingEnabled = (bool)verbose;
        }

        public static void EnsureDirExists(string dir)
        {
            dir = SlashesToPlatformSeparator(dir);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }

        private static string GetAndroidSdkPath()
        {
            var path = EditorPrefs.GetString("AndroidSdkRoot");
            if (string.IsNullOrEmpty(path) || EditorPrefs.GetBool("SdkUseEmbedded")) {
                var player = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None);
                if (!string.IsNullOrEmpty(player)) {
                    var sdk = Path.Combine(player, "SDK");
                    if (Directory.Exists(sdk)) {
                        path = sdk;
                    }
                }
            }
            if (path != null && (path.EndsWith("/") || path.EndsWith("\\"))) {
                path = path[..^1];
            }
            return path;
        }

        public static bool HasAndroidSdk()
        {
            var path = GetAndroidSdkPath();
            return path != null && path.Trim() != string.Empty && Directory.Exists(path);
        }

        public static bool LooksLikeValidClientId(string s) => s.EndsWith(".googleusercontent.com");

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

        public static bool LooksLikeValidAppId(string s)
        {
            if (s.Length < 5) return false;
            foreach (var c in s) {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }

        public static bool LooksLikeValidServiceId(string s)
        {
            if (s.Length < 3) return false;
            foreach (var c in s) {
                if (!char.IsLetterOrDigit(c) && c != '.') return false;
            }
            return true;
        }

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

        private static string ReadFile(string path)
        {
            path = SlashesToPlatformSeparator(path);
            if (!File.Exists(path)) {
                // Error(0x143, "file not found @ " + path);
                return null;
            }
            using var sr = new StreamReader(path);
            return sr.ReadToEnd();
        }

        private static string ReadEditorTemplate(string name)
        {
            var path = Path.Combine(RootPath, "Editor", "Templates", $"{name}.template");
            Debug.Log("Reading template file: " + path);
            return ReadFile(path);
        }

        public static object ResolveExternalDependencies()
        {
            var assembly = "Google.JarResolver";
            var klass = "GooglePlayServices.PlayServicesResolver";
            var method = "MenuResolve";
            return VersionHandler.InvokeStaticMethod(VersionHandler.FindClass(assembly, klass), method, null);
        }

        internal static string SlashesToPlatformSeparator(string path) => path.Replace('/', Path.DirectorySeparatorChar);

        public static void UpdateExternalDependencyResolverAssets(bool force = true)
        {
            VersionHandler.UpdateVersionedAssets(forceUpdate: force);
        }

        public static void UpdateGameInfo()
        {
            var info = GameInformation.Instance;
            if (info == null) {
                Debug.Log("GPG: Creating GameInformation asset");
                EnsureDirExists("Assets/Resources");
                info = GameInformation.CreateAsset();
                return;
            }
            if (info == null) {
                Debug.LogError("GPG: Failed to read/create the GameInformation asset");
                return;
            }
            var appId = ProjectSettings.Instance.Get(KEY_APP_ID);
            var svcId = ProjectSettings.Instance.Get(KEY_SERVICE_ID);
            var webId = ProjectSettings.Instance.Get(KEY_WEB_CLIENT_ID);
            var changed = info.AppId != appId || info.NearbyId != svcId || info.WebId != webId;
            if (changed) {
                info.AppId    = appId;
                info.NearbyId = svcId;
                info.WebId    = webId;
                EditorUtility.SetDirty(info);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private static void WriteFile(string file, string body)
        {
            file = SlashesToPlatformSeparator(file);
            var dir = Directory.GetParent(file);
            dir.Create();
            using var wr = new StreamWriter(file, false);
            wr.Write(body);
        }

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

            var contents = ReadEditorTemplate("Constants.cs");
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

    }

}

#endif