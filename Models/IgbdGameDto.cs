namespace NextUp.Models
{
    public class IgdbGameDto
    {
        public long id { get; set; }
        public string? name { get; set; }
        public long? first_release_date { get; set; }
        public Cover? cover { get; set; }
        public Platform[]? platforms { get; set; }

        public class Cover
        {
            public string? url { get; set; }
        }

        public class Platform
        {
            public string? name { get; set; }
        }
    }
}
