namespace AssociationRegistry.Test.Contactgegevens.When_WijzigContactgegeven;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_Contactgegeven_From_Kbo
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>();

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(contactgegevenWerdOvergenomenUitKbo)
        );

        var wijzigContactgegeven = () => vereniging.WijzigContactgegeven(contactgegevenWerdOvergenomenUitKbo.ContactgegevenId,
                                                                         fixture.Create<string>(), fixture.Create<string>(),
                                                                         fixture.Create<bool>());

        wijzigContactgegeven.Should().Throw<ContactgegevenUitKboKanNietGewijzigdWorden>();
    }
}
