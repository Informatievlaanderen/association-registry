namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_WithMinimalFields
{
    private static When_RegistreerVereniging_WithMinimalFields? called;
    public readonly RegistreerVerenigingRequest Request;
    public readonly HttpResponseMessage Response;

    private When_RegistreerVereniging_WithMinimalFields(AdminApiFixture fixture)
    {
        Request = new RegistreerVerenigingRequest
        {
            Naam = new Fixture().Create<string>(),
            Initiator = "OVO000001",
        };
        Response ??= fixture.DefaultClient.RegistreerVereniging(GetJsonBody(Request)).GetAwaiter().GetResult();
    }

    public static When_RegistreerVereniging_WithMinimalFields Called(AdminApiFixture fixture)
        => called ??= new When_RegistreerVereniging_WithMinimalFields(fixture);

    private string GetJsonBody(RegistreerVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson("files.request.with_minimal_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.initiator}}", request.Initiator);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Minimal_Fields
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Minimal_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerVerenigingRequest Request
        => When_RegistreerVereniging_WithMinimalFields.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_RegistreerVereniging_WithMinimalFields.Called(_fixture).Response;

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();

        session.Events.QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Where(e => e.Naam == Request.Naam)
            .Should().HaveCount(expected: 1);
    }

    [Fact]
    public void Then_it_returns_an_accepted_response_with_correct_headers()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        Response.Headers.Should().ContainKey(HeaderNames.Location);
        Response.Headers.Location!.OriginalString.Should()
            .StartWith($"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(expected: 1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(expected: 0);
    }
}
