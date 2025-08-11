namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AssociationRegistry.Events;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Returns_Single_Response_Score98
{
    [Fact]
    public async ValueTask Then_AdresNietUniekInAdressenregister()
    {
        var fixture = new Fixture().CustomizeDomain();

        var grarClient = new MockGrarClientBuilder(fixture)
           .WithResponses(new MockScore(98))
                        .Build();

        var matchStrategy = new PerfectScoreMatchStrategy();
        var verrijkingService = new GemeenteVerrijkingService(grarClient.Object);
        var service = new AdresMatchService(grarClient.Object, matchStrategy, verrijkingService);

        var actual = await service.GetAdresMatchEvent(fixture.Create<int>(), fixture.Create<Locatie>(),
                                                     fixture.Create<VCode>(), CancellationToken.None);

        actual.Should().BeOfType<AdresNietUniekInAdressenregister>();
    }
}
