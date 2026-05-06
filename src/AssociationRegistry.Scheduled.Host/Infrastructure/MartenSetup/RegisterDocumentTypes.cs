namespace AssociationRegistry.Scheduled.Host.Infrastructure.MartenSetup;

using Admin.ProjectionHost.Projections.PowerBiExport;
using Admin.Schema.Bewaartermijn;
using Admin.Schema.Persoonsgegevens;
using Admin.Schema.PowerBiExport;
using DecentraalBeheer.Vereniging;
using JasperFx.Events.Projections;
using Marten;

public static class RegisterDocumentTypes
{
    public static StoreOptions RegisterAdminDocumentTypes(this StoreOptions opts)
    {
        opts.RegisterDocumentType<VertegenwoordigerPersoonsgegevensDocument>();
        opts.RegisterDocumentType<BankrekeningnummerPersoonsgegevensDocument>();
        opts.RegisterDocumentType<BewaartermijnDocument>();
        opts.RegisterDocumentType<PowerBiExportDubbelDetectieDocument>();
        opts.RegisterDocumentType<PowerBiExportDocument>();

        opts.Schema.For<PowerBiExportDubbelDetectieDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.Schema.For<PowerBiExportDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.Schema.For<BewaartermijnDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<VerenigingState>();

        opts.Projections.Add(new PowerBiExportProjection(), ProjectionLifecycle.Async);
        opts.Projections.Add(new PowerBiExportDubbelDetectieProjection(), ProjectionLifecycle.Async);

        return opts;
    }
}
