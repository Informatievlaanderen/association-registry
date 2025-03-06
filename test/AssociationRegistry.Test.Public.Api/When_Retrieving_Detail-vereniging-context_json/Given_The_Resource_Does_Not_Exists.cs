﻿namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail_vereniging_context_json;

using Fixtures;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

[Collection(VerenigingPublicApiCollection.Name)]
[Category("PublicApi")]
[IntegrationTest]
public class Given_The_Resource_Does_Not_Exists : IClassFixture<StaticPublicApiFixture>
{
    private readonly HttpClient _httpClient;

    public Given_The_Resource_Does_Not_Exists(StaticPublicApiFixture fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    public async Task Then_we_get_a_notFound_response()
    {
        var response = await _httpClient.GetAsync("DoesNotExist.json");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
