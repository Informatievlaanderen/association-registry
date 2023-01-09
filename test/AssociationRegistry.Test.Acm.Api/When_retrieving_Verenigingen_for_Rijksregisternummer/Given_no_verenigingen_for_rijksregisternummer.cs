namespace AssociationRegistry.Test.Acm.Api.When_retrieving_Verenigingen_for_Rijksregisternummer;

using System.Collections.Immutable;
using AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;
using FluentAssertions;
using Framework;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class Given_no_verenigingen_for_rijksregisternummer
{
    [Theory]
    [InlineData("55120154321")]
    [InlineData("123")]
    [InlineData("GEENCONTROLEOPRRNR")]
    public async Task Then_an_empty_list_is_returned(string rijksregisternummer)
    {
        var verenigingenRepository = new VerenigingenRepositoryStub();
        var controller = new VerenigingenPerRijksregisternummerController();
        var expectedVerenigingen = ImmutableArray<Vereniging>.Empty;

        var response = (OkObjectResult)await controller.Get(verenigingenRepository, rijksregisternummer);
        var verenigingenResponse = (GetVerenigingenPerRijksregisternummerResponse)response.Value!;

        verenigingenResponse.Should()
            .BeEquivalentTo(new GetVerenigingenPerRijksregisternummerResponse(rijksregisternummer, expectedVerenigingen));
    }
}