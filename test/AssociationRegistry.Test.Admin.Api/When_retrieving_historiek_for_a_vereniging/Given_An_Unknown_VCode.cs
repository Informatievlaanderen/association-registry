namespace AssociationRegistry.Test.Admin.Api.When_retrieving_historiek_for_a_vereniging;

using System.Net;
using Azure;
using Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

public class Given_An_Unknown_Vereniging_Fixture : AdminApiFixture2
{
    public const string VCode = "v9999999";

    public Given_An_Unknown_Vereniging_Fixture() : base(nameof(Given_An_Unknown_Vereniging_Fixture))
    {
    }

    public HttpResponseMessage Response { get; set; } = null!;

    protected override async Task Given()
    {
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.GetHistoriek(VCode);
    }
}

public class Given_An_Unknown_Vereniging : IClassFixture<Given_An_Unknown_Vereniging_Fixture>
{
    private const string VCode = Given_An_Unknown_Vereniging_Fixture.VCode;
    private readonly Given_An_Unknown_Vereniging_Fixture _adminApiFixture;

    public Given_An_Unknown_Vereniging(Given_An_Unknown_Vereniging_Fixture fixture)
    {
        _adminApiFixture = fixture;
    }

    [Fact]
    public void Then_we_get_a_404()
        => _adminApiFixture.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
