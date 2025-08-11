namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AssociationRegistry.Grar.Models;
using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Clients;
using DecentraalBeheer.Vereniging;
using Vereniging;
using Xunit;

public class Returns_Multiple_Responses_With_One_Score100
{
    [Fact]
    public async ValueTask Then_AdresWerdOvergenomenUitAdressenregister()
    {
        var fixture = new Fixture().CustomizeDomain();
        var overTeNemenAdresId = fixture.Create<Registratiedata.AdresId>();

        var grarClient = new MockGrarClientBuilder(fixture)
                        .WithResponses(new MockScore(AddressMatchResponse.PerfectScore, overTeNemenAdresId),
                                    new MockScore(98))
                        .Build();

        var matchStrategy = new PerfectScoreMatchStrategy();
        var verrijkingService = new GemeenteVerrijkingService(grarClient.Object);
        var service = new AdresMatchService(grarClient.Object, matchStrategy, verrijkingService);

        var actual = await service.GetAdresMatchEvent(fixture.Create<int>(), fixture.Create<Locatie>(),
                                                     fixture.Create<VCode>(), CancellationToken.None);

        actual.Should().BeOfType<AdresWerdOvergenomenUitAdressenregister>();
        var @event = actual as AdresWerdOvergenomenUitAdressenregister;
        @event.AdresId.Should().BeEquivalentTo(overTeNemenAdresId);
    }
}
