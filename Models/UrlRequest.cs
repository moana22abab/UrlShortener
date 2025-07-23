using System.ComponentModel.DataAnnotations;

public class UrlRequest
{
    [Required]
    public string OriginalUrl { get; set; } = string.Empty;

    
    public string? CustomCode { get; set; }

    
    public DateTime? ExpirationDate { get; set; }
}
