namespace DTOs;

public class AnimeGetDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string[]? Genres { get; set; }
    public DateTime? ReleaseDate { get; set; }
}
public class AnimePostDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string[]? Genres { get; set; }
    public DateTime? ReleaseDate { get; set; }
}

public class AnimePutDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string[]? Genres { get; set; }
    public DateTime? ReleaseDate { get; set; }
}


public class AnimeDeleteDTO
{
    
}
