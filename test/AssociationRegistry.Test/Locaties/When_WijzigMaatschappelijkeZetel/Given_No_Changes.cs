namespace AssociationRegistry.Test.Locaties.When_WijzigMaatschappelijkeZetel;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_No_Changes
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Emits_LocatieWerdGewijzigdEvent(
        VerenigingState givenState,
        int maatschappelijkeZetelId,
        string nieuweNaam,
        bool nieuwIsPrimair)
    {
        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        vereniging.Hydrate(givenState);

        vereniging.WijzigMaatschappelijkeZetel(maatschappelijkeZetelId, nieuweNaam, nieuwIsPrimair);
        vereniging.UncommittedEvents.Should().BeEmpty();
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

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(
                                              fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                                         .Apply(new MaatschappelijkeZetelWerdOvergenomenUitKbo(maatschappelijkeZetel)),
                    maatschappelijkeZetel.LocatieId,
                    maatschappelijkeZetel.Naam,
                    maatschappelijkeZetel.IsPrimair,
                },
            };
        }
    }
}
