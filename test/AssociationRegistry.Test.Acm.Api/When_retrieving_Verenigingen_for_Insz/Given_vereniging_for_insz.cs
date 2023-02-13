namespace AssociationRegistry.Test.Acm.Api.When_retrieving_Verenigingen_for_Insz;

using System.Collections.Immutable;
using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class Given_vereniging_for_insz
{
    [Theory]
    [InlineData("7103654987")]
    [InlineData("7103")]
    public async Task Then_vereniging_is_returned(string insz)
    {
        var verenigingenRepository = new VerenigingenRepositoryStub();
        var controller = new VerenigingenPerInszController();
        var expectedVerenigingen = ImmutableArray.Create(
            new Vereniging("V0000001", "De eenzame in de lijst")
        );

        var response = (OkObjectResult)await controller.Get(verenigingenRepository, insz);
        var verenigingenResponse = (VerenigingenPerInszResponse)response.Value!;

        verenigingenResponse.Should()
            .BeEquivalentTo(new VerenigingenPerInszResponse(insz, expectedVerenigingen));
    }
}
