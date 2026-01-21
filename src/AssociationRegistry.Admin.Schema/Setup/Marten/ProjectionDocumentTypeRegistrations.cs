namespace AssociationRegistry.Admin.Schema.Setup.Marten;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Admin.Schema.KboSync;
using AssociationRegistry.Admin.Schema.PowerBiExport;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Persoonsgegevens;
using Bewaartermijn;
using global::Marten;
using Persoonsgegevens;

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
