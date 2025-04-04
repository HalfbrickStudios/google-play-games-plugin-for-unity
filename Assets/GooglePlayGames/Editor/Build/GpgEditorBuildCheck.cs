// <copyright file="GPGSPostBuild.cs" company="Google Inc.">
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

using UnityEditor;
using UnityEditor.Callbacks;

using static GooglePlayGames.Editor.GpgUtils;

namespace GooglePlayGames.Editor.Build {

    public static class GpgEditorBuildCheck {

        [PostProcessBuild(99999)]
        private static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var done = GPGSProjectSettings.Instance.GetBool(KEY_ANDROID_SETUP_DONE, false);
            if (done) return;
            
            var title = "Google Play Games not configured!";
            var message = "Warning!!  Google Play Games was not configured, Game Services will not work correctly.";
            GpgUtils.Alert(title, message);
        }

    }

}

#endif