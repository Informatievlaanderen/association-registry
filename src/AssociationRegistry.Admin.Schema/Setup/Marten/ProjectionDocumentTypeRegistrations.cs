namespace AssociationRegistry.Admin.Schema.Setup.Marten;

using global::Marten;
using Bewaartermijn;
using DecentraalBeheer.Vereniging;
using Detail;
using Erkenningen;
using Grar.NutsLau;
using Historiek;
using KboSync;
using Locaties;
using Persoonsgegevens;
using PowerBiExport;
using Relatieaanvragen;

public static class ProjectionDocumentTypeRegistrations
{
    public static StoreOptions RegisterProjectionDocumentTypes(this StoreOptions opts)
    {
        opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
        opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();

        opts.RegisterDocumentType<PowerBiExportDocument>();

        opts.Schema.For<PowerBiExportDocument>().UseNumericRevisions(true).UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<BewaartermijnDocument>();

        opts.Schema.For<BewaartermijnDocument>().UseNumericRevisions(true).UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<PowerBiExportDubbelDetectieDocument>();

        opts.Schema.For<PowerBiExportDubbelDetectieDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<LocatieLookupDocument>();

        opts.Schema.For<LocatieLookupDocument>().UseNumericRevisions(true).UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<LocatieZonderAdresMatchDocument>();

        opts.Schema.For<LocatieZonderAdresMatchDocument>().UseNumericRevisions(true).UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<ErkenningDocument>();

        opts.Schema.For<ErkenningDocument>().UseNumericRevisions(true).UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<BeheerKboSyncHistoriekGebeurtenisDocument>();
        opts.RegisterDocumentType<VertegenwoordigerPersoonsgegevensDocument>();
        opts.RegisterDocumentType<BankrekeningnummerPersoonsgegevensDocument>();

        opts.RegisterDocumentType<PostalNutsLauInfo>();

        opts.RegisterDocumentType<VerenigingState>();

        opts.RegisterRelatieaanvraagDocument();

        return opts;
    }

    /// <summary>Also used by the Admin API: both hosts must configure the table the same way.</summary>
    public static StoreOptions RegisterRelatieaanvraagDocument(this StoreOptions opts)
    {
        opts.RegisterDocumentType<RelatieaanvraagDocument>();

        // A single stream projection gets numeric revisions (mt_version bigint) in the projection host.
        opts.Schema.For<RelatieaanvraagDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false)
            .Index(x => x.KoepelVCode)
            .Index(x => x.VerenigingVCode);

        return opts;
    }
}
