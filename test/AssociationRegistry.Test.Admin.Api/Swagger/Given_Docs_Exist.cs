namespace AssociationRegistry.Test.Admin.Api.Swagger;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_RetrievingDocs
{
    public readonly HttpResponseMessage Response;
    public readonly Docs? Docs;

    private When_RetrievingDocs(AdminApiFixture fixture)
    {
        Response = fixture.UnauthenticatedClient.GetDocsJson().GetAwaiter().GetResult();
        Docs = JsonConvert.DeserializeObject<Docs>(Response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }

    private static When_RetrievingDocs? called;

    public static When_RetrievingDocs Called(AdminApiFixture fixture)
        => called ??= new When_RetrievingDocs(fixture);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Docs_Exist
{
    private readonly EventsInDbScenariosFixture _fixture;

    private HttpResponseMessage Response
        => When_RetrievingDocs.Called(_fixture).Response;

    private Docs? Docs
        => When_RetrievingDocs.Called(_fixture).Docs;

    public Given_Docs_Exist(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_theRootPath_returns_an_ok_response()
        => (await _fixture.UnauthenticatedClient.GetRoot()).StatusCode.Should().Be(HttpStatusCode.OK);

    [Fact]
    public void Then_it_returns_an_ok_response()
        => Response.Should().HaveStatusCode(HttpStatusCode.OK);

    [Fact]
    public void Has_paths()
        => Docs!.Paths!.Count.Should().BePositive();

    [Fact]
    public void Then_it_as_a_summary_for_each_path()
        => Docs!.Paths!
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

    public Dictionary<string, Dictionary<string, DocsPath>>? Paths { get; set; }
}
