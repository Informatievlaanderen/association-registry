namespace AssociationRegistry.Test.Contactgegevens.When_VerwijderContactgegeven;

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

        var contactgegevenWerdOvergenomenUitKbo = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>() with { ContactgegevenId = 1 };

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(contactgegevenWerdOvergenomenUitKbo));

        var wijzigLocatie = () => vereniging.VerwijderContactgegeven(contactgegevenWerdOvergenomenUitKbo.ContactgegevenId);

        wijzigLocatie.Should().Throw<ContactgegevenUitKboKanNietVerwijderdWorden>();
    }
}
