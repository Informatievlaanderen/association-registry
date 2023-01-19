namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_BasisGegevens;

using System.Net;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using AutoFixture;
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

public class With_An_Empty_Body_Fixture : AdminApiFixture2
{
    private readonly Fixture _fixture;
    public const string VCode = "V0001001";

    public With_An_Empty_Body_Fixture() : base(
        nameof(With_An_Empty_Body_Fixture))
    {
        _fixture = new Fixture();
    }


    protected override async Task Given()
    {
        await AddEvent(
            VCode,
            new VerenigingWerdGeregistreerd(
                VCode,
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                DateOnly.FromDateTime(_fixture.Create<DateTime>()),
                _fixture.Create<string>(),
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                DateOnly.FromDateTime(DateTime.Today)),
            new CommandMetadata(
                _fixture.Create<string>(),
                new DateTime(2022, 1, 1).ToUniversalTime().ToInstant()));
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.PatchVereniging(VCode, "");
    }

    public HttpResponseMessage Response { get; set; }
}

public class With_An_Empty_Body : IClassFixture<With_An_Empty_Body_Fixture>
{
    private readonly With_An_Empty_Body_Fixture _apiFixture;

    public With_An_Empty_Body(With_An_Empty_Body_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_a_bad_request_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
