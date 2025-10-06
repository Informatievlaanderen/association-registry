namespace AssociationRegistry.Test.Aggregates.VerenigingMetRechtspersoonlijkheidTests.When_NeemGegevensOverUitKboSync.With_No_Vertegenwoordigers;

using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Factories;
using FluentAssertions;
using Xunit;

public class Given_An_ActiveMagdaVereniging_With_Same_Vertegenwoordigers
{
    [Fact]
    public void Then_It_Saves_The_Vertegenwoordigers()
    {
        var fixture = new Fixture().CustomizeDomain();
        var verenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        var verenigingVolgensMagda = fixture.Create<VerenigingVolgensKbo>();

        var sut = new VerenigingMetRechtspersoonlijkheid();
        sut.Hydrate(verenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario.GetVerenigingState());

        sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensMagda));

        sut.UncommittedEvents.OfType<VertegenwoordigerWerdToegevoegdVanuitKBO>().Should()
           .BeEquivalentTo(verenigingVolgensMagda.Vertegenwoordigers.Select((x, i) => EventFactory.VertegenwoordigerWerdToegevoegdVanuitKbo(Vertegenwoordiger.CreateFromKbo(x) with
            {
                VertegenwoordigerId = ++i,
            })));
    }
}
