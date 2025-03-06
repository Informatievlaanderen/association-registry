namespace AssociationRegistry.Test.Vertegenwoordigers.When_VoegVertegenwoordigerToe;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using System.Collections.Generic;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primaire_Vertegenwoordiger
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_it_throws(VerenigingState givenState)
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();

        vereniging.Hydrate(givenState);

        var toeTeVoegenVertegenwoordiger = fixture.Create<Vertegenwoordiger>() with { IsPrimair = true };

        Assert.Throws<MeerderePrimaireVertegenwoordigers>(() => vereniging.VoegVertegenwoordigerToe(toeTeVoegenVertegenwoordiger));
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            var primaireVertegenwoordiger = fixture.Create<Registratiedata.Vertegenwoordiger>() with { IsPrimair = true };

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                    {
                        Vertegenwoordigers = new[] { primaireVertegenwoordiger },
                    }),
                },
                new object[]
                {
                    new VerenigingState().Apply(fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                    {
                        Vertegenwoordigers = new[] { primaireVertegenwoordiger },
                    }),
                },

            };
        }
    }
}
