namespace AssociationRegistry.Admin.Api.Infrastructure.MartenSetup;

using HostedServices.GrarKafkaConsumer.Kafka;
using Hosts.Configuration.ConfigurationBindings;
using Integrations.Magda.Models;
using Marten;

public static class AdminDocumentTypeRegistrations
{
    public static StoreOptions RegisterAdminDocumentTypes(this StoreOptions opts)
    {
        opts.RegisterDocumentType<AddressKafkaConsumerOffset>();
        opts.RegisterDocumentType<SettingOverride>();
        opts.Schema.For<MagdaCallReference>().Identity(x => x.Reference);

        return opts;
    }
}
