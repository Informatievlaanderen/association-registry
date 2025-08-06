namespace AssociationRegistry.Test.Contactgegevens.When_WijzigContactgegeven_FromKbo;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_Contactgegeven_From_Initiator
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();
        var contactgegevenWerdToegevoegd = fixture.Create<ContactgegevenWerdToegevoegd>();

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(contactgegevenWerdToegevoegd)
        );

        var wijzig = () => vereniging.WijzigContactgegeven(contactgegevenWerdToegevoegd.ContactgegevenId, fixture.Create<string>(),
                                                           fixture.Create<bool>());

        wijzig.Should().Throw<ActieIsNietToegestaanVoorContactgegevenBron>();
    }
}
