// <copyright file="AndroidTokenClient.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc.
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
//  limitations under the License.
// </copyright>

#if UNITY_ANDROID

using GooglePlayGames.OurUtils;

using JC = GooglePlayGames.JavaClass;

using JAEI = GooglePlayGames.Android.Java.ApiException.Instance;
using JAI  = GooglePlayGames.Android.Java.Activity.Instance;
using JHFC = GooglePlayGames.Android.Java.HelperFragment.Class;
using JEI  = GooglePlayGames.Android.Java.Exception.Instance;
using JTI  = GooglePlayGames.Android.Java.Task.Instance;
using JVI  = GooglePlayGames.Android.Java.View.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class HelperFragment {

        public static readonly string ClassName               =  "HelperFragment";
        public static readonly string PackageName             =  "com.google.games.bridge";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JHFC MakeClass() => new();

        public static JVI  JGetDecorView         (JAI  jActivity                ) => JHFC.Instance.JGetDecorView         (jActivity            );
        public static bool  IsResolutionRequired (JAEI jException               ) => JHFC.Instance.IsResolutionRequired  (jException           );
        public static JTI  JShowAchievementUi    (JAI  jActivity                ) => JHFC.Instance.JShowAchievementUi    (jActivity            );
        public static JTI  JShowAllLeaderboardsUi(JAI  jActivity                ) => JHFC.Instance.JShowAllLeaderboardsUi(jActivity            );
        public static JTI  JShowLeaderboardUi    (JAI  jActivity                ) => JHFC.Instance.JShowLeaderboardUi    (jActivity            );
        public static JTI  JShowLeaderboardUi    (JAI  jActivity, JEI jException) => JHFC.Instance.JShowLeaderboardUi    (jActivity, jException);
        public static void  ShowCaptureOverlayUi (JAI  jActivity                ) => JHFC.Instance.ShowCaptureOverlayUi  (jActivity            );

        internal sealed class Class : JC {

            private static JHFC s_instance = null;

            public static JHFC Instance => s_instance ??= new JHFC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JVI JGetDecorView(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.getDecorView(Activity)");
                return CallStatic<JVI>("getDecorView", jActivity);
            }

            public bool IsResolutionRequired(JAEI jException)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.isResolutionRequired(Exception)");
                return CallStatic<bool>("isResolutionRequired", jException);
            }

            public JTI JShowAchievementUi(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.showAchievementUi(Activity)");
                return CallStatic<JTI>("showAchievementUi", jActivity);
            }

            public JTI JShowAllLeaderboardsUi(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.showAllLeaderboardsUi(Activity)");
                return CallStatic<JTI>("showAllLeaderboardsUi", jActivity);
            }

            public JTI JShowLeaderboardUi(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.showLeaderboardUi(Activity)");
                return CallStatic<JTI>("showLeaderboardUi", jActivity);
            }

            public JTI JShowLeaderboardUi(JAI jActivity, JEI jException)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.askForLoadFriendsResolution(Activity)");
                return CallStatic<JTI>("askForLoadFriendsResolution", jActivity, jException);
            }

            public void ShowCaptureOverlayUi(JAI jActivity)
            {
                Logger.t($"JNI: Call {FullyQualifiedClassName}.showCaptureOverlayUi(Activity)");
                CallStatic("showCaptureOverlayUi", jActivity);
            }

        }

    }

}

#endif