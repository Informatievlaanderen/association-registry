namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
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

[UnitTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    [Theory]
    [InlineData("VZW")]
    [InlineData("IVZW")]
    [InlineData("PS")]
    [InlineData("SVON")]
    public void Then_it_creates_a_new_vereniging(string rechtsvorm)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();
        verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data with
        {
            Rechtsvorm = rechtsvorm,
        };

        var doc = BeheerVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new BeheerVerenigingDetailDocument
            {
                VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                Type = new BeheerVerenigingDetailDocument.VerenigingsType
                {
                    Code = Verenigingstype.Parse(rechtsvorm).Code,
                    Beschrijving = Verenigingstype.Parse(rechtsvorm).Beschrijving,
                },
                Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
                KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = string.Empty,
                Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                Doelgroep = new AssociationRegistry.Admin.Schema.Detail.Doelgroep
                {
                    Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
                },
                Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
                DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip.ToBelgianDate(),
                Status = "Actief",
                Contactgegevens = Array.Empty<BeheerVerenigingDetailDocument.Contactgegeven>(),
                Locaties = Array.Empty<BeheerVerenigingDetailDocument.Locatie>(),
                Vertegenwoordigers = Array.Empty<BeheerVerenigingDetailDocument.Vertegenwoordiger>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket>(),
                Sleutels = new BeheerVerenigingDetailDocument.Sleutel[]
                {
                    new()
                    {
                        Bron = Verenigingsbron.Kbo.Waarde,
                        Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                    },
                },
                Relaties = Array.Empty<BeheerVerenigingDetailDocument.Relatie>(),
                Bron = Bronnen.KBO,
                Metadata = new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence, verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
            });
        doc.DatumLaatsteAanpassing.Should().Be(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence, verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version));
    }
}
