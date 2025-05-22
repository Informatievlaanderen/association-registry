namespace AssociationRegistry.Test.Admin.Api.HostedServices;

using AssociationRegistry.Admin.Api.HostedServices.GeotagsInitialisation;
using Common.StubsMocksFakes;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class GeotagsInitialisationServiceTests
{
    [Fact]
    public async Task Given_No_Verenigingen_Zonder_Geotags()
    {
        var faktory = Faktory.New();
        var query = faktory.VerenigingenZonderGeotagsQuery.Mock([]);
        var messagebus = faktory.MessageBus.Mock();

        var sut = new GeotagsInitialisationService(query.Object, messagebus.Object, new NullLogger<GeotagsInitialisationService>());

        await sut.StartAsync(CancellationToken.None);

        messagebus.VerifyNoOtherCalls();
    }
}
