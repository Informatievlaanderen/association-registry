namespace AssociationRegistry.Test.When_SyncCompleted;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Framework.Customizations;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Always
{
    [Fact]
    public void Then_It_Should_Add_A_KboSyncSuccessfulEvent()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingMetRechtspersoonlijkheid();

        vereniging.Hydrate(new VerenigingState().Apply(
                               fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>()));

        vereniging.SyncCompleted();

        vereniging.UncommittedEvents.Should().ContainSingle(e => e.GetType() == typeof(SynchronisatieMetKboWasSuccesvol));
    }
}
