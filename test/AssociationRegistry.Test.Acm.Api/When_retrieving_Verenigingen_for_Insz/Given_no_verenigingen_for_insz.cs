namespace AssociationRegistry.Test.Acm.Api.When_retrieving_Verenigingen_for_Insz;

using System.Collections.Immutable;
using AssociationRegistry.Acm.Api.VerenigingenPerInsz;
using Framework;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class Given_no_verenigingen_for_insz
{
    [Theory]
    [InlineData("55120154321")]
    [InlineData("123")]
    [InlineData("GEENCONTROLEOPRRNR")]
    public async Task Then_an_empty_list_is_returned(string insz)
    {
        var verenigingenRepository = new VerenigingenRepositoryStub();
        var controller = new VerenigingenPerInszController();
        var expectedVerenigingen = ImmutableArray<Vereniging>.Empty;

        var response = (OkObjectResult)await controller.Get(verenigingenRepository, insz);
        var verenigingenResponse = (VerenigingenPerInszResponse)response.Value!;

        verenigingenResponse.Should()
            .BeEquivalentTo(new VerenigingenPerInszResponse(insz, expectedVerenigingen));
    }
}
