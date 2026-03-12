namespace AssociationRegistry.Admin.ExpiredBewaartermijnProcessor.Infrastructure.MartenSetup;

using DecentraalBeheer.Vereniging;
using Marten;
using Schema.Bewaartermijn;
using Schema.Persoonsgegevens;

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
