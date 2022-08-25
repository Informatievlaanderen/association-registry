using AssociationRegistry.Acm.Api.Caches;
using AssociationRegistry.Acm.Api.Infrastructure;

namespace AssociationRegistry.Test.Acm.Api.Tests.VerenigingenPerRijksregisternummer;

using System.Collections.Immutable;
using AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class ControllerGetTests
{
    private Data _data;

    public ControllerGetTests()
    {
        _data = new Data()
        {
            Verenigingen = new Dictionary<string, ImmutableArray<Vereniging>>
            {
                {
                    "7103654987",
                    ImmutableArray.Create(
                        new Vereniging("V0000001", "De eenzame in de lijst"))
                },
                {
                    "980365494",
                    ImmutableArray.Create(
                        new Vereniging("V1234567", "FWA De vrolijke BA’s"),
                        new Vereniging("V7654321", "FWA De Bron")
                    )
                }
            }
        };
    }

    public async Task Test_7103()
    {
        var controller = new VerenigingenPerRijksregisternummerController();

        var response = (OkObjectResult)await controller.Get(_data, "7103654987");

        var verenigingenResponse = (GetVerenigingenResponse)response.Value!;

        var expectedVerenigingen = ImmutableArray.Create(
            new Vereniging("V1234567", "FWA De vrolijke BA’s"),
            new Vereniging("V7654321", "FWA De Bron")
        );
        verenigingenResponse.Should().BeEquivalentTo(new GetVerenigingenResponse("7103654987", expectedVerenigingen));
    }

    public async Task Test_9803()
    {
        var controller = new VerenigingenPerRijksregisternummerController();

        var response = (OkObjectResult)await controller.Get(_data, "980365494");

        var verenigingenResponse = (GetVerenigingenResponse)response.Value!;

        var expectedVerenigingen = ImmutableArray.Create(
            new Vereniging("V0000001", "De eenzame in de lijst")
        );
        verenigingenResponse.Should().BeEquivalentTo(new GetVerenigingenResponse("980365494", expectedVerenigingen));
    }

    [Theory]
    [InlineData("55120154321")]
    [InlineData("123")]
    [InlineData("GEENCONTROLEOPRRNR")]
    public async Task Test_NotFound(string rijksregisternummer)
    {
        var controller = new VerenigingenPerRijksregisternummerController();

        var response = (OkObjectResult)await controller.Get(_data, rijksregisternummer);

        var verenigingenResponse = (GetVerenigingenResponse)response.Value!;

        verenigingenResponse.Should()
            .BeEquivalentTo(new GetVerenigingenResponse(rijksregisternummer, ImmutableArray<Vereniging>.Empty));
    }

    private static void VerifyVereniging(Vereniging vereniging, string expectedId, string expectedName)
    {
        vereniging.Id.Should().Be(expectedId);
        vereniging.Naam.Should().Be(expectedName);
    }
}
