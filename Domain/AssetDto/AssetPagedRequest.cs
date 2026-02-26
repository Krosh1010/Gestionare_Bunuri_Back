namespace Domain.AssetDto
{
    public class AssetPagedRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}