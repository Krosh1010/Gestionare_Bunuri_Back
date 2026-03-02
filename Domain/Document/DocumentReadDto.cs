using Domain.DbTables;

namespace Domain.Document
{
    public class DocumentReadDto
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public DocumentType Type { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public DateTime UploadedAt { get; set; }
    }
}
