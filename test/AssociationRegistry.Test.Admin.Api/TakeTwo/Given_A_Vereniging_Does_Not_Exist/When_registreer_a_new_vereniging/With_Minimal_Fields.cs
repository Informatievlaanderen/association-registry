namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Events;
using Framework;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_WithMinimalFields
{
    public readonly RegistreerVerenigingRequest Request;
    public HttpResponseMessage Response { get; }
    private When_RegistreerVereniging_WithMinimalFields(AdminApiFixture2 fixture)
    {
        Request = new RegistreerVerenigingRequest
        {
            Naam = new Fixture().Create<string>(),
            Initiator = "OVO000001",
        };
        Response ??= fixture.DefaultClient.RegistreerVereniging(GetJsonBody(Request)).GetAwaiter().GetResult();
    }

    private static When_RegistreerVereniging_WithMinimalFields? called;

    public static When_RegistreerVereniging_WithMinimalFields Called(AdminApiFixture2 fixture)
        => called ??= new When_RegistreerVereniging_WithMinimalFields(fixture);

    private string GetJsonBody(RegistreerVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_minimal_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.initiator}}", request.Initiator);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Minimal_Fields
{
    private readonly EventsInDbScenariosFixture _fixture;
    private RegistreerVerenigingRequest Request
        => When_RegistreerVereniging_WithMinimalFields.Called(_fixture).Request;
    private HttpResponseMessage Response
        => When_RegistreerVereniging_WithMinimalFields.Called(_fixture).Response;

    public With_Minimal_Fields(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();

        session.Events.QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .Where(e => e.Naam == Request.Naam)
            .Should().HaveCount(1);
    }

    [Fact]
    public void Then_it_returns_an_accepted_response_with_correct_headers()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        Response.Headers.Should().ContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
        Response.Headers.Location!.OriginalString.Should()
            .StartWith($"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        Response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = Response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(0);
    }
}
