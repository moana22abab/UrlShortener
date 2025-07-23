using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Models;

[ApiController]
[Route("")]
public class UrlShortenerController : ControllerBase
{
    private readonly AppDbContext _context;

    public UrlShortenerController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("shorten")]
    public IActionResult Shorten([FromBody] UrlRequest request)
    {
      
        if (string.IsNullOrWhiteSpace(request.OriginalUrl) || request.OriginalUrl.Trim().ToLower() == "string")
        {
            return BadRequest("Original URL is required.");
        }

        
        var existing = _context.UrlMappings
            .FirstOrDefault(x => x.OriginalUrl == request.OriginalUrl);

        if (existing != null)
        {
            var existingUrl = $"{Request.Scheme}://{Request.Host}/r/{existing.ShortCode}";
            return Ok(new { shortUrl = existingUrl });
        }

        
        string inputCode = request.CustomCode?.Trim();
        if (inputCode == "string") inputCode = null;

        string shortCode;

        if (!string.IsNullOrWhiteSpace(inputCode))
        {
            
            var customExists = _context.UrlMappings
                .Any(x => x.ShortCode == inputCode);

            if (customExists)
                return Conflict("Custom short code is already taken.");

            shortCode = inputCode;
        }
        else
        {
            
            do
            {
                shortCode = Guid.NewGuid().ToString("N")[..6];
            } while (_context.UrlMappings.Any(x => x.ShortCode == shortCode));
        }

        DateTime expiration;

        if (!request.ExpirationDate.HasValue || request.ExpirationDate <= DateTime.UtcNow)
        {
            
            expiration = DateTime.UtcNow.AddDays(7);
        }
        else
        {
            expiration = request.ExpirationDate.Value;
        }


        var mapping = new UrlMapping
        {
            OriginalUrl = request.OriginalUrl,
            ShortCode = shortCode,
            ExpirationDate = expiration
        };

        _context.UrlMappings.Add(mapping);
        _context.SaveChanges();

        var resultUrl = $"{Request.Scheme}://{Request.Host}/r/{shortCode}";
        return Ok(new { shortUrl = resultUrl });
    }





    [HttpGet("api/{code}")]
    public IActionResult GetOriginalUrl(string code)
    {
        var mapping = _context.UrlMappings.FirstOrDefault(x => x.ShortCode == code);

        if (mapping == null)
            return NotFound("Short URL not found.");

        return Ok(new { originalUrl = mapping.OriginalUrl });
    }

    [HttpGet("r/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult RedirectToOriginal(string code)
    {
        var mapping = _context.UrlMappings.FirstOrDefault(x => x.ShortCode == code);

        if (mapping == null || (mapping.ExpirationDate != null && mapping.ExpirationDate < DateTime.UtcNow))
            return NotFound("Short URL not found or has expired.");

        mapping.VisitCount++;
        _context.SaveChanges();

        return Redirect(mapping.OriginalUrl);
    }


    [HttpGet("api/{code}/analytics")]
    public IActionResult GetAnalytics(string code)
    {
        var mapping = _context.UrlMappings.FirstOrDefault(x => x.ShortCode == code);

        if (mapping == null)
            return NotFound("Short URL not found.");

        return Ok(new
        {
            originalUrl = mapping.OriginalUrl,
            visitCount = mapping.VisitCount
        });
    }


}
