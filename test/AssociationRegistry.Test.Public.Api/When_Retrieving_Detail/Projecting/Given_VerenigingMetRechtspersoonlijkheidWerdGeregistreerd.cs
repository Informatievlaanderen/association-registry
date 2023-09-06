﻿namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using Admin.Schema.Constants;
using Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;
using Doelgroep = Vereniging.Doelgroep;

[UnitTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_vereniging()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = new TestEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>());

        var doc = PubliekVerenigingDetailProjector.Create(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new PubliekVerenigingDetailDocument
            {
                VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
                Type = new PubliekVerenigingDetailDocument.VerenigingsType
                {
                    Code = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Code,
                    Beschrijving = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Beschrijving,
                },
                Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
                Roepnaam = string.Empty,
                KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = string.Empty,
                Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum,
                Doelgroep = new AssociationRegistry.Public.Schema.Detail.Doelgroep
                {
                    Minimumleeftijd = Doelgroep.StandaardMinimumleeftijd,
                    Maximumleeftijd = Doelgroep.StandaardMaximumleeftijd,
                },
                Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
                DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
                Status = VerenigingStatus.Actief,
                IsUitgeschrevenUitPubliekeDatastroom = false,
                Contactgegevens = Array.Empty<PubliekVerenigingDetailDocument.Contactgegeven>(),
                Locaties = Array.Empty<PubliekVerenigingDetailDocument.Locatie>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket>(),
                Sleutels = new PubliekVerenigingDetailDocument.Sleutel[]
                {
                    new()
                    {
                        Bron = Sleutelbron.Kbo.Waarde,
                        Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                    },
                },
            });
    }
}
