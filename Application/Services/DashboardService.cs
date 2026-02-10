using System.Collections.Generic;
using Infrastructure.Abstraction;
using Domain.Dashboard;
using Application.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardAssetSummaryDto> GetAssetSummaryAsync(int userId)
        {
            return await _dashboardRepository.GetAssetSummaryAsync(userId);
        }
    }
}
