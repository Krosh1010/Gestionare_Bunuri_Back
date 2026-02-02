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
        [HttpGet("{spaceId}")]
        public async Task<IActionResult> GetSpaceById(int spaceId)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int ownerId = int.Parse(userIdString);

            var space = await _spaceService.GetSpaceByIdAsync(spaceId, ownerId);
            if (space == null)
                return NotFound();

            return Ok(space);
        }

        [HttpGet("parents")]
        public async Task<IActionResult> GetParentSpaces()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int ownerId = int.Parse(userIdString);
            var parents = await _spaceService.GetSpacesByParentAsync(ownerId, null);
            return Ok(parents);
        }

        [HttpGet("children/{parentId}")]
        public async Task<IActionResult> GetChildSpaces(int parentId)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int ownerId = int.Parse(userIdString);
            var children = await _spaceService.GetSpacesByParentAsync(ownerId, parentId);
            return Ok(children);
        }
        [HttpGet("path/{spaceId}")]
        public async Task<IActionResult> GetSpacePath(int spaceId)
        {
            var path = await _spaceService.GetSpacePathAsync(spaceId);
            if (path == null || path.Count == 0)
                return NotFound();
            return Ok(path);
        }

        [HttpDelete("{spaceId}")]
        public async Task<IActionResult> DeleteSpace(int spaceId)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int ownerId = int.Parse(userIdString);

            var result = await _spaceService.DeleteSpaceAsync(spaceId, ownerId);
            if (!result)
                return NotFound("Space not found or you do not have permission.");

            return NoContent();
        }
        [HttpPatch("{spaceId}")]
        public async Task<IActionResult> PatchSpace(int spaceId, [FromBody] SpaceUpdateDto dto)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int ownerId = int.Parse(userIdString);

            var result = await _spaceService.PatchSpaceAsync(spaceId, ownerId, dto);
            if (result == null)
                return NotFound("Space not found or you do not have permission.");

            return Ok(result);
        }

    }
}
