﻿namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.LocatieZonderAdresMatch.Projector;

using AssociationRegistry.Admin.Schema.Detail;
using Events;
using AssociationRegistry.Framework;
using Grar;
using Framework.Fixtures;
using AutoFixture;
using FluentAssertions;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_AdresKonNietOvergenomenWordenUitAdressenregister_WithBadRequest : IClassFixture<
    GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture>
{
    private readonly GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture _fixture;

    public Given_AdresKonNietOvergenomenWordenUitAdressenregister_WithBadRequest(
        GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_A_Document_Should_Be_Created()
    {
        var session = _fixture.DocumentStore.LightweightSession();
        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync();
        docs.Should().BeEmpty();
    }
}

public class GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture : MultiStreamTestFixture
{
    public GivenAdresKonNietOvergenomenWordenUitAdressenregisterWithBadRequestFixture()
    {
        var vCode = "V9900017";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = new[] { locatie } },
            Fixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with
            {
                VCode = vCode, LocatieId = locatie.LocatieId, Reden = GrarClient.BadRequestSuccessStatusCodeMessage,
            },
        });
    }
}