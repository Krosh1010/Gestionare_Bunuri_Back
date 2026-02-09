using Domain.AssetDto;

namespace Infrastructure.Abstraction
{

    public interface IExportRepository
    {
        byte[] GenerateExcel(IEnumerable<AssetReadDto> assets, List<string> columns);
        byte[] GeneratePdf(IEnumerable<AssetReadDto> assets, List<string> columns);
        byte[] GenerateCsv(IEnumerable<AssetReadDto> assets, List<string> columns);
    }

}
