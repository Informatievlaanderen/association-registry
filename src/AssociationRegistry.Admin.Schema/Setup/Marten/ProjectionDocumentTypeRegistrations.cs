namespace AssociationRegistry.Admin.Schema.Setup.Marten;

using Bewaartermijn;
using DecentraalBeheer.Vereniging;
using Detail;
using global::Marten;
using Grar.NutsLau;
using Historiek;
using KboSync;
using Locaties;
using Persoonsgegevens;
using PowerBiExport;

public static class ProjectionDocumentTypeRegistrations
{
    public static StoreOptions RegisterProjectionDocumentTypes(this StoreOptions opts)
    {
        opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
        opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();

        opts.RegisterDocumentType<PowerBiExportDocument>();

        opts.Schema.For<PowerBiExportDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<BewaartermijnDocument>();

        opts.Schema.For<BewaartermijnDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<PowerBiExportDubbelDetectieDocument>();

        opts.Schema.For<PowerBiExportDubbelDetectieDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<LocatieLookupDocument>();

        opts.Schema.For<LocatieLookupDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<LocatieZonderAdresMatchDocument>();

        opts.Schema.For<LocatieZonderAdresMatchDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<BeheerKboSyncHistoriekGebeurtenisDocument>();
        opts.RegisterDocumentType<VertegenwoordigerPersoonsgegevensDocument>();
        opts.RegisterDocumentType<BankrekeningnummerPersoonsgegevensDocument>();

        opts.RegisterDocumentType<PostalNutsLauInfo>();

        opts.RegisterDocumentType<VerenigingState>();

        return opts;
    }
}
