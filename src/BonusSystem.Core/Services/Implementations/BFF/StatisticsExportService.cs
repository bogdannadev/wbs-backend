// using OfficeOpenXml;
using System.Text;
using System.Reflection;
using BonusSystem.Core.Services.Interfaces;

namespace BonusSystem.Core.Services.Implementations.BFF;

public class StatisticsExportService : IStatisticsExportService
{
    public async Task<Stream> ExportToCsvAsync<T>(T data)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, Encoding.UTF8);

        var properties = typeof(T).GetProperties();
        
        // Write headers
        await writer.WriteLineAsync(string.Join(",", properties.Select(p => p.Name)));

        // Write data
        if (data is IEnumerable<object> collection)
        {
            foreach (var item in collection)
            {
                var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                await writer.WriteLineAsync(string.Join(",", values));
            }
        }
        else
        {
            var values = properties.Select(p => p.GetValue(data)?.ToString() ?? "");
            await writer.WriteLineAsync(string.Join(",", values));
        }

        await writer.FlushAsync();
        stream.Position = 0;
        return stream;
    }

    // public async Task<Stream> ExportToExcelAsync<T>(T data)
    // {
    //     var stream = new MemoryStream();
    //     using (var package = new ExcelPackage(stream))
    //     {
    //         var worksheet = package.Workbook.Worksheets.Add("Report");
    //         var properties = typeof(T).GetProperties();

    //         // Write headers
    //         for (int i = 0; i < properties.Length; i++)
    //         {
    //             worksheet.Cells[1, i + 1].Value = properties[i].Name;
    //         }

    //         // Write data
    //         int row = 2;
    //         if (data is IEnumerable<object> collection)
    //         {
    //             foreach (var item in collection)
    //             {
    //                 for (int i = 0; i < properties.Length; i++)
    //                 {
    //                     worksheet.Cells[row, i + 1].Value = properties[i].GetValue(item);
    //                 }
    //                 row++;
    //             }
    //         }
    //         else
    //         {
    //             for (int i = 0; i < properties.Length; i++)
    //             {
    //                 worksheet.Cells[row, i + 1].Value = properties[i].GetValue(data);
    //             }
    //         }

    //         await package.SaveAsync();
    //     }

    //     stream.Position = 0;
    //     return stream;
    // }
}