namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using AssociationRegistry.Grar.AdresMatch;
using DecentraalBeheer.Vereniging;
using AssociationRegistry.Integrations.Grar.AdresMatch;
using Vereniging;
using Xunit;

public class Returns_No_Responses
{
    [Fact]
    public async ValueTask Then_AdresNietUniekInAdressenregister()
    {
        var fixture = new Fixture().CustomizeDomain();

        var grarClient = new MockGrarClientBuilder(fixture)
                        .WithNoResponses()
                        .Build();

        var matchStrategy = new PerfectScoreMatchStrategy();
        var verrijkingService = new GrarAddressVerrijkingsService(grarClient.Object);
        var service = new AdresMatchService(grarClient.Object, matchStrategy, verrijkingService);

        var actual = await service.GetAdresMatchEvent(fixture.Create<int>(), fixture.Create<Locatie>(),
                                                     fixture.Create<VCode>(), CancellationToken.None);

        actual.Should().BeOfType<AdresWerdNietGevondenInAdressenregister>();
    }
}
