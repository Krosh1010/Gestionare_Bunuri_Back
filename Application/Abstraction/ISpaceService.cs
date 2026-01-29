using Domain.Space;

public interface ISpaceService
{
    Task<object> CreateSpaceAsync(SpaceCreateDto dto, int ownerId);
}