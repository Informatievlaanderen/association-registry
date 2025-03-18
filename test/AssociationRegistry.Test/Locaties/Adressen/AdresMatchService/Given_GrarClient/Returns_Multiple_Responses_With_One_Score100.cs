namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Grar.AdresMatch;
using Grar.Models;
using Vereniging;
using Xunit;

public class Returns_Multiple_Responses_With_One_Score100
{
    [Fact]
    public async Task Then_AdresWerdOvergenomenUitAdressenregister()
    {
        var fixture = new Fixture().CustomizeDomain();
        var overTeNemenAdresId = fixture.Create<Registratiedata.AdresId>();

        var grarClient = new MockGrarClientBuilder(fixture)
                        .WithResponses(new MockScore(AddressMatchResponse.PerfectScore, overTeNemenAdresId),
                                    new MockScore(98))
                        .Build();

        var actual =  await AdresMatchService.GetAdresMatchEvent(fixture.Create<int>(), fixture.Create<Locatie>(), grarClient.Object,
                                                                 CancellationToken.None, fixture.Create<VCode>());

        actual.Should().BeOfType<AdresWerdOvergenomenUitAdressenregister>();
        var @event = actual as AdresWerdOvergenomenUitAdressenregister;
        @event.AdresId.Should().BeEquivalentTo(overTeNemenAdresId);
    }
}
