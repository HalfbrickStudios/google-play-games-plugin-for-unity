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
using GooglePlayGames.Utils;

using Logger = GooglePlayGames.Utils.Logger;

using UAJO = UnityEngine.AndroidJavaObject;

using ALTS = GooglePlayGames.Api.LeaderboardTimeSpan;
using AUS  = GooglePlayGames.Api.UiStatus;
using ASUS = GooglePlayGames.Api.SavedGame.SelectUiStatus;
using AISGM = GooglePlayGames.Api.SavedGame.ISavedGameMetadata;
using ASM = GooglePlayGames.Android.SnapshotMetadata;

using JC = GooglePlayGames.Android.JavaClass;
using JO = GooglePlayGames.Android.JavaObject;

using JAI  = GooglePlayGames.Android.Java.Activity.Instance;
using JEI  = GooglePlayGames.Android.Java.Exception.Instance;
using JHFC = GooglePlayGames.Android.Java.HelperFragment.Class;
using JHF = GooglePlayGames.Android.Java.HelperFragment;
using JT   = GooglePlayGames.Android.Java.Task;
using JUP  = GooglePlayGames.Android.Java.UnityPlayer;
using JSMI = GooglePlayGames.Android.Java.SnapshotMetadata.Instance;


namespace GooglePlayGames.Android.Java {

    internal static class HelperFragment {

        public static readonly string ClassName               =  "HelperFragment";
        public static readonly string PackageName             =  "com.google.games.bridge";
        public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

        internal static class Request
        {

            public static readonly string ClassName = $"{JHF.ClassName}$PlayerFriendStatus";
            public static readonly string FullyQualifiedClassName = $"{PackageName}.{ClassName}";

            public static Instance MakeInstance() => new();
            public static Instance WrapInstance(UAJO jObject) => new(jObject.GetRawObject());

            internal sealed class Instance : JO
            {
                internal Instance() : base(FullyQualifiedClassName)
                {
                    Logger.t($"JNI: Initializing class {FullyQualifiedClassName}");
                }

                internal Instance(IntPtr ptr) : base(ptr)
                {
                    Logger.t($"JNI: Wrapping instance {FullyQualifiedClassName}");
                }

                public JSMI JMetadata
                {
                    get
                    {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.JMetadata");
                        return GetStatic<JSMI>("metadata");
                    }
                }     
                
                public int Status
                {
                    get
                    {
                        Logger.t($"JNI: Reading {FullyQualifiedClassName}.Status");
                        return GetStatic<int>("status");
                    }
                }

            }

        }


        public static JHFC MakeClass() => new();

