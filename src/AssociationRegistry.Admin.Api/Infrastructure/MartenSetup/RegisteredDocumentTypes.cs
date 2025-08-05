namespace AssociationRegistry.Admin.Api.Infrastructure.MartenSetup;

using Grar.NutsLau;
using HostedServices.GrarKafkaConsumer.Kafka;
using Hosts.Configuration.ConfigurationBindings;
using Magda.Models;
using Marten;
using Schema.Detail;
using Schema.Historiek;
using Schema.KboSync;
using Schema.PowerBiExport;
using Vereniging;

public static class RegisteredDocumentTypes
{
    public static StoreOptions RegisterDocumentTypes(this StoreOptions opts)
    {
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

        opts.RegisterDocumentType<AddressKafkaConsumerOffset>();

        opts.RegisterDocumentType<PostalNutsLauInfo>();

        opts.RegisterDocumentType<VerenigingState>();

        opts.RegisterDocumentType<SettingOverride>();

        opts.Schema.For<MagdaCallReference>().Identity(x => x.Reference);

        return opts;
    }
}
