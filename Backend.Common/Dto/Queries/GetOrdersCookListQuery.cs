namespace Backend.Common.Dto.Queries;

public class GetOrdersCookListQuery
{
    public string? Sorting { get; set; } = null;
    public int Page { get; set; } = 1;
}