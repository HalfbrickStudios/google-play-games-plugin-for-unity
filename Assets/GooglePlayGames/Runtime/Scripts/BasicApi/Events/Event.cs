namespace GooglePlayGames.BasicApi.Events {

    internal class Event : IEvent {

        internal Event(string id, string name, string description, string imageUrl, ulong currentCount, EventVisibility visibility)
        {
            Id           = id;
            Name         = name;
            Description  = description;
            ImageUrl     = imageUrl;
            CurrentCount = currentCount;
            Visibility   = visibility;
        }

        public string          Id           { get; private set; }
        public string          Name         { get; private set; }
        public string          Description  { get; private set; }
        public string          ImageUrl     { get; private set; }
        public ulong           CurrentCount { get; private set; }
        public EventVisibility Visibility   { get; private set; }

    }

}