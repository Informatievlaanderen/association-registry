﻿namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.LocatieZonderAdresMatch.Projector;

using AssociationRegistry.Admin.Schema.Detail;
using Events;
using AssociationRegistry.Framework;
using Framework.Fixtures;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Marten;
using Xunit;

[Collection(nameof(MultiStreamTestCollection))]
public class Given_LocatieWerdGewijzigd_AndDocumentAlreadyExists : IClassFixture<GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture>
{
    private readonly GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture _fixture;

    public Given_LocatieWerdGewijzigd_AndDocumentAlreadyExists(GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Then_Only_One_Document_Should_Be_Created()
    {
        var session = _fixture.DocumentStore.LightweightSession();
        var docs = await session.Query<LocatieZonderAdresMatchDocument>().ToListAsync();
        docs.Should().NotBeEmpty();
        docs.Should().HaveCount(1);
    }
}

public class GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture : MultiStreamTestFixture
{
    public GivenLocatieWerdGewijzigdAndDocumentAlreadyExistsFixture()
    {
        var vCode = "V9900002";
        var locatie = Fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1, Locatietype = Locatietype.Activiteiten.Waarde };

        Stream(vCode, new IEvent[]
        {
            Fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode, Locaties = Array.Empty<Registratiedata.Locatie>() },
            Fixture.Create<LocatieWerdToegevoegd>() with { Locatie = locatie },
            Fixture.Create<LocatieWerdGewijzigd>() with { Locatie = locatie },
        });
    }
}
