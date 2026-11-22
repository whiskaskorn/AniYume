namespace DTOs;

public class QueryParametersDTO
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? Genre { get; set; }
    public string? SortBy { get; set; }
}