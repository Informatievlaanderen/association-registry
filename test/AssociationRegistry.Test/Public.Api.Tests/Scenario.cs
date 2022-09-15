namespace AssociationRegistry.Test.Public.Api.Tests;

using AssociationRegistry.Public.Api.DetailVerenigingen;
using Newtonsoft.Json;
using AssociationRegistry.Public.Api.ListVerenigingen;
using Stubs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

public static class Scenario
{
    public static async Task<ListVerenigingenResponse> When_retrieving_a_list_of_verenigingen(
        List<VerenigingListItem> verenigingen, int offset, int limit)
    {
        var verenigingenRepositoryStub = new VerenigingenRepositoryStub(verenigingen);
        var controller = new ListVerenigingenController(verenigingenRepositoryStub);

        var response = await controller.List(
            null!,
            new PaginationQueryParams(offset, limit));

        return response.Should().BeOfType<OkObjectResult>()
            .Which.Value?.Should().BeOfType<ListVerenigingenResponse>()
            .Subject!;
    }

    public static async Task<ListVerenigingenResponse> When_retrieving_a_list_of_verenigingen(
        List<VerenigingListItem> verenigingen)
    {
        var verenigingenRepositoryStub = new VerenigingenRepositoryStub(verenigingen);
        var controller = new ListVerenigingenController(verenigingenRepositoryStub);

        var listVerenigingenContext = typeof(Scenario).GetAssociatedResourceJson("list-verenigingen-context");
        var deserializeContext = JsonConvert.DeserializeObject<ListVerenigingContext>(listVerenigingenContext);

        var response = await controller.List(
            deserializeContext ?? throw new Exception("Could not deserialize list-verenigingen-context.json"),
            new PaginationQueryParams());

        return response.Should().BeOfType<OkObjectResult>()
            .Which.Value?.Should().BeOfType<ListVerenigingenResponse>()
            .Subject!;
    }

    public static async Task<DetailVerenigingResponse> When_retrieving_a_vereniging_detail(VerenigingDetail vereniging)
    {
        var verenigingenRepositoryStub = new VerenigingenRepositoryStub(new List<VerenigingDetail> { vereniging });
        var controller = new DetailVerenigingenController(verenigingenRepositoryStub);

        var detailVerenigingContext = typeof(Scenario).GetAssociatedResourceJson("detail-vereniging-context");
        var deserializeContext = JsonConvert.DeserializeObject<DetailVerenigingContext>(detailVerenigingContext);

        var response =
            await controller.Detail(
                deserializeContext ?? throw new Exception("Could not deserialize detail-vereniging-context.json"),
                vereniging.Id);

        return response.Should().BeOfType<OkObjectResult>()
            .Which.Value?.Should().BeOfType<DetailVerenigingResponse>()
            .Subject!;
    }
}
