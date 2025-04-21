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

using System;

using GooglePlayGames.Android.Java.Extensions;

using Logger = GooglePlayGames.Utils.Logger;

using UAJO = UnityEngine.AndroidJavaObject;

using AUS = GooglePlayGames.Api.UiStatus;

using JC = GooglePlayGames.Android.JavaClass;

using JAI  = GooglePlayGames.Android.Java.Activity.Instance;
using JEI  = GooglePlayGames.Android.Java.Exception.Instance;
using JHFC = GooglePlayGames.Android.Java.HelperFragment.Class;
using JT   = GooglePlayGames.Android.Java.Task;
using JTI  = GooglePlayGames.Android.Java.Task.Instance;
using JUP  = GooglePlayGames.Android.Java.UnityPlayer;
using JVI  = GooglePlayGames.Android.Java.View.Instance;

namespace GooglePlayGames.Android.Java {

    internal static class HelperFragment {

        public static readonly string ClassName               =  "HelperFragment";
        public static readonly string PackageName             =  "com.google.games.bridge";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        public static JHFC MakeClass() => new();

        public static void              AskForLoadFriendsResolution(JAI  jActivity, UAJO          jIntent, Action<AUS> callback) => JHFC.Instance.AskForLoadFriendsResolution(jActivity, jIntent, callback);
        public static void              AskForLoadFriendsResolution(UAJO jIntent,   Action<AUS>   callback                     ) => JHFC.Instance.AskForLoadFriendsResolution(jIntent, callback);
        public static JT.Instance<int> JAskForLoadFriendsResolution(UAJO jIntent                                               ) => JHFC.Instance.JAskForLoadFriendsResolution(jIntent               );
        public static JT.Instance<int> JAskForLoadFriendsResolution(JAI  jActivity,   UAJO        jIntent                      ) => JHFC.Instance.JAskForLoadFriendsResolution(jActivity,  jIntent   );
        public static JVI              JGetDecorView               (JAI  jActivity                                             ) => JHFC.Instance.JGetDecorView               (jActivity             );
        public static bool              IsResolutionRequired       (JEI  jException                                            ) => JHFC.Instance.IsResolutionRequired        (jException            );
        public static void              IsResolutionRequired       (JEI  jException, Action<bool> callback                     ) => JHFC.Instance.IsResolutionRequired        (jException, callback  );
        public static JTI              JShowAchievementUi          (JAI  jActivity                                             ) => JHFC.Instance.JShowAchievementUi          (jActivity             );
        public static JTI              JShowAllLeaderboardsUi      (JAI  jActivity                                             ) => JHFC.Instance.JShowAllLeaderboardsUi      (jActivity             );
        public static JTI              JShowLeaderboardUi          (JAI  jActivity                                             ) => JHFC.Instance.JShowLeaderboardUi          (jActivity             );
        public static JTI              JShowLeaderboardUi          (JAI  jActivity, JEI           jException                   ) => JHFC.Instance.JShowLeaderboardUi          (jActivity,  jException);
        public static void              ShowCaptureOverlayUi       (JAI  jActivity                                             ) => JHFC.Instance.ShowCaptureOverlayUi        (jActivity             );

        internal sealed class Class : JC {

            private static JHFC s_instance = null;

            public static JHFC Instance => s_instance ??= new JHFC();

            internal Class() : base(FullyQualifiedClassName)
            {
                Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
            }

            public JT.Instance<int> JAskForLoadFriendsResolution(JAI jActivity, UAJO jIntent)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.askForLoadFriendsResolution(Activity, AndroidJavaObject)");
                return CallStatic<JT.Instance<int>>("askForLoadFriendsResolution", jActivity, jIntent);
            }

            public JVI JGetDecorView(JAI jActivity)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.getDecorView(Activity)");
                return CallStatic<JVI>("getDecorView", jActivity);
            }

            public bool IsResolutionRequired(JEI jException)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.isResolutionRequired(Exception)");
                return CallStatic<bool>("isResolutionRequired", jException);
            }

            public JTI JShowAchievementUi(JAI jActivity)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.showAchievementUi(Activity)");
                return CallStatic<JTI>("showAchievementUi", jActivity);
            }

            public JTI JShowAllLeaderboardsUi(JAI jActivity)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.showAllLeaderboardsUi(Activity)");
                return CallStatic<JTI>("showAllLeaderboardsUi", jActivity);
            }

            public JTI JShowLeaderboardUi(JAI jActivity)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.showLeaderboardUi(Activity)");
                return CallStatic<JTI>("showLeaderboardUi", jActivity);
            }

            public JTI JShowLeaderboardUi(JAI jActivity, JEI jException)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.askForLoadFriendsResolution(Activity)");
                return CallStatic<JTI>("askForLoadFriendsResolution", jActivity, jException);
            }

            public void ShowCaptureOverlayUi(JAI jActivity)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.showCaptureOverlayUi(Activity)");
                CallStatic("showCaptureOverlayUi", jActivity);
            }

        }

    }

}

namespace GooglePlayGames.Android.Java.Extensions {

    internal static class HelperFragmentExtensions {

        public static void IsResolutionRequired(this JHFC self, JEI jException, Action<bool> callback)
        {
            var isResolutionRequired = self.IsResolutionRequired(jException);
            callback?.Invoke(isResolutionRequired);
        }

        public static JT.Instance<int> JAskForLoadFriendsResolution(this JHFC self, UAJO jIntent)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JAskForLoadFriendsResolution(jActivity, jIntent);
        }

        public static void AskForLoadFriendsResolution(this JHFC self, JAI jActivity, UAJO jIntent, Action<AUS> callback)
        {
            using var jTask = self.JAskForLoadFriendsResolution(jActivity, jIntent);
            jTask.JAddOnSuccessListener(code => {
                callback?.Invoke((AUS)code);
            }).JAddOnFailureListener(jException => {
                callback?.Invoke(AUS.InternalError);
            });
        }

        public static void AskForLoadFriendsResolution(this JHFC self, UAJO jIntent, Action<AUS> callback)
        {
            using var jActivity = JUP.JCurrentActivity;
            self.AskForLoadFriendsResolution(jActivity, jIntent, callback);
        }

    }

}

#endif