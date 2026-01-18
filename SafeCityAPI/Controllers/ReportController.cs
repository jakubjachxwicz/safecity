using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCityAPI.DTOs;
using SafeCityAPI.Services;

namespace SafeCityAPI.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    private readonly IRateLimitService _rateLimitService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportService service,
        IRateLimitService rateLimitService,
        ILogger<ReportsController> logger)
    {
        _service = service;
        _rateLimitService = rateLimitService;
        _logger = logger;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest request)
    {
        try
        {
            var ipAddress = GetClientIpAddress();
            var userId = GetUserIdFromAuthenticatedToken();
            
            // Sprawdź rate limit PRZED utworzeniem zgłoszenia
            var (canReport, secondsRemaining) = await _rateLimitService.CanReportAsync(userId, ipAddress);
            
            if (!canReport)
            {
                _logger.LogWarning("Rate limit exceeded for {Identifier}, {Seconds}s remaining", 
                    userId?.ToString() ?? ipAddress, secondsRemaining);
                
                return StatusCode(StatusCodes.Status429TooManyRequests, new 
                { 
                    error = "Too many requests",
                    message = $"Please wait {secondsRemaining} seconds before submitting another report",
                    secondsRemaining,
                    retryAfter = secondsRemaining
                });
            }
            
            // Utwórz zgłoszenie
            var result = await _service.CreateReportAsync(request, ipAddress, userId);
            
            return CreatedAtAction(nameof(GetReportById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating report");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Pobierz wszystkie aktywne zgłoszenia (publiczne)
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<ReportResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveReports()
    {
        try
        {
            var reports = await _service.GetActiveReportsAsync();
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching active reports");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReportById(Guid id)
    {
        try
        {
            var report = await _service.GetReportByIdAsync(id);
            
            if (report == null)
            {
                return NotFound(new { error = "Report not found" });
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching report {ReportId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<ReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchReports(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] double? minLat = null,
        [FromQuery] double? maxLat = null,
        [FromQuery] double? minLon = null,
        [FromQuery] double? maxLon = null)
    {
        try
        {
            if (minLat.HasValue && (minLat < -90 || minLat > 90))
                return BadRequest(new { error = "minLat must be between -90 and 90" });

            if (maxLat.HasValue && (maxLat < -90 || maxLat > 90))
                return BadRequest(new { error = "maxLat must be between -90 and 90" });

            if (minLon.HasValue && (minLon < -180 || minLon > 180))
                return BadRequest(new { error = "minLon must be between -180 and 180" });

            if (maxLon.HasValue && (maxLon < -180 || maxLon > 180))
                return BadRequest(new { error = "maxLon must be between -180 and 180" });

            if (minLat.HasValue && maxLat.HasValue && minLat > maxLat)
                return BadRequest(new { error = "minLat cannot be greater than maxLat" });

            if (minLon.HasValue && maxLon.HasValue && minLon > maxLon)
                return BadRequest(new { error = "minLon cannot be greater than maxLon" });
            
            if (from.HasValue && to.HasValue && from > to)
                return BadRequest(new { error = "from cannot be after to" });

            var reports = await _service.SearchReportsAsync(from, to, minLat, maxLat, minLon, maxLon);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching reports");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    [HttpGet("my/count")]
    [Authorize]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyReportCount()
    {
        try
        {
            var userId = GetUserIdFromAuthenticatedToken();
            
            if (userId == null)
            {
                _logger.LogWarning("Failed to extract user ID from authenticated token");
                return Unauthorized(new { error = "Invalid token" });
            }

            var count = await _service.GetUserReportCountAsync(userId.Value);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user report count");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    [HttpGet("my/history")]
    [Authorize]
    [ProducesResponseType(typeof(List<ReportResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyReportHistory()
    {
        try
        {
            var userId = GetUserIdFromAuthenticatedToken();
            
            if (userId == null)
            {
                _logger.LogWarning("Failed to extract user ID from authenticated token");
                return Unauthorized(new { error = "Invalid token" });
            }

            var reports = await _service.GetUserReportHistoryAsync(userId.Value);
            return Ok(reports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user report history");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    [HttpGet("/health")]
    public IActionResult HealthCheck()
    {
        return Ok(new 
        { 
            status = "Healthy",
            timestamp = DateTime.UtcNow 
        });
    }
    
     /// <summary>
    /// Edytuj swoje zgłoszenie (tylko dla zalogowanych użytkowników)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReport(Guid id, [FromBody] UpdateReportRequest request)
    {
        try
        {
            var userId = GetUserIdFromAuthenticatedToken();
            
            if (userId == null)
            {
                _logger.LogWarning("Failed to extract user ID from authenticated token");
                return Unauthorized(new { error = "Invalid token" });
            }

            var result = await _service.UpdateReportAsync(id, request, userId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Report not found: {Message}", ex.Message);
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized access: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Validation error: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating report {ReportId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Usuń swoje zgłoszenie (tylko dla zalogowanych użytkowników)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReport(Guid id)
    {
        try
        {
            var userId = GetUserIdFromAuthenticatedToken();
            
            if (userId == null)
            {
                _logger.LogWarning("Failed to extract user ID from authenticated token");
                return Unauthorized(new { error = "Invalid token" });
            }

            var deleted = await _service.DeleteReportAsync(id, userId.Value);
            
            if (!deleted)
            {
                return NotFound(new { error = "Report not found" });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized access: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting report {ReportId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
    
    private Guid? GetUserIdFromAuthenticatedToken()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            _logger.LogDebug("No authenticated user");
            return null;
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                         ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
        
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogDebug("Authenticated user: {UserId}", userId);
            return userId;
        }

        _logger.LogWarning("Authenticated user but no valid userId claim found");
        return null;
    }
    
    private string GetClientIpAddress()
    {
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}