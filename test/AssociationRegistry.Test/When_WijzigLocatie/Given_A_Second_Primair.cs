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
public class Given_A_Second_Primair
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Throws_MultiplePrimaireLocaties(VerenigingState givenState, Registratiedata.Locatie gewijzigdeLocatie)
    {
        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(givenState);

        var wijzigLocatie = () => vereniging.WijzigLocatie(
            gewijzigdeLocatie.LocatieId,
            null,
            null,
            gewijzigdeLocatie.IsPrimair,
            null,
            null);

        wijzigLocatie.Should().Throw<MultiplePrimaireLocaties>();
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            var primaireLocatie = fixture.Create<Registratiedata.Locatie>() with { IsPrimair = true };
            var teWijzigenLocatie = fixture.Create<Registratiedata.Locatie>();
            var gewijzigdeLocatie = teWijzigenLocatie with
            {
                IsPrimair = true,
            };

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<AfdelingWerdGeregistreerd>() with
                        {
                            Locaties = new[] { primaireLocatie, teWijzigenLocatie },
                        }),
                    gewijzigdeLocatie,
                },
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                        {
                            Locaties = new[] { primaireLocatie, teWijzigenLocatie },
                        }),
                    gewijzigdeLocatie,
                },
            };
        }
    }
}
