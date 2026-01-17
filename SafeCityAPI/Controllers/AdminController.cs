using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCityAPI.DTOs;
using SafeCityAPI.Services;

namespace SafeCityAPI.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IAdminService adminService,
        ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    /// <summary>
    /// Usuń zgłoszenie (tylko admin)
    /// </summary>
    [HttpDelete("reports/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteReport(Guid id)
    {
        try
        {
            var deleted = await _adminService.DeleteReportAsync(id);
            
            if (!deleted)
            {
                return NotFound(new { error = "Report not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting report {ReportId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Pobierz listę użytkowników (tylko admin)
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(List<AdminUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        try
        {
            if (limit < 1 || limit > 100)
            {
                return BadRequest(new { error = "Limit must be between 1 and 100" });
            }

            if (offset < 0)
            {
                return BadRequest(new { error = "Offset must be non-negative" });
            }

            var users = await _adminService.GetUsersAsync(limit, offset);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users list");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Pobierz statystyki systemu (tylko admin)
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(AdminStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await _adminService.GetStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching admin statistics");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}