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

using System;

using GooglePlayGames.Api;

using UDHT = UnityEngine.Networking.DownloadHandlerTexture;
using UIA  = UnityEngine.SocialPlatforms.IAchievement;
using UIAD = UnityEngine.SocialPlatforms.IAchievementDescription;
using UT2D = UnityEngine.Texture2D;
using UWR  = UnityEngine.Networking.UnityWebRequest;
using UWRT = UnityEngine.Networking.UnityWebRequestTexture;

using GPGA  = GooglePlayGames.PlayGamesAchievement;
using GPGP  = GooglePlayGames.PlayGamesPlatform;
using GPGRP = GooglePlayGames.ReportProgress;

namespace GooglePlayGames {

    public delegate void ReportProgress(string id, double progress, Action<bool> callback);

    public sealed class PlayGamesAchievement : UIA, UIAD {

        private readonly string m_description      = string.Empty;
        private          UT2D   m_image            = null;
        private          UWR    m_imageFetcher     = null;
        private readonly GPGRP  m_progressCallback = null;
        private readonly string m_revealedImageUrl = null;
        private readonly string m_unlockedImageUrl = null;

        internal PlayGamesAchievement() : this(GPGP.Instance.ReportProgress) { }

        internal PlayGamesAchievement(GPGRP progressCallback)
        {
            m_progressCallback = progressCallback;
        }

        internal PlayGamesAchievement(Achievement achievement) : this()
        {
            m_description      = achievement.Description;
            m_revealedImageUrl = achievement.RevealedImageUrl;
            m_unlockedImageUrl = achievement.UnlockedImageUrl;

            IsCompleted      =  achievement.IsUnlocked;
            CurrentSteps     =  achievement.CurrentSteps;
            IsHidden         = !achievement.IsRevealed;
            Id               =  achievement.Id;
            IsIncremental    =  achievement.IsIncremental;
            LastReportedDate =  achievement.LastModifiedTime;
            Points           =  achievement.Points;
            Title            =  achievement.Name;
            TotalSteps       =  achievement.TotalSteps;

            if (achievement.IsIncremental) {
                if (achievement.TotalSteps > 0) {
                    PercentCompleted = achievement.CurrentSteps / achievement.TotalSteps * 100.0;
                } else {
                    PercentCompleted = 0.0;
                }
            } else {
                PercentCompleted = achievement.IsUnlocked ? 100.0 : 0.0;
            }
        }

        public string   AchievedDescription   { get => m_description;                                 }
        public int      CurrentSteps          { get;                                                  }
        public string   Id                    { get;                  private set;                    }
        public UT2D     Image                 { get => LoadImage();   private set => m_image = value; }
        public bool     IsCompleted           { get;                                                  }
        public bool     IsHidden              { get;                                                  }
        public bool     IsIncremental         { get;                                                  }
        public DateTime LastReportedDate      { get;                                                  }
        public double   PercentCompleted      { get;                  private set;                    }
        public int      Points                { get;                                                  }
        public string   Title                 { get;                                                  }
        public int      TotalSteps            { get;                                                  }
        public string   UnachievedDescription { get => m_description;                                 }

        private UT2D LoadImage()
        {
            if (IsHidden) return null;
            var url = IsCompleted ? m_unlockedImageUrl : m_revealedImageUrl;
            if (!string.IsNullOrEmpty(url)) return null;
            if (m_imageFetcher == null || m_imageFetcher.url != url) {
                m_imageFetcher = UWRT.GetTexture(url);
                m_image = null;
            }
            if (m_image != null) return m_image;
            if (m_imageFetcher.isDone) {
                m_image = UDHT.GetContent(m_imageFetcher);
            }
            return m_image;
        }

        #region Backward compatibility layer

        [Obsolete("Use IsCompleted instead")] public bool Completed => IsCompleted;
        [Obsolete("Use IsHidden instead")]    public bool Hidden    => IsHidden;

        #endregion Backward compatibility layer

        #region IAchievement and IAchievementDescription implementation

        [Obsolete("Use AchievedDescription instead")]   public string   achievedDescription   { get => AchievedDescription;                                    }
        [Obsolete("Use IsCompleted instead")]           public bool     completed             { get => IsCompleted;                                            }
        [Obsolete("Use CurrentSteps instead")]          public int      currentSteps          { get => CurrentSteps;                                           }
        [Obsolete("Use IsHidden instead")]              public bool     hidden                { get => IsHidden;                                               }
        [Obsolete("Use Id instead")]                    public string   id                    { get => Id;                    set => Id               = value; }
        [Obsolete("Use Image instead")]                 public UT2D     image                 { get => Image;                                                  }
        [Obsolete("Use IsIncremental instead")]         public bool     isIncremental         { get => IsIncremental;                                          }
        [Obsolete("Use LastReportedDate instead")]      public DateTime lastReportedDate      { get => LastReportedDate;                                       }
        [Obsolete("Use PercentCompleted instead")]      public double   percentCompleted      { get => PercentCompleted;      set => PercentCompleted = value; }
        [Obsolete("Use Points instead")]                public int      points                { get => Points;                                                 }
        [Obsolete("Use Title instead")]                 public string   title                 { get => Title;                                                  }
        [Obsolete("Use TotalSteps instead")]            public int      totalSteps            { get => TotalSteps;                                             }
        [Obsolete("Use UnachievedDescription instead")] public string   unachievedDescription { get => UnachievedDescription;                                  }

        public void ReportProgress(Action<bool> callback) => m_progressCallback?.Invoke(Id, PercentCompleted, callback);

        #endregion IAchievement and IAchievementDescription implementation

        #region Object implementation

        public override string ToString() => $"PlayGamesAchievement(CurrentSteps: {CurrentSteps}, description: {m_description}, Id: {Id}, image: {m_image}, imageFetcher: {m_imageFetcher}, IsCompleted: {IsCompleted}, IsHidden: {IsHidden}, IsIncremental: {IsIncremental}, LastReportedDate: {LastReportedDate}, PercentCompleted: {PercentCompleted}, Points: {Points}, progressCallback: {m_progressCallback}, revealedImageUrl: {m_revealedImageUrl}, Title: {Title}, TotalSteps: {TotalSteps}, unlockedImageUrl: {m_unlockedImageUrl})";

        public override int GetHashCode()
        {
            var hash1 = HashCode.Combine(CurrentSteps,     m_description,    Id,     m_image,            m_imageFetcher,     IsCompleted, IsHidden,   IsIncremental);
            var hash2 = HashCode.Combine(LastReportedDate, PercentCompleted, Points, m_progressCallback, m_revealedImageUrl, Title,       TotalSteps, m_unlockedImageUrl);
            return HashCode.Combine(hash1, hash2);
        }

        public override bool Equals(object other)
        {
            if (other is not GPGA it) return false;
            return Id              .Equals(it.Id)               &&
                   m_description   .Equals(it.m_description)    &&
                   m_image         .Equals(it.m_image)          &&
                   m_imageFetcher  .Equals(it.m_imageFetcher)   &&
                   IsCompleted      ==     it.IsCompleted       &&
                   IsHidden         ==     it.IsHidden          &&
                   IsIncremental    ==     it.IsIncremental     &&
                   LastReportedDate.Equals(it.LastReportedDate) &&
                   PercentCompleted.Equals(it.PercentCompleted) &&
                   Points           ==     it.Points            &&
                   Title           .Equals(it.Title)            &&
                   TotalSteps       ==     it.TotalSteps;
        }

        #endregion Object implementation

    }

}