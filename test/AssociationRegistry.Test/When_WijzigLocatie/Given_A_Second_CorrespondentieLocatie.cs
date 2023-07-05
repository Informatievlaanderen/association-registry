namespace AssociationRegistry.Test.When_WijzigLocatie;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_CorrespondentieLocatie
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Throws_MultipleCorrespondentieLocaties(VerenigingState givenState, Registratiedata.Locatie gewijzigdeLocatie)
    {
        var vereniging = new Vereniging();
        vereniging.Hydrate(givenState);

        var wijzigLocatie = () => vereniging.WijzigLocatie(
            gewijzigdeLocatie.LocatieId,
            null,
            gewijzigdeLocatie.Locatietype,
            null,
            null,
            null);

        wijzigLocatie.Should().Throw<MultipleCorrespondentieLocaties>();
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            var correspondentieLocatie = fixture.Create<Registratiedata.Locatie>() with { Locatietype = Locatietype.Correspondentie };
            var teWijzigenLocatie = fixture.Create<Registratiedata.Locatie>();
            var gewijzigdeLocatie = teWijzigenLocatie with
            {
                Locatietype = Locatietype.Correspondentie,
            };

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<AfdelingWerdGeregistreerd>() with
                        {
                            Locaties = new[] { correspondentieLocatie, teWijzigenLocatie },
                        }),
                    gewijzigdeLocatie,
                },
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                        {
                            Locaties = new[] { correspondentieLocatie, teWijzigenLocatie },
                        }),
                    gewijzigdeLocatie,
                },
            };
        }
    }
}
