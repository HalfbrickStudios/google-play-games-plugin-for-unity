// <copyright file="GPGSUpgrader.cs" company="Google Inc.">
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

#if UNITY_ANDROID

using UnityEditor;

using static GooglePlayGames.Editor.GpgEditorUtils;

namespace GooglePlayGames.Editor {

    // GPGS upgrader handles performing and upgrade tasks.
    [InitializeOnLoad]
    internal class GpgEditorUpgradeManager {

        static GpgEditorUpgradeManager()
        {
            // TODO: review this build script
            return;

            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            GpgEditorProjectSettings.Instance.Set(KEY_LAST_UPGRADE, Version.VersionKey);
            GpgEditorProjectSettings.Instance.Set(KEY_PLUGIN_VERSION, Version.VersionString);
            GpgEditorProjectSettings.Instance.Save();

            var changed = false;
            if (!AndroidManifestExists()) {
                changed = true;
                GenerateAndroidManifest();
            }
            if (changed) AssetDatabase.Refresh();
        }

    }

}

#endif
