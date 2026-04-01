namespace AssociationRegistry.Test.Admin.AddressSync;

using AssociationRegistry.Admin.AddressSync;
using AssociationRegistry.Admin.AddressSync.Fetchers;
using AssociationRegistry.Admin.AddressSync.Infrastructure.Notifications;
using AssociationRegistry.Admin.AddressSync.MessageHandling.Sqs.AddressSync;
using AssociationRegistry.Grar.AdresMatch;
using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using Integrations.Grar.Integration.Messages;
using Integrations.Slack;
using Marten;
using MartenDb.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;

public class AddressSyncServiceTests
{
    [Fact]
    public async ValueTask Given_TeSynchroniserenLocatiesFetcher_Throws_Then_A_Notification_Is_Sent()
    {
    }

    [Fact]
    public async ValueTask Given_TeSynchroniserenLocatiesZonderAdresMatchFetcher_Throws_Then_A_Notification_Is_Sent()
    {
 }
}
