using Domain.AssetDto;
using Domain.Export;

namespace Application.Abstraction
{
    public interface IExportService
    {
        Task<byte[]> ExportAssetsToExcel(int userId, AssetExportRequest request);
        Task<byte[]> ExportAssetsToPdf(int userId, AssetExportRequest request);
        Task<byte[]> ExportAssetsToCsv(int userId, AssetExportRequest request);
    }


}
