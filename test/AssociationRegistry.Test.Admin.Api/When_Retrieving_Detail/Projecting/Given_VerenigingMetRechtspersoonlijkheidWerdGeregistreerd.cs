﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using JsonLdContext;
using Vereniging;
using Vereniging.Bronnen;
using Xunit;
using Xunit.Categories;
using Doelgroep = AssociationRegistry.Admin.Schema.Detail.Doelgroep;

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

        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd =
            fixture.Create<TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>>();

        verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data with
        {
            Rechtsvorm = rechtsvorm,
        };

        var doc = BeheerVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new BeheerVerenigingDetailDocument
            {
                JsonLdMetadata = new JsonLdMetadata()
                {
                    Id = JsonLdType.Vereniging.CreateWithIdValues(doc.VCode),
                    Type = JsonLdType.Vereniging.Type,
                },
                VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                Verenigingstype = new VerenigingsType
                {
                    Code = Verenigingstype.Parse(rechtsvorm).Code,
                    Naam = Verenigingstype.Parse(rechtsvorm).Naam,
                },
                Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
                Roepnaam = string.Empty,
                KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = string.Empty,
                Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
                Doelgroep = new Doelgroep
                {
                    Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
                },
                Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
                DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Tijdstip.ToBelgianDate(),
                Status = VerenigingStatus.Actief,
                Contactgegevens = Array.Empty<AssociationRegistry.Admin.Schema.Detail.Contactgegeven>(),
                Locaties = Array.Empty<AssociationRegistry.Admin.Schema.Detail.Locatie>(),
                Vertegenwoordigers = Array.Empty<AssociationRegistry.Admin.Schema.Detail.Vertegenwoordiger>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<AssociationRegistry.Admin.Schema.Detail.HoofdactiviteitVerenigingsloket>(),
                Sleutels = new Sleutel[]
                {
                    new()
                    {
                        JsonLdMetadata = new JsonLdMetadata()
                        {
                            Id = JsonLdType.Sleutel.CreateWithIdValues(doc.VCode, Sleutelbron.Kbo.Waarde),
                            Type = JsonLdType.Sleutel.Type,
                        },
                        Bron = Sleutelbron.Kbo.Waarde,
                        Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                        GestructureerdeIdentificator = new GestructureerdeIdentificator()
                        {
                            JsonLdMetadata = new JsonLdMetadata()
                            {
                                Id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(doc.VCode, Sleutelbron.Kbo.Waarde),
                                Type = JsonLdType.GestructureerdeSleutel.Type,
                            },
                            Nummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                        },
                    },
                },
                Relaties = Array.Empty<Relatie>(),
                Bron = Bron.KBO,
                Metadata = new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence,
                                        verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
            });
    }
}
