namespace BonusSystem.Core.Services.Interfaces;

public interface IStatisticsExportService
{
    Task<Stream> ExportToCsvAsync<T>(T data);
    Task<Stream> ExportToExcelAsync<T>(T data);
}