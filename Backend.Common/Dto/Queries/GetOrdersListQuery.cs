namespace Backend.Common.Dto.Queries;

public class GetOrdersListQuery
{
    public string? SearchOrderNumber { get; set; } = null;
    public DateTime? DateStart { get; set; } = null;
    public DateTime? DateEnd { get; set; } = null;
    public int Page { get; set; } = 1;
}