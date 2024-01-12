﻿namespace AssociationRegistry.Test.When_WijzigLocatie;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Throws_DuplicateLocatie(VerenigingState givenState, Registratiedata.Locatie gewijzigdeLocatie)
    {
        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(givenState);

        var adresId = AdresId.Hydrate(Adresbron.Parse(gewijzigdeLocatie.AdresId!.Broncode), gewijzigdeLocatie.AdresId.Bronwaarde);
        var adres = HydrateAdres(gewijzigdeLocatie.Adres!);

        var wijzigLocatie = () => vereniging.WijzigLocatie(gewijzigdeLocatie.LocatieId, gewijzigdeLocatie.Naam,
                                                           gewijzigdeLocatie.Locatietype, gewijzigdeLocatie.IsPrimair, adresId, adres);

        wijzigLocatie.Should().Throw<LocatieIsNietUniek>();
    }

    private static Adres HydrateAdres(Registratiedata.Adres gewijzigdeLocatieAdres)
    {
        gewijzigdeLocatieAdres.Deconstruct(
            out var straatnaam,
            out var huisnummer,
            out var busnummer,
            out var postcode,
            out var gemeente,
            out var land);

        return Adres.Hydrate(straatnaam, huisnummer, busnummer, postcode, gemeente, land);
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            var locaties = fixture.CreateMany<Registratiedata.Locatie>().ToArray();
            var locatie1 = locaties[0];
            var locatie2 = locaties[1];

            var gewijzigdeLocatie = locatie2 with
            {
                Naam = locatie1.Naam,
                Locatietype = locatie1.Locatietype,
                IsPrimair = locatie1.IsPrimair,
                AdresId = locatie1.AdresId,
                Adres = locatie1.Adres,
            };

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                        {
                            Locaties = locaties,
                        }),
                    gewijzigdeLocatie,
                },
            };
        }
    }
}
