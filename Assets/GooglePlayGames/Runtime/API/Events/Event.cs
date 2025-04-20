using System;

using AE  = GooglePlayGames.Api.Events.Event;
using AEV = GooglePlayGames.Api.Events.EventVisibility;
using AIE = GooglePlayGames.Api.Events.IEvent;

namespace GooglePlayGames.Api.Events {

    internal sealed class Event : AIE {

        internal Event(string id, string name, string description, string image, ulong count, AEV visibility)
        {
            CurrentCount = count;
            Description  = description;
            Id           = id;
            ImageUrl     = image;
            Name         = name;
            Visibility   = visibility;
        }

        public ulong  CurrentCount { get; }
        public string Description  { get; }
        public string Id           { get; }
        public string ImageUrl     { get; }
        public string Name         { get; }
        public AEV    Visibility   { get; }

        #region Object implementation

        public override string ToString() => $"IEvent(Id: {Id}, Name: {Name}, CurrentCount: {CurrentCount}, Visibility: {Visibility}, Description: {Description}, ImageUrl: {ImageUrl})";

        public override int GetHashCode() => HashCode.Combine(CurrentCount, Description, Id, ImageUrl, Name, Visibility);

        public override bool Equals(object other)
        {
            if (other is not AE it) return false;
            return CurrentCount ==     it.CurrentCount &&
                   Description .Equals(it.Description) &&
                   Id          .Equals(it.Id)          &&
                   ImageUrl    .Equals(it.ImageUrl)    &&
                   Name        .Equals(it.Name)        &&
                   Visibility   ==     it.Visibility;
        }

        #endregion Object implementation

    }

}