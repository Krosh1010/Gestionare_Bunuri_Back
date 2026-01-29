using Domain.Space;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpacesController : ControllerBase
    {
        private readonly ISpaceService _spaceService;

        public SpacesController(ISpaceService spaceService)
        {
            _spaceService = spaceService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSpace([FromBody] SpaceCreateDto dto)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int ownerId = int.Parse(userIdString);

            var result = await _spaceService.CreateSpaceAsync(dto, ownerId);

            if (result is string error)
            {
                if (error == "Owner user does not exist.")
                    return BadRequest(error);
                if (error == "Space with this name already exists for this owner.")
                    return Conflict(error);

                return StatusCode(500, error);
            }

            return Ok(result);
        }
    }
}
