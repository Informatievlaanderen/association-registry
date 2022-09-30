namespace AssociationRegistry.Test.Public.Api.UnitTests;

using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.DetailVerenigingen;
using AssociationRegistry.Public.Api.Infrastructure.Json;
using AssociationRegistry.Public.Api.ListVerenigingen;
using Stubs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public static class Scenario
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        Converters = new List<JsonConverter> { new NullableDateOnlyJsonConvertor("yyyy-MM-dd") },
    };

    public static async Task<ListVerenigingenResponse> When_retrieving_a_list_of_verenigingen(
        List<VerenigingListItem> verenigingen,
        int offset,
        int limit)
    {
        var verenigingenRepositoryStub = new VerenigingenRepositoryStub(verenigingen);
        var controller = new ListVerenigingenController(verenigingenRepositoryStub);

        var response = await controller.List(
            new AppSettings { AssociationRegistryUri = "http://localhost:11003/" },
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

        var response = await controller.List(
            new AppSettings { AssociationRegistryUri = "http://localhost:11003/" },
            new PaginationQueryParams());

        return response.Should().BeOfType<OkObjectResult>()
            .Which.Value?.Should().BeOfType<ListVerenigingenResponse>()
            .Subject!;
    }

    public static async Task<DetailVerenigingResponse> When_retrieving_a_vereniging_detail(VerenigingDetail vereniging)
    {
        var verenigingenRepositoryStub = new VerenigingenRepositoryStub(new List<VerenigingDetail> { vereniging });
        var controller = new DetailVerenigingenController(verenigingenRepositoryStub);

        var response =
            await controller.Detail(
                new AppSettings { AssociationRegistryUri = "http://localhost:11003/" },
                vereniging.Id);

        return response.Should().BeOfType<OkObjectResult>()
            .Which.Value?.Should().BeOfType<DetailVerenigingResponse>()
            .Subject!;
    }
}
