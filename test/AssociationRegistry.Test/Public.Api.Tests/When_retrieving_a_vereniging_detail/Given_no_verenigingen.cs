namespace AssociationRegistry.Test.Public.Api.Tests.When_retrieving_a_vereniging_detail;

using AssociationRegistry.Public.Api.DetailVerenigingen;
using AssociationRegistry.Public.Api.ListVerenigingen;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stubs;
using Xunit;

public class Given_no_verenigingen
{
    private readonly Fixture _fixture;

    public Given_no_verenigingen()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Then_404_is_returned()
    {
        var controller =
            new DetailVerenigingenController(new VerenigingenRepositoryStub(new List<VerenigingListItem>()));
        var detailVerenigingContext = typeof(Scenario).GetAssociatedResourceJson("detail-vereniging-context");
        var context = JsonConvert.DeserializeObject<DetailVerenigingContext>(detailVerenigingContext)!;

        var response = await controller.Detail(context, _fixture.Create<string>());
        response.Should().BeOfType<NotFoundResult>();
    }
}
