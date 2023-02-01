namespace AssociationRegistry.Test.Admin.Api.When_fetching_the_documentation;

using System.Net;
using Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

public class Given_Docs_Exist_Fixture : AdminApiFixture
{
    public Given_Docs_Exist_Fixture() : base(nameof(Given_Docs_Exist_Fixture))
    {
    }

    public HttpResponseMessage Response { get; set; } = null!;

    public Docs? Docs { get; set; }

    protected override Task Given()
        => Task.CompletedTask;

    protected override async Task When()
    {
        Response = await AdminApiClient.GetDocs();
        Docs = JsonConvert.DeserializeObject<Docs>(await Response.Content.ReadAsStringAsync());
    }
}

public class Given_Docs_Exist : IClassFixture<Given_Docs_Exist_Fixture>
{
    private readonly Given_Docs_Exist_Fixture _theDocsFixture;

    public Given_Docs_Exist(Given_Docs_Exist_Fixture theDocsFixture)
        => _theDocsFixture = theDocsFixture;

    [Fact]
    public void Then_it_returns_an_ok_response()
        => _theDocsFixture.Response.Should().HaveStatusCode(HttpStatusCode.OK);

    [Fact]
    public void Then_it_has_paths()
        => _theDocsFixture.Docs!.Paths!.Count.Should().BePositive();

    [Fact]
    public void Then_it_as_a_summary_for_each_path()
        => _theDocsFixture.Docs!.Paths!
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

    public Dictionary<string, Dictionary<string, DocsPath>>? Paths { get; set; }
}
