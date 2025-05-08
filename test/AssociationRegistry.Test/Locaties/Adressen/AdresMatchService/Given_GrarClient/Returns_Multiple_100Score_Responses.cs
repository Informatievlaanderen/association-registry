namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AssociationRegistry.Grar.Models;
using AutoFixture;
using Common.AutoFixture;
using Events;
using Vereniging;
using Xunit;
using AssociationRegistry.Grar.AdresMatch;
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

      var actual =  await AdresMatchService.GetAdresMatchEvent(fixture.Create<int>(), fixture.Create<Locatie>(), grarClient.Object,
                                                   CancellationToken.None, fixture.Create<VCode>());

      actual.Should().BeOfType<AdresNietUniekInAdressenregister>();
    }
}
