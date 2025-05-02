using ExportPro.Export.SDK.Interfaces;

namespace ExportPro.Export.Job.ServiceHost.Services;

public class ReportGeneratorFactory(IEnumerable<IReportGenerator> generators)
{
    private readonly IEnumerable<IReportGenerator> _generators = generators;

    public IReportGenerator GetGenerator(string fileType)
    {
        return _generators.FirstOrDefault(g =>
            string.Equals(g.Extension, fileType, StringComparison.OrdinalIgnoreCase))
            ?? throw new NotSupportedException($"Report type '{fileType}' is not supported.");
    }
}