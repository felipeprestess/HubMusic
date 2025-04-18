namespace HubMusic.Models
{
    public class CreateSongRequest
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string? Album { get; set; }
        public TimeSpan Duration { get; set; }
        public string Genre { get; set; }
    }
}