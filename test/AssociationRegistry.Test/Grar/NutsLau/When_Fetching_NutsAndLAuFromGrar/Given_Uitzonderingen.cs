namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_NutsAndLAuFromGrar;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.NutsLau;
using FluentAssertions;
using Integrations.Grar.NutsLau;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_Uitzonderingen
{
    [Fact]
    public async ValueTask Then_We_Map_It_To_The_Correct_Nuts_Lau()
    {
        var client = new Mock<IGrarClient>();
        var overriddenPostcodes = NutsLauFromGrarFetcher.Uitzonderingen.Keys.OrderBy(_ => Guid.NewGuid()).Take(2).ToArray(); // random selection
        var expectedResults = overriddenPostcodes
                             .Select(code => NutsLauFromGrarFetcher.Uitzonderingen[code])
                             .ToArray();

        var postCodeFetcher = NutsLauSetupHelper.SetupPostCodeFetcher(overriddenPostcodes);

        var sut = new NutsLauFromGrarFetcher(client.Object, postCodeFetcher.Object, NullLogger<NutsLauFromGrarFetcher>.Instance);

        var actual = await sut.GetFlemishAndBrusselsNutsAndLauByPostcode(CancellationToken.None);

        actual.Should().BeEquivalentTo(expectedResults);

        client.Verify(x => x.GetPostalNutsLauInformation(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
