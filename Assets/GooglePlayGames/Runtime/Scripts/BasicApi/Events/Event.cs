namespace GooglePlayGames.BasicApi.Events {

    internal sealed class Event : IEvent {

        internal Event(string id, string name, string description, string imageUrl, ulong currentCount, EventVisibility visibility)
        {
            CurrentCount = currentCount;
            Description  = description;
            Id           = id;
            ImageUrl     = imageUrl;
            Name         = name;
            Visibility   = visibility;
        }

        public ulong           CurrentCount { get; }
        public string          Description  { get; }
        public string          Id           { get; }
        public string          ImageUrl     { get; }
        public string          Name         { get; }
        public EventVisibility Visibility   { get; }

    }

}