public class UpdateSongRequest
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Genre { get; set; }
}