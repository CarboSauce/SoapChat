using System.Security.Claims;
using System.ServiceModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoapChat.Api.UserService;

namespace SoapChat.Api.Controllers;

[ApiController]
[Route("api/avatar")]
public class AvatarController : ControllerBase
{
    private readonly UserServiceClient _userServiceClient;

    public AvatarController(UserServiceClient userServiceClient)
    {
        _userServiceClient = userServiceClient;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(string userId)
    {
        try
        {
            var data = await _userServiceClient.GetAvatarAsync(userId);
            if (data == null || data.Length == 0)
                return NotFound();

            return File(data, "application/octet-stream", $"{userId}.bin");
        }
        catch (FaultException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var data = ms.ToArray();

            await _userServiceClient.UploadAvatarAsync(userId, data);
            return Ok();
        }
        catch (FaultException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
