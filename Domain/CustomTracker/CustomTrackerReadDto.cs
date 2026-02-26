using Domain.DbTables;

namespace Domain.CustomTracker
{
    public class CustomTrackerReadDto
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CustomTrackerStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
