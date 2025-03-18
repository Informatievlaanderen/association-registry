namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Grar.AdresMatch;
using Grar.Models;
using Vereniging;
using Xunit;

public class Returns_Multiple_100Score_Responses
{
    [Fact]
    public async Task Then_AdresNietUniekInAdressenregister()
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
