﻿namespace AssociationRegistry.Test.Grar.NutsLau.When_Fetching_NutsAndLAuFromGrar;

using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Grar.Models.PostalInfo;
using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
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
        SetupGrarClient(client, postcodes[0], nutsLauResponse);
        SetupGrarClient(client, postcodes[1], null);

        var sut = new NutsLauFromGrarFetcher(client.Object);

        var actual = await sut.GetFlemishAndBrusselsNutsAndLauByPostcode(postcodes, CancellationToken.None);

        actual.Should().BeEquivalentTo([
            new PostalNutsLauInfo()
            {
                Postcode = nutsLauResponse.Postcode,
                Gemeentenaam = nutsLauResponse.Gemeentenaam,
                Nuts3 = nutsLauResponse.Nuts,
                Lau = nutsLauResponse.Lau,
            }
        ]);
    }

    private void SetupGrarClient(Mock<IGrarClient> client, string postalcode, PostalNutsLauInfoResponse response)
    {
        client.Setup(x => x.GetPostalNutsLauInformation(postalcode, It.IsAny<CancellationToken>()))
              .ReturnsAsync(response);
    }
}
