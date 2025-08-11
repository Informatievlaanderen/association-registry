namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using AssociationRegistry.Grar.AdresMatch;
using DecentraalBeheer.Vereniging;
using Vereniging;
using Xunit;

public class Returns_Single_Response_With_Score100
{
    [Fact]
    public async ValueTask Then_AdresWerdOvergenomenUitAdressenregister()
    {
        var fixture = new Fixture().CustomizeDomain();

        var grarClient = new MockGrarClientBuilder(fixture)
                        .WithResponses(new MockScore(100))
                        .Build();

      var actual =  await LegacyAdresMatchWrapperService.GetAdresMatchEvent(fixture.Create<int>(), fixture.Create<Locatie>(), grarClient.Object,
                                                   CancellationToken.None, fixture.Create<VCode>());

      actual.Should().BeOfType<AdresWerdOvergenomenUitAdressenregister>();
    }
}
