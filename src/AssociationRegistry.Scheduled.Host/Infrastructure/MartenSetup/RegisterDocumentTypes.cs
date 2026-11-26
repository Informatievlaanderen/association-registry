namespace AssociationRegistry.Scheduled.Host.Infrastructure.MartenSetup;

using Admin.ProjectionHost.Projections.PowerBiExport;
using Admin.Schema.Bewaartermijn;
using Admin.Schema.Erkenningen;
using Admin.Schema.Persoonsgegevens;
using Admin.Schema.PowerBiExport;
using Admin.Schema.Setup.Marten;
using DecentraalBeheer.Vereniging;
using DubbelBeheer.Schema;
using JasperFx.Events.Projections;
using Marten;
using MartenDb.Relatieaanvragen;

public static class RegisterDocumentTypes
{
    public static StoreOptions RegisterAdminDocumentTypes(this StoreOptions opts)
    {
        opts.RegisterDocumentType<VertegenwoordigerPersoonsgegevensDocument>();
        opts.RegisterDocumentType<BankrekeningnummerPersoonsgegevensDocument>();
        opts.RegisterDocumentType<BewaartermijnDocument>();
        opts.RegisterDocumentType<PowerBiExportDubbelDetectieDocument>();
        opts.RegisterDocumentType<PowerBiExportDocument>();
        opts.RegisterDocumentType<ErkenningDocument>();
        opts.RegisterDubbelImportDocumentTypes();

        opts.Schema.For<ErkenningDocument>().UseNumericRevisions(true).UseOptimisticConcurrency(false);

        opts.Schema.For<PowerBiExportDubbelDetectieDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.Schema.For<PowerBiExportDocument>().UseNumericRevisions(true).UseOptimisticConcurrency(false);

        opts.Schema.For<BewaartermijnDocument>().UseNumericRevisions(true).UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<VerenigingState>();

        // Verval reads the relatieaanvragen and deletes their guards: the same registrations as the other hosts.
        opts.RegisterRelatieaanvraagDocument();
        opts.RegisterOpenRelatieaanvraagDocument();

        opts.Projections.Add(new PowerBiExportProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new PowerBiExportDubbelDetectieProjection(), ProjectionLifecycle.Async);

        return opts;
    }

    public static StoreOptions RegisterDubbelImportDocumentTypes(this StoreOptions opts)
    {
        opts.RegisterDocumentType<DubbelImportDocument>();
        opts.Schema.For<DubbelImportDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false)
            .Duplicate(x => x.BatchId);

        opts.RegisterDocumentType<DubbelImportBatchDocument>();
        opts.Schema.For<DubbelImportBatchDocument>().UseOptimisticConcurrency(true);

        return opts;
    }
}
