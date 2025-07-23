namespace UrlShortener.Models
{
    public class UrlMapping
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; } = string.Empty;
        public string ShortCode { get; set; } = string.Empty;
        public int VisitCount { get; set; } = 0;
        public DateTime? ExpirationDate { get; set; }
    }
}
