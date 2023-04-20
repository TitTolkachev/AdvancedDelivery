namespace DeliveryBackend.DTO.Queries;

public class GetRestaurantListQuery
{
    public string? Sorting { get; set; } = null;
    public int Page { get; set; } = 1;
}