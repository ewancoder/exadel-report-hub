using ExportPro.Export.SDK.Interfaces;

namespace ExportPro.Export.Job.ServiceHost.Interfaces;

public interface IReportGeneratorFactory
{
    IReportGenerator GetGenerator(string format);
}