namespace Models;

public class Anime
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string[]? Genres { get; set; }
    public DateTime ReleaseDate { get; set; }
}