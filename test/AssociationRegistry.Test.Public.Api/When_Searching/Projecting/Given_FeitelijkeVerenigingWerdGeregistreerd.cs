namespace AssociationRegistry.Test.Public.Api.When_Searching.Projecting;

using Events;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using AssociationRegistry.Public.Schema.Search;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_vereniging()
    {
        var fixture = new Fixture().CustomizeAll();
        var feitelijkeVerenigingWerdGeregistreerd = new TestEvent<FeitelijkeVerenigingWerdGeregistreerd>(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>());
        var projector = new ElasticEventProjection(new VerenigingBrolFeeder());

        var doc = projector.Create(feitelijkeVerenigingWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new VerenigingDocument
            {
                VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                Type = new VerenigingDocument.VerenigingsType
                {
                    Code = VerenigingsType.FeitelijkeVereniging.Code,
                    Beschrijving = VerenigingsType.FeitelijkeVereniging.Beschrijving,
                },
                Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
                KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
                Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties.Select(
                    loc => new VerenigingDocument.Locatie
                    {
                        Hoofdlocatie = loc.Hoofdlocatie,
                        Naam = loc.Naam,
                        Locatietype = loc.Locatietype,
                        Postcode = loc.Postcode,
                        Gemeente = loc.Gemeente,
                        Adres = loc.ToAdresString(),
                    }).ToArray(),
                HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                    arg => new VerenigingDocument.HoofdactiviteitVerenigingsloket
                    {
                        Code = arg.Code,
                        Naam = arg.Beschrijving,
                    }).ToArray(),
                Sleutels = Array.Empty<VerenigingDocument.Sleutel>(),
            });
    }
}
