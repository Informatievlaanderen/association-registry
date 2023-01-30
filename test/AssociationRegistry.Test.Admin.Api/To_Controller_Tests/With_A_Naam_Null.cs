﻿namespace AssociationRegistry.Test.Admin.Api.To_Controller_Tests;

using System.Net;
using Events;
using AssociationRegistry.Framework;
using Fixtures;
using AutoFixture;
using FluentAssertions;
using NodaTime.Extensions;
using Xunit;

//TODO rework into controller test
public class With_A_Naam_Null_Fixture : AdminApiFixture
{
    private readonly Fixture _fixture;
    private const string VCode = "V0001001";
    private const string JsonBody = @"{ ""Initiator"": ""OVO000001"", ""Naam"": null}";

    public With_A_Naam_Null_Fixture() : base(
        nameof(With_A_Naam_Null_Fixture))
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
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>()),
            new CommandMetadata(
                _fixture.Create<string>(),
                new DateTime(2022, 1, 1).ToUniversalTime().ToInstant()));
    }

    protected override async Task When()
    {
        Response = await AdminApiClient.PatchVereniging(VCode, JsonBody);
    }

    public HttpResponseMessage Response { get; private set; } = null!;
}

public class With_A_Naam_Null : IClassFixture<With_A_Naam_Null_Fixture>
{
    private readonly With_A_Naam_Null_Fixture _apiFixture;

    public With_A_Naam_Null(With_A_Naam_Null_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_a_bad_request_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
