namespace Labo_fin_formation.DocumentManager.Models
{
    public class DocumentDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string? PriorityLevel { get; set; }
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid DeletedBy { get; set; }
        public DateTime DeletedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
