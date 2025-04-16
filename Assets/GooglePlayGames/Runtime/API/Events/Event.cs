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

    }

}