namespace Labo_fin_formation.DocumentManager.Models
{
    public class DocumentFilter
    {
        public string? SearchTerm { get; set; }
        public string? ConfidentialityLevel { get; set; }
        public string OrderBy { get; set; } = "CreatedAt";
        public bool Descending { get; set; } = true;
        public int Offset { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
