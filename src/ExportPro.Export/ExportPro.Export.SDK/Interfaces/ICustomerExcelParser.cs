using ExportPro.StorageService.SDK.DTOs.CustomerDTO;

namespace ExportPro.Export.SDK.Interfaces;

public interface ICustomerExcelParser
{
    List<CreateUpdateCustomerDto> Parse(Stream excelStream);
}