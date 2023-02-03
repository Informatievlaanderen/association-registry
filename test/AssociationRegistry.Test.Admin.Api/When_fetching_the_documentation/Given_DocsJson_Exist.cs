namespace AssociationRegistry.Test.Admin.Api.When_fetching_the_documentation;

using System.Net;
using Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

//TODO Move to TakeTwo as soon as we have a solution for one call with multiple tests in test class
public class Given_DocsJson_Exist_Fixture : AdminApiFixture
{
    public Given_DocsJson_Exist_Fixture() : base(nameof(Given_DocsJson_Exist_Fixture))
    {
    }

    public HttpResponseMessage DocsJsonResponse { get; set; } = null!;

    public Docs? DocsJson { get; set; }

    protected override Task Given()
        => Task.CompletedTask;

    protected override async Task When()
    {
        DocsJsonResponse = await UnauthenticatedClient.GetDocsJson();
        DocsJson = JsonConvert.DeserializeObject<Docs>(await DocsJsonResponse.Content.ReadAsStringAsync());
    }
}

public class Given_DocsJson_Exist : IClassFixture<Given_DocsJson_Exist_Fixture>
{
    private readonly Given_DocsJson_Exist_Fixture _theDocsFixture;

    public Given_DocsJson_Exist(Given_DocsJson_Exist_Fixture theDocsFixture)
        => _theDocsFixture = theDocsFixture;

    [Fact]
    public async Task Then_theRootPath_returns_an_ok_response()
        => (await _theDocsFixture.UnauthenticatedClient.GetRoot()).StatusCode.Should().Be(HttpStatusCode.OK);

    [Fact]
    public void Then_it_returns_an_ok_response()
        => _theDocsFixture.DocsJsonResponse.Should().HaveStatusCode(HttpStatusCode.OK);

    [Fact]
    public void Then_it_has_paths()
        => _theDocsFixture.DocsJson!.Paths!.Count.Should().BePositive();

    [Fact]
    public void Then_it_as_a_summary_for_each_path()
        => _theDocsFixture.DocsJson!.Paths!
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
