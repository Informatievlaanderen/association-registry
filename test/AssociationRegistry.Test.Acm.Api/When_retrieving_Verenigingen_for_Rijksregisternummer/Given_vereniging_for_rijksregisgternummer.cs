namespace AssociationRegistry.Test.Acm.Api.When_retrieving_Verenigingen_for_Rijksregisternummer;

using System.Collections.Immutable;
using AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;
using FluentAssertions;
using Framework;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class Given_vereniging_for_rijksregisgternummer
{
    [Theory]
    [InlineData("7103654987")]
    [InlineData("7103")]
    public async Task Then_vereniging_is_returned(string rijksregisternummer)
    {
        var verenigingenRepository = new VerenigingenRepositoryStub();
        var controller = new VerenigingenPerRijksregisternummerController();
        var expectedVerenigingen = ImmutableArray.Create(
            new Vereniging("V0000001", "De eenzame in de lijst")
        );

        var response = (OkObjectResult)await controller.Get(verenigingenRepository, rijksregisternummer);
        var verenigingenResponse = (GetVerenigingenPerRijksregisternummerResponse)response.Value!;

        verenigingenResponse.Should()
            .BeEquivalentTo(new GetVerenigingenPerRijksregisternummerResponse(rijksregisternummer, expectedVerenigingen));
    }
}
