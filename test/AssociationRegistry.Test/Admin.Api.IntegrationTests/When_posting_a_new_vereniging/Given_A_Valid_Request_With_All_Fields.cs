﻿namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.When_posting_a_new_vereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Verenigingen;
using AutoFixture;
using Fixtures;
using FluentAssertions;
using Framework.Helpers;
using Marten;
using Vereniging;
using Xunit;

public class Given_A_Valid_Request_With_All_Fields_Fixture : AdminApiFixture
{
    public Given_A_Valid_Request_With_All_Fields_Fixture() : base(
        nameof(Given_A_Valid_Request_With_All_Fields_Fixture))
    {
        var fixture = new Fixture();
        var request = new RegistreerVerenigingRequest
        {
            Naam = fixture.Create<string>(),
            KorteNaam = fixture.Create<string>(),
            KorteBeschrijving = fixture.Create<string>(),
            StartDatum = DateOnly.FromDateTime(DateTime.Today),
            KboNummer = "0123456749",
            Initiator = "OVO000001",
        };
        Content = GetJsonBody(request).AsJsonContent();
        Request = request;
    }

    public RegistreerVerenigingRequest Request { get; }

    public StringContent Content { get; }

    private string GetJsonBody(RegistreerVerenigingRequest request)
        => GetType()
            .GetAssociatedResourceJson($"files.request.with_all_fields")
            .Replace("{{vereniging.naam}}", request.Naam)
            .Replace("{{vereniging.korteNaam}}", request.KorteNaam)
            .Replace("{{vereniging.korteBeschrijving}}", request.KorteBeschrijving)
            .Replace("{{vereniging.startdatum}}", request.StartDatum!.Value.ToString(WellknownFormats.DateOnly))
            .Replace("{{vereniging.kboNummer}}", request.KboNummer)
            .Replace("{{vereniging.initiator}}", request.Initiator);
}

public class Given_A_Valid_Request_With_All_Fields : IClassFixture<Given_A_Valid_Request_With_All_Fields_Fixture>
{
    private readonly Given_A_Valid_Request_With_All_Fields_Fixture _apiFixture;

    public Given_A_Valid_Request_With_All_Fields(Given_A_Valid_Request_With_All_Fields_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task Then_it_returns_an_accepted_response()
    {
        var response = await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", _apiFixture.Content);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await _apiFixture.HttpClient.PostAsync("/v1/verenigingen", _apiFixture.Content);

        var savedEvent = await _apiFixture.DocumentStore
            .LightweightSession().Events
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .SingleAsync(e => e.Naam == _apiFixture.Request.Naam);
        savedEvent.KorteNaam.Should().Be(_apiFixture.Request.KorteNaam);
        savedEvent.KorteBeschrijving.Should().Be(_apiFixture.Request.KorteBeschrijving);
        savedEvent.Startdatum.Should().Be(_apiFixture.Request.StartDatum);
        savedEvent.KboNummer.Should().Be(_apiFixture.Request.KboNummer);
        savedEvent.Status.Should().Be("Actief");
    }
}
