namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Formatters = AssociationRegistry.Admin.Api.Infrastructure.Extensions.Formatters;
using WellknownFormats = AssociationRegistry.Admin.Api.Constants.WellknownFormats;

[UnitTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_vereniging()
    {
        var fixture = new Fixture().CustomizeAll();
        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();
        var projector = new BeheerVerenigingDetailProjection();

        var doc = projector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new BeheerVerenigingDetailDocument
            {
                VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                Type = new BeheerVerenigingDetailDocument.VerenigingsType
                {
                    Code = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Code,
                    Beschrijving = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Beschrijving,
                },
                Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
                KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = string.Empty,
                Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
                DatumLaatsteAanpassing = Formatters.ToBelgianDate(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip),
                Status = "Actief",
                Contactgegevens = Array.Empty<BeheerVerenigingDetailDocument.Contactgegeven>(),
                Locaties = Array.Empty<BeheerVerenigingDetailDocument.Locatie>(),
                Vertegenwoordigers = Array.Empty<BeheerVerenigingDetailDocument.Vertegenwoordiger>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket>(),
                Sleutels = new BeheerVerenigingDetailDocument.Sleutel[]
                {
                    new()
                    {
                        Bron = Bron.Kbo.Waarde,
                        Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                    },
                },
                Relaties = Array.Empty<BeheerVerenigingDetailDocument.Relatie>(),
                Metadata = new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence, verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
            });
        doc.DatumLaatsteAanpassing.Should().Be(Formatters.ToBelgianDate(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip));
        doc.Metadata.Should().BeEquivalentTo(new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence, verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version));
    }
}
