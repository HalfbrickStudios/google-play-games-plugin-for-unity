// <copyright file="GPGSDocsUI.cs" company="Google Inc.">
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

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using static GooglePlayGames.Editor.GpgEditorStrings;
using static GooglePlayGames.Editor.GpgEditorUtils;

namespace GooglePlayGames.Editor.UI {

    internal static class GpgEditorUiDocumentation {

        [MenuItem("Google/Play Games/Documentation/Getting Started Guide...", false, 100)]
        private static void MenuItemGettingStartedGuide() => Application.OpenURL(ExternalLinks.GettingStartedGuideURL);

        [MenuItem("Google/Play Games/Documentation/API...", false, 101)]
        private static void MenuItemPlayGamesServicesAPI() => Application.OpenURL(ExternalLinks.PlayGamesServicesApiURL);

        [MenuItem("Google/Play Games/About/Plugin...", false, 300)]
        private static void MenuItemAbout()
        {
            var version = Version.VersionString;
            var semver = string.Format("0x{0:X8}", Version.VersionInt);
            var message = $"{AboutText}\n\nPlugin version: {version} ({semver})";
            Alert(AboutTitle, message);
        }

        [MenuItem("Google/Play Games/About/License...", false, 301)]
        private static void MenuItemLicense() => EditorUtility.DisplayDialog(LicenseTitle, LicenseText, Ok);

    }

}

#endif