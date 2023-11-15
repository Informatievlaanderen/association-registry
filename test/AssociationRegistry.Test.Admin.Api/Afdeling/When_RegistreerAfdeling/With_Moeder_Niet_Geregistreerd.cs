namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling.RequestModels;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerAfdeling_With_Moeder_Niet_Geregistreerd
{
    public readonly RegistreerAfdelingRequest Request;
    public readonly HttpResponseMessage Response;

    public When_RegistreerAfdeling_With_Moeder_Niet_Geregistreerd(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        Request = new RegistreerAfdelingRequest
        {
            Naam = autoFixture.Create<string>(),
            KboNummerMoedervereniging = "0407622110",
        };

        Response ??= fixture.DefaultClient.RegistreerAfdeling(GetJsonBody(Request)).GetAwaiter().GetResult();
    }

    private string GetJsonBody(RegistreerAfdelingRequest request)
        => GetType()
          .GetAssociatedResourceJson("files.request.with_minimum_fields")
          .Replace("{{vereniging.naam}}", request.Naam)
          .Replace("{{vereniging.kboNummerMoedervereniging}}", request.KboNummerMoedervereniging);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Moeder_Niet_Geregistreerd : IClassFixture<When_RegistreerAfdeling_With_Moeder_Niet_Geregistreerd>
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly RegistreerAfdelingRequest _request;
    private readonly HttpResponseMessage _response;

    public With_Moeder_Niet_Geregistreerd(When_RegistreerAfdeling_With_Moeder_Niet_Geregistreerd setup, EventsInDbScenariosFixture fixture)
    {
        _request = setup.Request;
        _response = setup.Response;
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        using var session = _fixture.DocumentStore
                                    .LightweightSession();

        var savedMoederEvent = session.Events
                                      .QueryRawEventDataOnly<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()
                                      .Single(e => e.KboNummer == _request.KboNummerMoedervereniging);

        var savedEvent = session.Events
                                .QueryRawEventDataOnly<AfdelingWerdGeregistreerd>()
                                .Single(e => e.Naam == _request.Naam);

        savedEvent.Moedervereniging.KboNummer.Should().Be(_request.KboNummerMoedervereniging);
        savedEvent.Moedervereniging.VCode.Should().Be(savedMoederEvent.VCode);
        savedEvent.Moedervereniging.Naam.Should().Be(savedMoederEvent.Naam);
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public void Then_it_returns_a_location_header()
    {
        _response.Headers.Should().ContainKey(HeaderNames.Location);

        _response.Headers.Location!.OriginalString.Should()
                 .StartWith($"{_fixture.ServiceProvider.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");
    }

    [Fact]
    public void Then_it_returns_a_sequence_header()
    {
        _response.Headers.Should().ContainKey(WellknownHeaderNames.Sequence);
        var sequenceValues = _response.Headers.GetValues(WellknownHeaderNames.Sequence).ToList();
        sequenceValues.Should().HaveCount(expected: 1);
        var sequence = Convert.ToInt64(sequenceValues.Single());
        sequence.Should().BeGreaterThan(expected: 0);
    }
}
