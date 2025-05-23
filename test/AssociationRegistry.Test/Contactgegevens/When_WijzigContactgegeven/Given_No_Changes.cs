namespace AssociationRegistry.Test.Contactgegevens.When_WijzigContactgegeven;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_No_Changes
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Does_Not_Emit_Events(VerenigingState givenState, Registratiedata.Contactgegeven duplicateContactgegeven)
    {
        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(givenState);

        vereniging.WijzigContactgegeven(duplicateContactgegeven.ContactgegevenId, duplicateContactgegeven.Waarde,
                                        duplicateContactgegeven.Beschrijving, duplicateContactgegeven.IsPrimair);

        vereniging.UncommittedEvents.Should().BeEmpty();
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            var contactgegeven = fixture.Create<Registratiedata.Contactgegeven>();
            var gewijzigdeLocatie = contactgegeven;

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                        {
                            Contactgegevens = new[] { contactgegeven },
                        }),
                    gewijzigdeLocatie,
                },
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                        {
                            Contactgegevens = new[] { contactgegeven },
                        }),
                    gewijzigdeLocatie,
                },
                new object[]
                {
                    new VerenigingState()
                       .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                       .Apply(new ContactgegevenWerdToegevoegd(contactgegeven.ContactgegevenId, contactgegeven.Contactgegeventype,
                                                               contactgegeven.Waarde, contactgegeven.Beschrijving,
                                                               contactgegeven.IsPrimair)),
                    gewijzigdeLocatie,
                },
            };
        }
    }
}
