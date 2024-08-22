﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_LocatieZonderAdresMatch.Projector;

using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework.Fixtures;
using Grar;
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
