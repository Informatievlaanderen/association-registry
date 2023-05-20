namespace AssociationRegistry.Test.Public.Api.When_Searching.Projecting;

using Events;
using AssociationRegistry.Public.ProjectionHost.Projections.Search;
using AssociationRegistry.Public.Schema.Search;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_vereniging()
    {
        var fixture = new Fixture().CustomizeAll();
        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = new TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>());
        var projector = new ElasticEventProjection(new VerenigingBrolFeeder());

        var doc = projector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new VerenigingDocument
            {
                VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                Type = new VerenigingDocument.VerenigingsType
                {
                    Code = VerenigingsType.VerenigingMetRechtspersoonlijkheid.Code,
                    Beschrijving = VerenigingsType.VerenigingMetRechtspersoonlijkheid.Beschrijving,
                },
                Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
                KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
                Locaties = Array.Empty<VerenigingDocument.Locatie>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<VerenigingDocument.HoofdactiviteitVerenigingsloket>(),
                Sleutels = new VerenigingDocument.Sleutel[]
                {
                    new()
                    {
                        Bron = Bron.Kbo.Waarde,
                        Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                    },
                },
            });
    }
}
