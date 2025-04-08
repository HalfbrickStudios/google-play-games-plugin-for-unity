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

using UnityEditor;

using static GooglePlayGames.Editor.UI.Strings;

namespace GooglePlayGames.Editor.UI {

    public static class Utils {

        public static void Alert(string title, string message) => EditorUtility.DisplayDialog(title, message, Ok);

        public static void Alert(string message) => Alert(Title, message);

        public static void Error(int code, string message)
        {
            var hex = code.ToString("X");
            Alert($"{Title} Error", $"Code: 0x{hex}\nMessage: {message}");
        }

    }

}

#endif