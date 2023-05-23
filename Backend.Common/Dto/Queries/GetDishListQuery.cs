namespace Backend.Common.Dto.Queries;

public class GetDishListQuery
{
    public Guid RestaurantId { get; set; }
    public List<string> Categories { get; set; } = new();
    public bool? Vegetarian { get; set; } = null;
    public string? Sorting { get; set; } = null;
    public int Page { get; set; } = 1;
}