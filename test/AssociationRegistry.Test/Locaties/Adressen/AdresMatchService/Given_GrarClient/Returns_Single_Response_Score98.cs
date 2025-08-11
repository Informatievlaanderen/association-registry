namespace AssociationRegistry.Test.Locaties.Adressen.AdresMatchService.Given_GrarClient;

using AssociationRegistry.Events;
using AssociationRegistry.Grar.AdresMatch;
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

      var actual =  await LegacyAdresMatchWrapperService.GetAdresMatchEvent(fixture.Create<int>(), fixture.Create<Locatie>(), grarClient.Object,
                                                   CancellationToken.None, fixture.Create<VCode>());

      actual.Should().BeOfType<AdresNietUniekInAdressenregister>();
    }
}
