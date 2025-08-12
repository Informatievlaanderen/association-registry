namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_NutsAndLAuFromGrar;

using AssociationRegistry.Grar;
using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using AssociationRegistry.Integrations.Grar.Clients;
using Integrations.Grar.NutsLau;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_A_Grar_Returns_No_Null_For_A_PostalCode
{

    [Fact]
    public async ValueTask Then_Ignores_The_Null_Response()
    {
        var fixture = new Fixture().CustomizeDomain();
        var client = new Mock<IGrarClient>();
        string[] postcodes = ["1500", "1501"];
        var nutsLauResponse = fixture.Create<PostalNutsLauInfoResponse>();

        var postCodeFetcher = NutsLauSetupHelper.SetupPostCodeFetcher(postcodes);
        NutsLauSetupHelper.SetupGrarClient(client, postcodes[0], nutsLauResponse);
        NutsLauSetupHelper.SetupGrarClient(client, postcodes[1], null);

        var sut = new NutsLauFromGrarFetcher(client.Object,
                                             postCodeFetcher.Object,
                                             NullLogger<NutsLauFromGrarFetcher>.Instance);

        var actual = await sut.GetFlemishAndBrusselsNutsAndLauByPostcode(CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new PostalNutsLauInfo()
            {
                Postcode = nutsLauResponse.Postcode,
                Gemeentenaam = nutsLauResponse.Gemeentenaam,
                Nuts3 = nutsLauResponse.Nuts,
                Lau = nutsLauResponse.Lau,
            },
        ]);
    }
}
