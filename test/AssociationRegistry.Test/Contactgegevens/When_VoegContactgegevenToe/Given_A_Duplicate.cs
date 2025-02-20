namespace AssociationRegistry.Test.Contactgegevens.When_VoegContactgegevenToe;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate
{
    [Fact]
    public void With_FeitelijkeVereniging_Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var contactgegeven = fixture.Create<Registratiedata.Contactgegeven>();

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                               {
                                   Contactgegevens = new[] { contactgegeven },
                               }));

        Assert.Throws<ContactgegevenIsDuplicaat>(() => vereniging.VoegContactgegevenToe(
                                                     Contactgegeven.Create(
                                                         contactgegeven.Contactgegeventype,
                                                         contactgegeven.Waarde,
                                                         contactgegeven.Beschrijving,
                                                         contactgegeven.IsPrimair)));
    }

    [Fact]
    public void With_VerenigingZonderEigenRechtspersoonlijkheid_Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var contactgegeven = fixture.Create<Registratiedata.Contactgegeven>();

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                               {
                                   Contactgegevens = new[] { contactgegeven },
                               }));

        Assert.Throws<ContactgegevenIsDuplicaat>(() => vereniging.VoegContactgegevenToe(
                                                     Contactgegeven.Create(
                                                         contactgegeven.Contactgegeventype,
                                                         contactgegeven.Waarde,
                                                         contactgegeven.Beschrijving,
                                                         contactgegeven.IsPrimair)));
    }
}
