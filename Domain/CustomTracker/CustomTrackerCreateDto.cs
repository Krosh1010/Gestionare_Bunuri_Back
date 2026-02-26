namespace Domain.CustomTracker
{
    public class CustomTrackerCreateDto
    {
        public int AssetId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
