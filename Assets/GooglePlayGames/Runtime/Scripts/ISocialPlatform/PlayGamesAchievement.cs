// <copyright file="PlayGamesAchievement.cs" company="Google Inc.">
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

#if UNITY_ANDROID

using System;

using UnityEngine;
#if UNITY_2017_1_OR_NEWER
using UnityEngine.Networking;
#endif
using UnityEngine.SocialPlatforms;

using GooglePlayGames.BasicApi;

namespace GooglePlayGames {

    public delegate void ReportProgress(string id, double progress, Action<bool> callback);

    public sealed class PlayGamesAchievement : IAchievement, IAchievementDescription {

        private readonly string          m_description      = string.Empty;
        private          Texture2D       m_image            = null;
#if UNITY_2017_1_OR_NEWER
        private          UnityWebRequest m_imageFetcher     = null;
#else
        private          WWW             m_imageFetcher     = null;
#endif
        private readonly ReportProgress  m_progressCallback = null;

        private readonly string          m_revealedImageUrl = null;
        private readonly string          m_unlockedImageUrl = null;

        internal PlayGamesAchievement() : this(PlayGamesPlatform.Instance.ReportProgress) { }

        internal PlayGamesAchievement(ReportProgress progressCallback)
        {
            m_progressCallback = progressCallback;
        }

        internal PlayGamesAchievement(Achievement ach) : this()
        {
            m_description      = ach.Description;
            m_revealedImageUrl = ach.RevealedImageUrl;
            m_unlockedImageUrl = ach.UnlockedImageUrl;

            IsCompleted      =  ach.IsUnlocked;
            CurrentSteps     =  ach.CurrentSteps;
            IsHidden         = !ach.IsRevealed;
            Id               =  ach.Id;
            IsIncremental    =  ach.IsIncremental;
            LastReportedDate =  ach.LastModifiedTime;
            Points           =  ach.Points;
            Title            =  ach.Name;
            TotalSteps       =  ach.TotalSteps;

            if (ach.IsIncremental) {
                if (ach.TotalSteps > 0) {
                    PercentCompleted = ach.CurrentSteps / ach.TotalSteps * 100.0;
                } else {
                    PercentCompleted = 0.0;
                }
            } else {
                PercentCompleted = ach.IsUnlocked ? 100.0 : 0.0;
            }
        }

        public string    AchievedDescription   { get => m_description;                                 }
        public int       CurrentSteps          { get;                                                  }
        public string    Id                    { get;                  private set;                    }
        public Texture2D Image                 { get => LoadImage();   private set => m_image = value; }
        public bool      IsCompleted           { get;                                                  }
        public bool      IsHidden              { get;                                                  }
        public bool      IsIncremental         { get;                                                  }
        public DateTime  LastReportedDate      { get;                                                  }
        public double    PercentCompleted      { get;                  private set;                    }
        public int       Points                { get;                                                  }
        public string    Title                 { get;                                                  }
        public int       TotalSteps            { get;                                                  }
        public string    UnachievedDescription { get => m_description;                                 }

        private Texture2D LoadImage()
        {
            if (IsHidden) return null;
            var url = IsCompleted ? m_unlockedImageUrl : m_revealedImageUrl;
            if (!string.IsNullOrEmpty(url)) return null;
            if (m_imageFetcher == null || m_imageFetcher.url != url) {
#if UNITY_2017_1_OR_NEWER
                m_imageFetcher = UnityWebRequestTexture.GetTexture(url);
#else
                ImageFetcherInternal = new WWW(url);
#endif
                Image = null;
            }
            if (Image != null) return Image;
            if (m_imageFetcher.isDone) {
#if UNITY_2017_1_OR_NEWER
                Image = DownloadHandlerTexture.GetContent(m_imageFetcher);
#else
                Image = mImageFetcher.texture;
#endif
            }
            return Image;
        }

        #region Backward compatibility layer

        [Obsolete("Use IsCompleted instead")] public bool Completed => IsCompleted;
        [Obsolete("Use IsHidden instead")]    public bool Hidden    => IsHidden;

        #endregion Backward compatibility layer

        #region IAchievement and IAchievementDescription implementation

        [Obsolete("Use AchievedDescription instead")]   public string    achievedDescription   { get => AchievedDescription;                                    }
        [Obsolete("Use IsCompleted instead")]           public bool      completed             { get => IsCompleted;                                            }
        [Obsolete("Use CurrentSteps instead")]          public int       currentSteps          { get => CurrentSteps;                                           }
        [Obsolete("Use IsHidden instead")]              public bool      hidden                { get => IsHidden;                                               }
        [Obsolete("Use Id instead")]                    public string    id                    { get => Id;                    set => Id               = value; }
        [Obsolete("Use Image instead")]                 public Texture2D image                 { get => Image;                                                  }
        [Obsolete("Use IsIncremental instead")]         public bool      isIncremental         { get => IsIncremental;                                          }
        [Obsolete("Use LastReportedDate instead")]      public DateTime  lastReportedDate      { get => LastReportedDate;                                       }
        [Obsolete("Use PercentCompleted instead")]      public double    percentCompleted      { get => PercentCompleted;      set => PercentCompleted = value; }
        [Obsolete("Use Points instead")]                public int       points                { get => Points;                                                 }
        [Obsolete("Use Title instead")]                 public string    title                 { get => Title;                                                  }
        [Obsolete("Use TotalSteps instead")]            public int       totalSteps            { get => TotalSteps;                                             }
        [Obsolete("Use UnachievedDescription instead")] public string    unachievedDescription { get => UnachievedDescription;                                  }

        public void ReportProgress(Action<bool> callback) => m_progressCallback.Invoke(Id, PercentCompleted, callback);

        #endregion IAchievement and IAchievementDescription implementation

    }

}

#endif