using Domain.Dashboard;
using System.Threading.Tasks;

namespace Infrastructure.Abstraction
{
    public interface IDashboardRepository
    {
        Task<DashboardAssetSummaryDto> GetAssetSummaryAsync(int userId);

    }
}
