namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_NutsAndLAuFromGrar;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
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

        var sut = new NutsLauFromGrarFetcher(client.Object);

        var postcodes = expectedResults.Select(x => x.Postcode).ToArray();
        var actual = await sut.GetFlemishAndBrusselsNutsAndLauByPostcode(postcodes, CancellationToken.None);

        actual.Should().BeEquivalentTo(expectedResults);

        client.Verify(x => x.GetPostalNutsLauInformation(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