        public static void              AskForLoadFriendsResolution                 (JAI         jActivity, UAJO          jIntent,           Action<AUS> callback                                                     ) => JHFC.Instance.AskForLoadFriendsResolution                  (jActivity,  jIntent,           callback                             );
        public static void              AskForLoadFriendsResolution                 (UAJO        jIntent,   Action<AUS>   callback                                                                                    ) => JHFC.Instance.AskForLoadFriendsResolution                  (jIntent,    callback                                                );
        public static JT.Instance<int> JAskForLoadFriendsResolution                 (UAJO        jIntent                                                                                                              ) => JHFC.Instance.JAskForLoadFriendsResolution                 (jIntent                                                             );
        public static JT.Instance<int> JAskForLoadFriendsResolution                 (JAI         jActivity,   UAJO        jIntent                                                                                     ) => JHFC.Instance.JAskForLoadFriendsResolution                 (jActivity,  jIntent                                                 );
        public static bool              IsResolutionRequired                        (JEI         jException                                                                                                           ) => JHFC.Instance.IsResolutionRequired                         (jException                                                          );
        public static void              IsResolutionRequired                        (JEI         jException, Action<bool> callback                                                                                    ) => JHFC.Instance.IsResolutionRequired                         (jException, callback                                                );
        public static void              ShowAchievementsUi                          (Action<AUS> callback                                                                                                             ) => JHFC.Instance.ShowAchievementsUi                           (callback                                                            );
        public static void              ShowAchievementsUi                          (JAI         jActivity,  Action<AUS>  callback                                                                                    ) => JHFC.Instance.ShowAchievementsUi                           (jActivity,  callback                                                );
        public static JT.Instance<int> JShowAchievementUi                           (                                                                                                                                 ) => JHFC.Instance.JShowAchievementUi                           (                                                                    );
        public static JT.Instance<int> JShowAchievementUi                           (JAI         jActivity                                                                                                            ) => JHFC.Instance.JShowAchievementUi                           (jActivity                                                           );
        public static void              ShowAllLeaderboardsUi                       (Action<AUS> callback                                                                                                             ) => JHFC.Instance.ShowAllLeaderboardsUi                        (callback                                                            );
        public static void              ShowAllLeaderboardsUi                       (JAI         jActivity,  Action<AUS>  callback                                                                                    ) => JHFC.Instance.ShowAllLeaderboardsUi                        (jActivity,  callback                                                );
        public static JT.Instance<int> JShowAllLeaderboardsUi                       (                                                                                                                                 ) => JHFC.Instance.JShowAllLeaderboardsUi                       (                                                                    );
        public static JT.Instance<int> JShowAllLeaderboardsUi                       (JAI         jActivity                                                                                                            ) => JHFC.Instance.JShowAllLeaderboardsUi                       (jActivity                                                           );
        public static void              ShowLeaderboardUi                           (string      id,         ALTS         span,              Action<AUS> callback                                                     ) => JHFC.Instance.ShowLeaderboardUi                            (id,         span,              callback                             );
        public static void              ShowLeaderboardUi                           (JAI         jActivity,  string       id,                ALTS        span,              Action<AUS> callback                      ) => JHFC.Instance.ShowLeaderboardUi                            (jActivity,  id,                span,              callback          );
        public static JT.Instance<int> JShowLeaderboardUi                           (string      id,         int          span                                                                                        ) => JHFC.Instance.JShowLeaderboardUi                           (id,         span                                                    );
        public static JT.Instance<int> JShowLeaderboardUi                           (JAI         jActivity,  string       id,                int         span                                                         ) => JHFC.Instance.JShowLeaderboardUi                           (jActivity,  id,                span                                 );
        public static void             ShowCompareProfileWithAlternativeNameHintsUi (string      userId,     string       comparandUserName, string      userName,          Action<AUS> callback                      ) => JHFC.Instance.ShowCompareProfileWithAlternativeNameHintsUi (userId,     comparandUserName, userName,          callback          );
        public static void             ShowCompareProfileWithAlternativeNameHintsUi (JAI         jActivity,  string       userId,            string      comparandUserName, string      userName, Action<AUS> callback) => JHFC.Instance.ShowCompareProfileWithAlternativeNameHintsUi (jActivity,  userId,            comparandUserName, userName, callback);
        public static JT.Instance<int> JShowCompareProfileWithAlternativeNameHintsUi(string      userId,     string       comparandUserName, string      userName                                                     ) => JHFC.Instance.JShowCompareProfileWithAlternativeNameHintsUi(userId,     comparandUserName, userName                             );
        public static JT.Instance<int> JShowCompareProfileWithAlternativeNameHintsUi(JAI         jActivity,  string       userId,            string      comparandUserName, string      userName                      ) => JHFC.Instance.JShowCompareProfileWithAlternativeNameHintsUi(jActivity,  userId,            comparandUserName, userName          );
        public static JT.Instance<Request.Instance> JShowSelectSnapshotUi(JAI jActivity, string title, bool showCreate, bool showDelete, int limit) => JHFC.Instance.JShowSelectSnapshotUi(jActivity, title, showCreate, showDelete, limit);

        public static void ShowSelectSnapshotUi(string title, bool showCreate, bool showDelete, int limit, Action<ASUS, AISGM> callback)
        {
            JHFC.Instance.ShowSelectSnapshotUi(title, showCreate, showDelete, limit, callback);
        }

        public static void ShowSelectSnapshotUi(JAI jActivity, string title, bool showCreate, bool showDelete, int limit, Action<ASUS, AISGM> callback)
        {
            JHFC.Instance.ShowSelectSnapshotUi(jActivity, title, showCreate, showDelete, limit, callback);
        }


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

            public bool IsResolutionRequired(JEI jException)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.isResolutionRequired(Exception)");
                return CallStatic<bool>("isResolutionRequired", jException);
            }

            public JT.Instance<int> JShowAchievementUi(JAI jActivity)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.showAchievementUi(Activity)");
                return CallStatic<JT.Instance<int>>("showAchievementUi", jActivity);
            }

            public JT.Instance<int> JShowAllLeaderboardsUi(JAI jActivity)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.showAllLeaderboardsUi(Activity)");
                return CallStatic<JT.Instance<int>>("showAllLeaderboardsUi", jActivity);
            }

            public JT.Instance<int> JShowLeaderboardUi(JAI jActivity, string id, int span)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.showLeaderboardUi(Activity)");
                return CallStatic<JT.Instance<int>>("showLeaderboardUi", jActivity, id, span);
            }

            public JT.Instance<int> JShowCompareProfileWithAlternativeNameHintsUi(JAI jActivity, string userId, string comparandUserName, string userName)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.showCompareProfileWithAlternativeNameHintsUi(Activity, string, string, string)");
                return CallStatic<JT.Instance<int>>("showCompareProfileWithAlternativeNameHintsUi", jActivity, userId, comparandUserName, userName);
            }

            public JT.Instance<Request.Instance> JShowSelectSnapshotUi(JAI jActivity, string title, bool showCreate, bool showDelete, int limit)
            {
                Logger.t($"JNI: Calling {FullyQualifiedClassName}.showSelectSnapshotUi(Activity, string, bool, bool, int)");
                return CallStatic<JT.Instance<Request.Instance>>("showSelectSnapshotUi", jActivity, title, showCreate, showDelete, limit);
            }
        }
    }

}

namespace GooglePlayGames.Android.Java.Extensions {

