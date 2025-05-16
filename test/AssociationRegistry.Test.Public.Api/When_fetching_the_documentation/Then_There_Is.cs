namespace AssociationRegistry.Test.Public.Api.When_fetching_the_documentation;

using Fixtures.GivenEvents;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Then_The_Docs
{
    public Then_The_Docs(GivenEventsFixture fixture)
    {
        Response = fixture.PublicApiClient.GetDocs().GetAwaiter().GetResult();
        Docs = JsonConvert.DeserializeObject<Docs>(Response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }

    public HttpResponseMessage Response { get; }
    public Docs? Docs { get; }

    [Fact]
    public void Json_Returns_200OK()
        => Response.Should().HaveStatusCode(HttpStatusCode.OK);

    [Fact]
    public void Have_Paths()
        => Docs!.Paths.Count.Should().BePositive();

    [Fact]
    public void Have_A_Summary_ForEach_Path()
        => Docs!.Paths
                .ToList()
                .ForEach(path =>
                             path.Value.ToList().ForEach(method => method.Value.Summary.Should()
                                                                         .NotBeNullOrWhiteSpace(
                                                                              because:
                                                                              $"'[{method.Key}] {path.Key}' should have a summary.")));
}

public class Docs
{
    public class DocsPath
    {
        public string? Summary { get; set; }
    }

    public Dictionary<string, Dictionary<string, DocsPath>> Paths { get; set; } = new();
}
