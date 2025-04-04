// <copyright file="GPGSStrings.cs" company="Google Inc.">
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

// Keep the strings all the time even if on an unsupported configuration.

#if UNITY_EDITOR

namespace GooglePlayGames.Editor {

    public static class GpgEditorStrings {

        internal const  string TplAppId  = "Application ID";
        internal const  string TplASL    = "Android Support Library";
        internal const  string TplCID    = "Client ID";
        internal const  string TplGP     = "Google Play";
        internal static string TplGPG    = $"{TplGP} Games";
        internal static string TplGPP    = $"{TplGPG} Plugin";
        internal static string TplGPGP4U = $"{TplGPP} for Unity";
        internal static string TplGPS    = $"{TplGP} Services";
        internal const  string TplLP     = "Library Project";
        internal const  string TplNC     = "Nearby Connections";
        internal static string TplNCS    = $"{TplNC} Service";
        internal const  string TplSDK    = "Android SDK";
        internal static string TplASM    = $"{TplSDK} Manager";
        internal static string TplWACID  = $"Web App {TplCID}";

        public static string Title = TplGPG;

        public const string Cancel  = "Cancel";
        public const string Error   = "Error";
        public const string No      = "No";
        public const string Ok      = "OK";
        public const string Success = "Success";
        public const string Warning = "Warning";
        public const string Yes     = "Yes";

        public static class PostInstall {

            public static string Title = TplGPGP4U;

            public static string Text = $"The {TplGPGP4U} version $VERSION is now ready to use. " +
                                        $"If this is a new installation or if you have just upgraded from a previous version, please click the '{TplGPG}' menu and select 'Android Setup' to set up your project.";

        }

        public static class Setup {

            public static string AppIdTitle = $"{TplGPG} {TplAppId}";

            public static string AppId = TplAppId;

            public static string AppIdBlurb = $"Enter your {TplAppId} below. " +
                                               "This is the numeric identifier provided by the Developer Console (for example, 123456789012).";

            public static string AppIdError = $"The {TplAppId} does not appear to be valid. " +
                                               "It must consist solely of digits, usually 10 or more.";

            public static string WebClientIdTitle = $"{TplWACID} (Optional)";

            public static string ClientId = TplCID;

            public static string ClientIdError = $"The {TplWACID} does not appear to be valid. " +
                                                  "It should end in .apps.googleusercontent.com.";

            public static string AppIdMismatch = $"Web app {TplWACID} not associated with this game!";

            public static string NearbyServiceId = $"{TplNCS} ID";

            public const  string NearbyServiceBlurb = "Enter the service id that identifies the nearby connections service scope";

            public const  string SetupButton = "Setup";

        }

        public static class NearbyConnections {

            public static string Title = $"{TplGPG} - {TplNC} Setup";

            public static string Blurb = $"To configure {TplNC} in this project, please enter the information below and click on the Setup button.";

            public static string SetupComplete = $"{TplNC} configured successfully.";

        }

        public static class AndroidSetup {

            public static string Title = $"{TplGPG} - Android Configuration";

            public static string Blurb = $"To configure {TplGPG} in this project, go to the Play Game console, then enter the information below and click on the Setup button.";

            public static string WebClientIdBlurb = $"The {TplWACID} is needed to access the user's ID token and call other APIs on behalf of the user.  " +
                                                     "It is not required for Game Services.  " + 
                                                     "Enter your OAuth2 Client ID below. " +
                                                     "To obtain this ID, generate a web linked app in Developer Console. " +
                                                     "Example: 123456789012-abcdefghijklm.apps.googleusercontent.com";

            public const  string PkgName = "Package name";

            public const  string PkgNameBlurb = "Enter your application's package name below (for example, com.example.lorem.ipsum).";

            public const  string PackageNameError = "The package name does not appear to be valid. " +
                                                    "Enter a valid Android package name (for example, com.example.lorem.ipsum).";

            public static string SdkNotFound = $"{TplSDK} Not found";

            public static string SdkNotFoundBlurb = $"The {TplSDK} path was not found. " +
                                                     "Please configure it in the Unity preferences window (under External Tools).";

            public static string LibProjNotFound = $"{TplGPS} {TplLP} Not Found";

            public static string LibProjNotFoundBlurb = $"{TplGPS} {TplLP} could not be found your SDK installation. " +
                                                        $"Make sure it is installed (open the {TplASM} and go to Extras, and select {TplGPS}).";

            public static string SupportJarNotFound = $"{TplASL} v4 Not Found";

            public static string SupportJarNotFoundBlurb = $"{TplASL} v4 could not be found your SDK installation. " +
                                                           $"Make sure it is installed (open the {TplASM} and go to Extras, and select '{TplASL}').";

            public static string LibProjVerNotFound = $"The version of your copy of the {TplGPS} {TplLP} could not be determined. " +
                                                       "Please make sure it is at least version {0}. " +
                                                       "Continue?";

            public static string LibProjVerTooOld = $"Your copy of the {TplGPS} {TplLP} is out of date. " +
                                                    $"Please launch the {TplASM} and upgrade your {TplGPS} bundle to the latest version (your version: {{0}}; required version: {{1}}). " +
                                                     "Proceeding may cause problems. " +
                                                     "Proceed anyway?";

            public static string SetupComplete = $"{TplGPG} configured successfully.";

        }

        public static class ExternalLinks {

            public const  string GettingStartedGuideURL = "https://github.com/playgameservices/play-games-plugin-for-unity";

            public const  string PlayGamesServicesApiURL = "https://developers.google.com/games/services";

            public static string GooglePlayGamesAndroidSdkTitle = $"{TplGPS} {TplSDK} Download";

            public static string GooglePlayGamesAndroidSdkBlurb = $"The {TplGPS} SDK for Android must be downloaded via the {TplASM}. " +
                                                                   "Do you wish to start the SDK manager now?";

            public static string GooglePlayGamesAndroidSdkInstructions = $"The {TplASM} will be launched. " +
                                                                         $"Install or upgrade the '{TplGPS}' package, which can be found under the 'Extras' category.";

            public static string GooglePlayGamesAndroidSdkManagerFailed = $"Failed to find the {TplASM} executable. " +
                                                                          $"Make sure the {TplSDK} is properly installed and that its path is correctly configured in the Unity preferences window (under External Tools).";

        }

        public static string AboutTitle = TplGPGP4U;

        public static string AboutText =  "Copyright (C) 2014 Google Inc.\n\n" +
                                         $"This is an open-source plugin that allows cross-platform integration with {TplGPG} services. " +
                                          "For more information, visit the official site on Github: https://github.com/playgameservices/play-games-plugin-for-unity";

        public static string LicenseTitle = TplGPGP4U;

        public const string LicenseText = "Copyright (C) 2014 Google Inc. " +
                                          "All Rights Reserved.\n\n" +
                                          "Licensed under the Apache License, Version 2.0 (the \"License\"); " +
                                          "you may not use this file except in compliance with the License. " +
                                          "You may obtain a copy of the License at\n\n" +
                                          "      http://www.apache.org/licenses/LICENSE-2.0\n\n" +
                                          "Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an \"AS IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. " +
                                          "See the License for the specific language governing permissions and limitations under the License.";

    }

}

#endif