    internal static class HelperFragmentExtensions {

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

        public static void IsResolutionRequired(this JHFC self, JEI jException, Action<bool> callback)
        {
            var isResolutionRequired = self.IsResolutionRequired(jException);
            callback?.Invoke(isResolutionRequired);
        }

        public static JT.Instance<int> JShowAchievementUi(this JHFC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JShowAchievementUi(jActivity);
        }

        public static void ShowAchievementsUi(this JHFC self, JAI jActivity, Action<AUS> callback)
        {
            using var jTask = self.JShowAchievementUi(jActivity);
            jTask.JAddOnSuccessListener(code => {
                callback?.Invoke((AUS)code);
            }).JAddOnFailureListener(jException => {
                callback?.Invoke(AUS.InternalError);
            });
        }

        public static void ShowAchievementsUi(this JHFC self, Action<AUS> callback)
        {
            using var jActivity = JUP.JCurrentActivity;
            self.ShowAchievementsUi(jActivity, callback);
        }

        public static JT.Instance<int> JShowAllLeaderboardsUi(this JHFC self)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JShowAllLeaderboardsUi(jActivity);
        }

        public static void ShowAllLeaderboardsUi(this JHFC self, Action<AUS> callback)
        {
            using var jActivity = JUP.JCurrentActivity;
            self.ShowAllLeaderboardsUi(jActivity, callback);
        }

        public static void ShowAllLeaderboardsUi(this JHFC self, JAI jActivity, Action<AUS> callback)
        {
            using var jTask = self.JShowAllLeaderboardsUi(jActivity);
            jTask.JAddOnSuccessListener(code => {
                callback?.Invoke((AUS)code);
            }).JAddOnFailureListener(jException => {
                callback?.Invoke(AUS.InternalError);
            });
        }

        public static JT.Instance<int> JShowCompareProfileWithAlternativeNameHintsUi(this JHFC self, string userId, string comparandUserName, string userName)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JShowCompareProfileWithAlternativeNameHintsUi(jActivity, userId, comparandUserName, userName);
        }

        public static void ShowCompareProfileWithAlternativeNameHintsUi(this JHFC self, string userId, string comparandUserName, string userName, Action<AUS> callback)
        {
            using var jActivity = JUP.JCurrentActivity;
            self.ShowCompareProfileWithAlternativeNameHintsUi(userId, comparandUserName, userName, callback);
        }

        public static void ShowCompareProfileWithAlternativeNameHintsUi(this JHFC self, JAI jActivity, string userId, string comparandUserName, string userName, Action<AUS> callback)
        {
            using var jTask = self.JShowCompareProfileWithAlternativeNameHintsUi(jActivity, userId, comparandUserName, userName);
            jTask.JAddOnSuccessListener(code => {
                callback?.Invoke((AUS)code);
            }).JAddOnFailureListener(jException => {
                callback?.Invoke(AUS.InternalError);
            });
        }

        public static JT.Instance<int> JShowLeaderboardUi(this JHFC self, string id, int span)
        {
            using var jActivity = JUP.JCurrentActivity;
            return self.JShowLeaderboardUi(jActivity, id, span);
        }

        public static void ShowLeaderboardUi(this JHFC self, string id, ALTS span, Action<AUS> callback)
        {
            using var jActivity = JUP.JCurrentActivity;
            self.ShowLeaderboardUi(jActivity, id, span, callback);
        }

        public static void ShowLeaderboardUi(this JHFC self, JAI jActivity, string id, ALTS span, Action<AUS> callback)
        {
                  var casted = Utility.ToJavaLeaderboardVariantTimeSpan(span);
            using var jTask  = self.JShowLeaderboardUi(jActivity, id, casted);
            jTask.JAddOnSuccessListener(code => {
                callback?.Invoke((AUS)code);
            }).JAddOnFailureListener(jException => {
                callback?.Invoke(AUS.InternalError);
            });
        }

        public static void ShowSelectSnapshotUi(this JHFC self, string title, bool showCreate, bool showDelete, int limit, Action<ASUS, AISGM> callback)
        {
            using var jActivity = JUP.JCurrentActivity;
            self.JShowSelectSnapshotUi(jActivity, title, showCreate, showDelete, limit);
        }

        public static void ShowSelectSnapshotUi(this JHFC self, JAI jActivity, string title, bool showCreate, bool showDelete, int limit, Action<ASUS, AISGM> callback)
        {
            using var jTask = self.JShowSelectSnapshotUi(jActivity, title, showCreate, showDelete, limit);
            jTask.JAddOnSuccessListener(
                result =>
                {
                    var status = (ASUS)result.Status;
                    using var jMetadata = result.JMetadata;

                    ASM metadata =
                        jMetadata == null
                            ? null
                            : new ASM(jMetadata, jSnapshotContents: null);

                    callback?.Invoke(status, metadata);
                }).JAddOnFailureListener(
                jException =>
                {
                    callback?.Invoke(ASUS.InternalError, null);
                });
        }

    }

}

#endif