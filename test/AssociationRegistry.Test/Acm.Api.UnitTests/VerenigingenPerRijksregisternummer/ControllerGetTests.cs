namespace AssociationRegistry.Test.Acm.Api.UnitTests.VerenigingenPerRijksregisternummer;

using System.Collections.Immutable;
using AssociationRegistry.Acm.Api.Caches;
using AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class ControllerGetTests
{
    private readonly IVerenigingenRepository _verenigingenRepository;

    public ControllerGetTests()
    {
        _verenigingenRepository = new VerenigingenRepositoryStub();
    }

    [Fact]
    public async Task Test_7103654987()
    {
        var controller = new VerenigingenPerRijksregisternummerController();

        var response = (OkObjectResult)await controller.Get(_verenigingenRepository, "7103654987");

        var verenigingenResponse = (GetVerenigingenPerRijksregisternummerResponse)response.Value!;

        var expectedVerenigingen = ImmutableArray.Create(
            new Vereniging("V0000001", "De eenzame in de lijst")
        );
        verenigingenResponse.Should()
            .BeEquivalentTo(new GetVerenigingenPerRijksregisternummerResponse("7103654987", expectedVerenigingen));
    }

    [Fact]
    public async Task Test_7103()
    {
        var controller = new VerenigingenPerRijksregisternummerController();

        var response = (OkObjectResult)await controller.Get(_verenigingenRepository, "7103");

        var verenigingenResponse = (GetVerenigingenPerRijksregisternummerResponse)response.Value!;

        var expectedVerenigingen = ImmutableArray.Create(
            new Vereniging("V0000001", "De eenzame in de lijst")
        );
        verenigingenResponse.Should()
            .BeEquivalentTo(new GetVerenigingenPerRijksregisternummerResponse("7103", expectedVerenigingen));
    }

    [Theory]
    [InlineData("55120154321")]
    [InlineData("123")]
    [InlineData("GEENCONTROLEOPRRNR")]
    public async Task Test_NotFound(string rijksregisternummer)
    {
        var controller = new VerenigingenPerRijksregisternummerController();

        var response = (OkObjectResult)await controller.Get(_verenigingenRepository, rijksregisternummer);

        var verenigingenResponse = (GetVerenigingenPerRijksregisternummerResponse)response.Value!;

        verenigingenResponse.Should()
            .BeEquivalentTo(new GetVerenigingenPerRijksregisternummerResponse(rijksregisternummer, ImmutableArray<Vereniging>.Empty));
    }
}

public class VerenigingenRepositoryStub : IVerenigingenRepository
{
    public VerenigingenPerRijksregisternummer Verenigingen { get; set; } =
        VerenigingenPerRijksregisternummer
            .FromVerenigingenAsDictionary(
                new VerenigingenAsDictionary()
                {
                    {
                        "7103",
                        new()
                        {
                            { "V0000001", "De eenzame in de lijst" },
                        }
                    },
                    {
                        "9803",
                        new()
                        {
                            { "V1234567", "FWA De vrolijke BA’s" },
                            { "V7654321", "FWA De Bron" },
                        }
                    },
                }
            );

    public Task UpdateVerenigingen(VerenigingenAsDictionary verenigingenAsDictionary, Stream verenigingenStream,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
