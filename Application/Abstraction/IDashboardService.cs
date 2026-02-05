using Domain.Dashboard;

namespace Application.Abstraction
{
    public interface IDashboardService
    {
        Task<DashboardAssetSummaryDto> GetAssetSummaryAsync(int userId);
    }
}

