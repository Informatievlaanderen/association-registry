namespace AssociationRegistry.Admin.Api.Infrastructure.MartenSetup;

using DecentraalBeheer.Vereniging;
using Grar.NutsLau;
using HostedServices.GrarKafkaConsumer.Kafka;
using Hosts.Configuration.ConfigurationBindings;
using Integrations.Magda.Models;
using Marten;
using Schema.Detail;
using Schema.Historiek;
using Schema.KboSync;
using Schema.Persoonsgegevens;
using Schema.PowerBiExport;

public static class AdminDocumentTypeRegistrations
{
    public static StoreOptions RegisterAdminDocumentTypes(this StoreOptions opts)
    {
        opts.RegisterDocumentType<AddressKafkaConsumerOffset>();
        opts.RegisterDocumentType<SettingOverride>();
        opts.Schema.For<MagdaCallReference>().Identity(x => x.Reference);


        opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
        opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();

        opts.RegisterDocumentType<PowerBiExportDocument>();
        opts.Schema.For<PowerBiExportDocument>()
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

        opts.RegisterDocumentType<PostalNutsLauInfo>();
        opts.RegisterDocumentType<VertegenwoordigerPersoonsgegevensDocument>();

        opts.RegisterDocumentType<VerenigingState>();

        return opts;
    }
}
