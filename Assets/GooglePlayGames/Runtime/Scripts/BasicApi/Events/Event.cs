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

        public ulong           CurrentCount { get; private set; }
        public string          Description  { get; private set; }
        public string          Id           { get; private set; }
        public string          ImageUrl     { get; private set; }
        public string          Name         { get; private set; }
        public EventVisibility Visibility   { get; private set; }

    }

}