namespace Domain.AssetDto
{
    public class AssetPagedRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Filtre opționale
        public string? Name { get; set; }
        public string? Category { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int? SpaceId { get; set; }
    }
}