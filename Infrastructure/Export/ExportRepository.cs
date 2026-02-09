using ClosedXML.Excel;
using Domain.AssetDto;
using Infrastructure.Abstraction;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Export
{
    public class ExportRepository : IExportRepository
    {
        private static readonly Dictionary<string, (string Header, Func<AssetReadDto, object?> Value)> FieldMap = new()
        {
            { "Id", ("ID bun", a => a.Id) },
            { "Name", ("Nume bun", a => a.Name) },
            { "Description", ("Descriere", a => a.Description) },
            { "Category", ("Categorie", a => a.Category) },
            { "Value", ("Valoare", a => a.Value) },
            { "SpaceName", ("Locatia", a => a.SpaceName) },
            { "PurchaseDate", ("Data achizitie", a => a.PurchaseDate?.ToString("yyyy-MM-dd")) },
            { "WarrantyEndDate", ("Garanție pana la", a => a.WarrantyEndDate?.ToString("yyyy-MM-dd")) },
            { "WarrantyStatus", ("Status garantie", a => a.WarrantyStatus?.ToString()) },
            { "WarrantyDaysLeft", ("Zile ramase garantie", a =>
                a.WarrantyEndDate.HasValue
                    ? Math.Max((a.WarrantyEndDate.Value - DateTime.UtcNow).Days, 0)
                    : (int?)null)
            },
            { "InsuranceEndDate", ("Asigurare pana la", a => a.InsuranceEndDate?.ToString("yyyy-MM-dd")) },
            { "InsuranceStatus", ("Status asigurare", a => a.InsuranceStatus?.ToString()) },
            { "InsuranceDaysLeft", ("Zile ramase asigurare", a =>
                a.InsuranceEndDate.HasValue
                    ? Math.Max((a.InsuranceEndDate.Value - DateTime.UtcNow).Days, 0)
                    : (int?)null)
            },
            { "InsuranceValue", ("Valoare asigurare", a => a.InsuranceValue) },
            { "InsuranceCompany", ("Companie asigurare", a => a.InsuranceCompany) },
            { "InsuranceStartDate", ("Asigurare de la", a => a.InsuranceStartDate?.ToString("yyyy-MM-dd")) },
            { "WarrantyProvider", ("Furnizor garanție", a => a.WarrantyProvider) },
            { "WarrantyStartDate", ("Garantie de la", a => a.WarrantyStartDate?.ToString("yyyy-MM-dd")) }
        };

        public byte[] GenerateExcel(IEnumerable<AssetReadDto> assets, List<string> columns)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Bunuri");

            // Header
            for (int i = 0; i < columns.Count; i++)
                if (FieldMap.TryGetValue(columns[i], out var map))
                    worksheet.Cell(1, i + 1).Value = map.Header;

            int row = 2;
            foreach (var asset in assets)
            {
                for (int col = 0; col < columns.Count; col++)
                {
                    if (FieldMap.TryGetValue(columns[col], out var map))
                        worksheet.Cell(row, col + 1).Value = map.Value(asset)?.ToString() ?? string.Empty;
                }
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GeneratePdf(IEnumerable<AssetReadDto> assets, List<string> columns)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(defs =>
                        {
                            for (int i = 0; i < columns.Count; i++)
                                defs.RelativeColumn();
                        });

                        // Header
                        for (int i = 0; i < columns.Count; i++)
                            if (FieldMap.TryGetValue(columns[i], out var map))
                                table.Cell().Element(CellStyle).Text(map.Header).SemiBold();

                        // Rows
                        foreach (var asset in assets)
                        {
                            for (int i = 0; i < columns.Count; i++)
                                if (FieldMap.TryGetValue(columns[i], out var map))
                                    table.Cell().Element(CellStyle).Text(map.Value(asset)?.ToString() ?? string.Empty);
                        }

                        static IContainer CellStyle(IContainer container) =>
                            container.PaddingVertical(2).PaddingHorizontal(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });
                });
            });

            return document.GeneratePdf();
        }
        public byte[] GenerateCsv(IEnumerable<AssetReadDto> assets, List<string> columns)
        {
            var sb = new System.Text.StringBuilder();

            // Header
            for (int i = 0; i < columns.Count; i++)
            {
                if (FieldMap.TryGetValue(columns[i], out var map))
                    sb.Append(map.Header);
                if (i < columns.Count - 1)
                    sb.Append(",");
            }
            sb.AppendLine();

            // Rows
            foreach (var asset in assets)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    if (FieldMap.TryGetValue(columns[i], out var map))
                    {
                        var value = map.Value(asset)?.ToString() ?? string.Empty;
                        // Escape CSV
                        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                            value = $"\"{value.Replace("\"", "\"\"")}\"";
                        sb.Append(value);
                    }
                    if (i < columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        }

    }
}
