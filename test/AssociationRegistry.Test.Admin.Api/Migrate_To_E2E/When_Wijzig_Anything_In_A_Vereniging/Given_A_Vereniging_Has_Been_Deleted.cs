﻿namespace AssociationRegistry.Test.Admin.Api.Migrate_To_E2E.When_Wijzig_Anything_In_A_Vereniging;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Verwijder.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.EventStore;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using Xunit;

[Collection(nameof(AdminApiCollection))]
[Category(Categories.MoveToBasicE2E)]
public class Given_A_Vereniging_Has_Been_Deleted
{
    private const string TEST_INITIATOR = EventStore.DigitaalVlaanderenOvoNumber;
    private readonly AdminApiClients _adminApiClients;
    private readonly Fixture _fixture;
    private readonly V061_VerenigingWerdGeregistreerd_And_Verwijderd_And_FollowedByUpdates _scenario;

    public Given_A_Vereniging_Has_Been_Deleted(EventsInDbScenariosFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _adminApiClients = fixture.AdminApiClients;
        _scenario = fixture.V061VerenigingWerdGeregistreerdAndVerwijderdAndFollowedByUpdates;
    }

    [Fact]
    // Naam wijzigen van vereniging
    public async ValueTask It_Should_Return_NotFound_Or_BadRequest_OnPatch_When_WijzigVereniging()
    {
        var response = await _adminApiClients.Authenticated.PatchVereniging(
            VCode.Create(_scenario.VCode),
            JsonConvert.SerializeObject(new WijzigBasisgegevensRequest
            {
                Naam = "Nieuwe naam voor vereniging",
            }),
            initiator: TEST_INITIATOR);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    // Nieuw contactgegeven toevoegen
    public async ValueTask It_Should_Return_NotFound_Or_BadRequest_OnPost_When_Adding_Contactgegeven()
    {
        var response = await _adminApiClients.Authenticated.PostContactgegevens(
            VCode.Create(_scenario.VCode),
            JsonConvert.SerializeObject(new VoegContactgegevenToeRequest
            {
                Contactgegeven = _fixture.Create<ToeTeVoegenContactgegeven>(),
            }),
            initiator: TEST_INITIATOR);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    // Vertegenwoordiger verwijderen
    public async ValueTask It_Should_Return_NotFound_Or_BadRequest_OnDelete_When_Removing_Vertegenwoordiger()
    {
        var response = await _adminApiClients.Authenticated.DeleteVertegenwoordiger(
            VCode.Create(_scenario.VCode),
            _scenario.VertegenwoordigerId,
            jsonBody: "",
            initiator: TEST_INITIATOR);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    // Nieuwe locatie toevoegen
    public async ValueTask It_Should_Return_NotFound_Or_BadRequest_OnPost_When_Adding_Locatie()
    {
        var response = await _adminApiClients.Authenticated.PostLocatie(
            VCode.Create(_scenario.VCode),
            JsonConvert.SerializeObject(new VoegLocatieToeRequest
            {
                Locatie = _fixture.Create<ToeTeVoegenLocatie>(),
            }),
            initiator: TEST_INITIATOR);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }

    [Fact]
    // Nieuwe locatie toevoegen
    public async ValueTask It_Should_Return_NotFound_Or_BadRequest_OnDelete()
    {
        var response = await _adminApiClients.SuperAdmin.DeleteVereniging(
            VCode.Create(_scenario.VCode),
            JsonConvert.SerializeObject(_fixture.Create<VerwijderVerenigingRequest>()),
            initiator: TEST_INITIATOR);

        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest);
    }
}
