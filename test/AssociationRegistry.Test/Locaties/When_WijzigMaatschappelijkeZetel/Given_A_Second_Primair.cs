namespace AssociationRegistry.Test.Locaties.When_WijzigMaatschappelijkeZetel;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_A_Second_Primair
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Throws_MultiplePrimaireLocaties(VerenigingState givenState, int maatschappelijkeZetelId)
    {
        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        vereniging.Hydrate(givenState);

        var wijzigLocatie = () => vereniging.WijzigMaatschappelijkeZetel(
            maatschappelijkeZetelId,
            naam: null,
            isPrimair: true);

        wijzigLocatie.Should().Throw<MeerderePrimaireLocatiesZijnNietToegestaan>();
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var maatschappelijkeZetel = fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 1,
            };

            var primaireLocatie = fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 2,
                IsPrimair = true,
            };

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(
                                              fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                                         .Apply(new MaatschappelijkeZetelWerdOvergenomenUitKbo(maatschappelijkeZetel))
                                         .Apply(new LocatieWerdToegevoegd(primaireLocatie)),
                    maatschappelijkeZetel.LocatieId,
                },
            };
        }
    }
}
