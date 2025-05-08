namespace AssociationRegistry.Test.Admin.Api.Documentation.When_Fetching_The_Docs;

using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using FluentAssertions;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using Xunit;

[Collection(nameof(AdminApiCollection))]
[Category(Categories.MoveToBasicIntegrationCollection)]
public class Then_The_Docs
{
    public Then_The_Docs(EventsInDbScenariosFixture fixture)
    {
        Response = fixture.AdminApiClient.GetDocs().GetAwaiter().GetResult();
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

