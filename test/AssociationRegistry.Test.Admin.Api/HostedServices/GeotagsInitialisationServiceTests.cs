namespace AssociationRegistry.Test.Admin.Api.HostedServices;

using AssociationRegistry.Admin.Api.HostedServices.GeotagsInitialisation;
using AssociationRegistry.DecentraalBeheer.Geotags.InitialiseerGeotags;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Extensions;
using Common.StubsMocksFakes;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
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

    [Fact]
    public async Task Given_Verenigingen_Met_Geotags()
    {
        var faktory = Faktory.New();
        var fixture = new Fixture().CustomizeAdminApi();
        var vCodes = fixture.CreateMany<VCode>();

        var query = faktory.VerenigingenZonderGeotagsQuery.Mock(vCodes.Select(x => x.ToString()));
        var messagebus = faktory.MessageBus.Mock();

        var sut = new GeotagsInitialisationService(query.Object, messagebus.Object, new NullLogger<GeotagsInitialisationService>());

        await sut.StartAsync(CancellationToken.None);

        foreach (var vCode in vCodes)
        {
            var commandEnvelope = new CommandEnvelope<InitialiseerGeotagsCommand>(
                new InitialiseerGeotagsCommand(vCode), CommandMetadata.ForDigitaalVlaanderenProcess);

            messagebus.VerifyCommand<InitialiseerGeotagsCommand>(x => x.Command == commandEnvelope.Command &&
                                     x.Metadata.Initiator == CommandMetadata.ForDigitaalVlaanderenProcess.Initiator &&
                                     x.Metadata.ExpectedVersion == CommandMetadata.ForDigitaalVlaanderenProcess.ExpectedVersion, Times.Once());
        }
    }
}
