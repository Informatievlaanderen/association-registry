namespace AssociationRegistry.Scheduled.Host.PowerBi.Exporters;

using System.Collections.ObjectModel;
using Amazon.S3;
using AssociationRegistry.Admin.Schema.PowerBiExport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Writers;

public class PowerBiDubbelDetectieExporters : ReadOnlyCollection<Exporter<PowerBiExportDubbelDetectieDocument>>
{
    public PowerBiDubbelDetectieExporters(IList<Exporter<PowerBiExportDubbelDetectieDocument>> exporters)
        : base(exporters)
    {
    }

    public static PowerBiDubbelDetectieExporters Create(
        IServiceProvider sp,
        PowerBiExportOptions options
    ) =>
        new(
            new List<Exporter<PowerBiExportDubbelDetectieDocument>>
            {
                CreateDubbelDetectieExporter<DubbelDetectieRecordWriter>(
                    sp,
                    options,
                    WellKnownFileNames.DubbelDetectie
                ),
            }
        );

    private static Exporter<PowerBiExportDubbelDetectieDocument> CreateDubbelDetectieExporter<TWriter>(
        IServiceProvider sp,
        PowerBiExportOptions options,
        string fileName
    )
        where TWriter : class, IRecordWriter<PowerBiExportDubbelDetectieDocument>, new() =>
        new(
            fileName,
            options.BucketName,
            new TWriter(),
            sp.GetRequiredService<IAmazonS3>(),
            sp.GetRequiredService<ILogger<Exporter<PowerBiExportDubbelDetectieDocument>>>());
}
