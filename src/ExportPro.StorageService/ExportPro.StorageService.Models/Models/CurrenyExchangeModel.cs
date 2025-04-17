﻿namespace ExportPro.StorageService.Models.Models;

public class CurrenyExchangeModel
{
    public required string From { get; set; }
    public required string To { get; set; }
    public required DateTime Date { get; set; }
    public double? AmountFrom { get; set; }
}
