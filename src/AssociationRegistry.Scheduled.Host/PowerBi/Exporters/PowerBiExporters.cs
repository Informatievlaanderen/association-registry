namespace AssociationRegistry.Scheduled.Host.PowerBi.Exporters;

using System.Collections.ObjectModel;
using Amazon.S3;
using AssociationRegistry.Admin.Schema.PowerBiExport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Writers;

public class PowerBiExporters : ReadOnlyCollection<Exporter<PowerBiExportDocument>>
{
    public PowerBiExporters(IList<Exporter<PowerBiExportDocument>> exporters)
        : base(exporters)
    {
    }

    public static PowerBiExporters Create(
        IServiceProvider sp,
        PowerBiExportOptions powerBiExportOptions
    ) =>
        new(
            new List<Exporter<PowerBiExportDocument>>
            {
                CreateExporter<BasisgegevensRecordWriter>(sp, powerBiExportOptions, WellKnownFileNames.Basisgegevens),
                CreateExporter<ContactgegevensRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Contactgegevens
                ),
                CreateExporter<HoofdactiviteitenRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Hoofdactiviteiten
                ),
                CreateExporter<WerkingsgebiedenRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Werkingsgebieden
                ),
                CreateExporter<LocatiesRecordWriter>(sp, powerBiExportOptions, WellKnownFileNames.Locaties),
                CreateExporter<HistoriekRecordWriter>(sp, powerBiExportOptions, WellKnownFileNames.Historiek),
                CreateExporter<LidmaatschappenRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Lidmaatschappen
                ),
                CreateExporter<BankrekeningnummerRecordWriter>(
                    sp,
                    powerBiExportOptions,
                    WellKnownFileNames.Bankrekeningnummers
                ),
                CreateExporter<ErkenningenRecordWriter>(sp, powerBiExportOptions, WellKnownFileNames.Erkenningen),
            }
        );

    private static Exporter<PowerBiExportDocument> CreateExporter<TWriter>(
        IServiceProvider sp,
        PowerBiExportOptions options,
        string fileName
    )
        where TWriter : class, IRecordWriter<PowerBiExportDocument>, new() =>
        new(
            fileName,
            options.BucketName,
            new TWriter(),
            sp.GetRequiredService<IAmazonS3>(),
            sp.GetRequiredService<ILogger<Exporter<PowerBiExportDocument>>>());
}
