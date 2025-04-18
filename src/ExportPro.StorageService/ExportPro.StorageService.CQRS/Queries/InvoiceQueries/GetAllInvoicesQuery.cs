﻿using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.CQRS.Queries.InvoiceQueries;

public class GetAllInvoicesQuery : IQuery<PaginatedListDto<InvoiceDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; } = false;
}
