using ExportPro.Core.Repositories.CSVRepositry;
using ExportPro.Core.Repositories.ExcelRepository;
using ExportPro.Core.Repositories.PDFRepository;
using ExportPro.Core.Repositories.StorageRepository;

namespace ExportPro.Core.UoW;

public interface IUnitOfWork
{
    ICSVRepository CSVRepository { get; }
    IExcelRepository ExcelRepository { get; }
    IPDFRepository PDFRepository { get; }
    IStorageRepository StorageRepository { get; }
}
