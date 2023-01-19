namespace AssociationRegistry.Test.Public.Api.When_fetching_the_documentation;

using System.Net;
using Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

public class Then_The_Docs_Fixture : PublicApiFixture
{
    public Then_The_Docs_Fixture() : base(nameof(Then_The_Docs_Fixture))
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

public class Then_The_Docs : IClassFixture<Then_The_Docs_Fixture>
{
    private readonly Then_The_Docs_Fixture _theDocsFixture;

    public Then_The_Docs(Then_The_Docs_Fixture theDocsFixture)
        => _theDocsFixture = theDocsFixture;

    [Fact]
    public void Json_Returns_200OK()
        => _theDocsFixture.Response.Should().HaveStatusCode(HttpStatusCode.OK);

    [Fact]
    public async Task Have_Paths()
        => _theDocsFixture.Docs.Paths.Count.Should().BePositive();

    [Fact]
    public async Task Have_A_Summary_ForEach_Path()
        => _theDocsFixture.Docs.Paths
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
