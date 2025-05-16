namespace AssociationRegistry.Test.Locaties.When_WijzigLocatie;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
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
            naam: null,
            locatietype: null,
            gewijzigdeLocatie.IsPrimair,
            adresId: null,
            adres: null);

        wijzigLocatie.Should().Throw<MeerderePrimaireLocatiesZijnNietToegestaan>();
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
                        fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                        {
                            Locaties = new[] { primaireLocatie, teWijzigenLocatie },
                        }),
                    gewijzigdeLocatie,
                },
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                        {
                            Locaties = new[] { primaireLocatie, teWijzigenLocatie },
                        }),
                    gewijzigdeLocatie,
                },
            };
        }
    }
}
