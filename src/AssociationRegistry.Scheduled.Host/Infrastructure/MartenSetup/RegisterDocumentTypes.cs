namespace AssociationRegistry.Scheduled.Host.Infrastructure.MartenSetup;

using Admin.Schema.Bewaartermijn;
using Admin.Schema.Persoonsgegevens;
using DecentraalBeheer.Vereniging;
using Marten;

public static class RegisterDocumentTypes
{
    public static StoreOptions RegisterAdminDocumentTypes(this StoreOptions opts)
    {
        opts.RegisterDocumentType<VertegenwoordigerPersoonsgegevensDocument>();
        opts.RegisterDocumentType<BankrekeningnummerPersoonsgegevensDocument>();
        opts.RegisterDocumentType<BewaartermijnDocument>();

        opts.Schema.For<BewaartermijnDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        opts.RegisterDocumentType<VerenigingState>();

        return opts;
    }
}
