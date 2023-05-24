namespace Backend.Common.Dto.Queries;

public class GetOrdersManagerListQuery
{
    public List<string>? Statuses { get; set; } = null;
    public string? SearchOrderNumber { get; set; } = null;
    public string? Sorting { get; set; } = null;
    public int Page { get; set; } = 1;
}