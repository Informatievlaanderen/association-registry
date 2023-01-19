namespace AssociationRegistry.Test.Public.Api.When_fetching_the_documentation;

using System.Net;
using Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

public class Then_Fixture : PublicApiFixture
{
    public Then_Fixture() : base(nameof(Then_Fixture))
    {
        Response = PublicApiClient.GetDocs().GetAwaiter().GetResult();
        Docs = JsonConvert.DeserializeObject<Docs>(Response.Content.ReadAsStringAsync().GetAwaiter().GetResult());

    }

    public override async Task InitializeAsync()
    {
    }

    public HttpResponseMessage Response { get; set; } = null!;

    public Docs? Docs { get; set; }
}

public class Then : IClassFixture<Then_Fixture>
{
    private readonly Then_Fixture _fixture;

    public Then(Then_Fixture fixture)
        => _fixture = fixture;

    [Fact]
    public void The_DocsJson_Returns_200OK()
        => _fixture.Response.Should().HaveStatusCode(HttpStatusCode.OK);

    [Fact]
    public async Task There_Are_Paths()
        => _fixture.Docs.Paths.Count.Should().BePositive();

    [Fact]
    public async Task Each_Path_Has_A_Summary()
        => _fixture.Docs.Paths
            .ToList()
            .ForEach(
                path =>
                    path.Value.ToList().ForEach(
                        method => method.Value.Summary.Should().NotBeNullOrWhiteSpace(
                            because: $"'[{method.Key}] {path.Key}' should have a summary.")));
}

public class Docs
{
    public class DocsPath
    {
        public string? Summary { get; set; }
    }

    public Dictionary<string, Dictionary<string, DocsPath>> Paths { get; set; }
}
