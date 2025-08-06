namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using AssociationRegistry.Grar.AdresMatch;
using DecentraalBeheer.Vereniging;
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

      var actual =  await AdresMatchService.GetAdresMatchEvent(fixture.Create<int>(), fixture.Create<Locatie>(), grarClient.Object,
                                                   CancellationToken.None, fixture.Create<VCode>());

      actual.Should().BeOfType<AdresWerdNietGevondenInAdressenregister>();
    }
}
