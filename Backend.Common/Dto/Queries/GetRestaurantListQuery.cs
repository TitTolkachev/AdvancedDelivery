namespace Backend.Common.Dto.Queries;

public class GetRestaurantListQuery
{
    public string? SearchName { get; set; } = null;
    public int Page { get; set; } = 1;
}