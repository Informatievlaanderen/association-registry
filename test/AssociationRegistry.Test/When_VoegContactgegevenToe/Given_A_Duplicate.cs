namespace AssociationRegistry.Test.When_VoegContactgegevenToe;

using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate
{
    [Fact]
    public void Then_it_throws()
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
}
