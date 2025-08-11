namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AssociationRegistry.Grar.Models;
using AutoFixture;
using Common.AutoFixture;
using Events;
using Vereniging;
using Xunit;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Clients;
using DecentraalBeheer.Vereniging;
using FluentAssertions;

public class Returns_Multiple_100Score_Responses
{
    [Fact]
    public async ValueTask Then_AdresNietUniekInAdressenregister()
    {
        var fixture = new Fixture().CustomizeDomain();

        var grarClient = new MockGrarClientBuilder(fixture)
                        .WithResponses(new MockScore(AddressMatchResponse.PerfectScore),
                                    new MockScore(AddressMatchResponse.PerfectScore))
                        .Build();

        var matchStrategy = new PerfectScoreMatchStrategy();
        var verrijkingService = new GemeenteVerrijkingService(grarClient.Object);
        var service = new AdresMatchService(grarClient.Object, matchStrategy, verrijkingService);

        var actual = await service.GetAdresMatchEvent(fixture.Create<int>(), fixture.Create<Locatie>(),
                                                     fixture.Create<VCode>(), CancellationToken.None);

        actual.Should().BeOfType<AdresNietUniekInAdressenregister>();
    }
}
