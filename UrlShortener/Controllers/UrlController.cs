using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;

[ApiController]
[Route("")]
public class UrlShortenerController : ControllerBase
{
    private static readonly Dictionary<string, string> shortToLong = new();
    private static readonly Dictionary<string, string> longToShort = new();

    [HttpPost("shorten")]
    public IActionResult Shorten([FromBody] UrlRequest request)
    {
        if (longToShort.ContainsKey(request.OriginalUrl))
        {
            var existingShort = longToShort[request.OriginalUrl];
            var shortUrl = $"{Request.Scheme}://{Request.Host}/r/{existingShort}";
            return Ok(new { shortUrl });
        }

        var shortCode = Guid.NewGuid().ToString("N")[..6];
        shortToLong[shortCode] = request.OriginalUrl;
        longToShort[request.OriginalUrl] = shortCode;

        var generatedShortUrl = $"{Request.Scheme}://{Request.Host}/r/{shortCode}";
        return Ok(new { shortUrl = generatedShortUrl });
    }

    // ✅ For API clients or Swagger — returns the original URL
    [HttpGet("api/{code}")]
    public IActionResult GetOriginalUrl(string code)
    {
        if (shortToLong.TryGetValue(code, out var originalUrl))
        {
            return Ok(new { originalUrl });
        }

        return NotFound("Short URL not found.");
    }

    [HttpGet("r/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult RedirectToOriginal(string code)
    {
        if (shortToLong.TryGetValue(code, out var originalUrl))
        {
            return Redirect(originalUrl);
        }

        return NotFound("Short URL not found.");
    }

}
