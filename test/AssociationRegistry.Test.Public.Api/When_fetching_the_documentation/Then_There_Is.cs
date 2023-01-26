namespace AssociationRegistry.Test.Public.Api.When_fetching_the_documentation;

using System.Net;
using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]

public class Then_The_Docs
{
    private readonly GivenEventsFixture _fixture;

    public Then_The_Docs(GivenEventsFixture fixture)
    {
        _fixture = fixture;
        Response = fixture.PublicApiClient.GetDocs().GetAwaiter().GetResult();
        Docs = JsonConvert.DeserializeObject<Docs>(Response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

    }

    public HttpResponseMessage Response { get; set; } = null!;

    public Docs? Docs { get; set; }

    [Fact]
    public void Json_Returns_200OK()
        => Response.Should().HaveStatusCode(HttpStatusCode.OK);

    [Fact]
    public async Task Have_Paths()
        => Docs.Paths.Count.Should().BePositive();

    [Fact]
    public async Task Have_A_Summary_ForEach_Path()
        => Docs.Paths
            .ToList()
            .ForEach(path =>
                path.Value.ToList().ForEach(method => method.Value.Summary.Should().NotBeNullOrWhiteSpace(because: $"'[{method.Key}] {path.Key}' should have a summary.")));
}

public class Docs
{
    public class DocsPath
    {
        public string? Summary { get; set; }
    }

    public Dictionary<string, Dictionary<string, DocsPath>> Paths { get; set; }
}
