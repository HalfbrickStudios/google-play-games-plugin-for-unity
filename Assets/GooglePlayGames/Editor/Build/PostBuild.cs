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

using UnityEditor;
using UnityEditor.Callbacks;

using static GooglePlayGames.Editor.Strings;
using static GooglePlayGames.Editor.Utils;
using static GooglePlayGames.Editor.UI.Utils;

namespace GooglePlayGames.Editor.Build {

    public static class GpgEditorBuildCheck {

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