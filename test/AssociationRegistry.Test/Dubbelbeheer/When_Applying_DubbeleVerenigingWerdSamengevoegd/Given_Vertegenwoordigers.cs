namespace AssociationRegistry.Test.Dubbelbeheer.When_Applying_DubbeleVerenigingWerdSamengevoegd;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AutoFixture;
using FluentAssertions;
using Xunit;
using static AssociationRegistry.Events.SamenvoegingDubbeleVereniging;
using static AssociationRegistry.Test.Common.DubbelMerge.TestVertegenwoordigers;

/// <summary>De overgenomen vertegenwoordigers komen bij de bestaande, met hun nieuwe id en eventueel aangepast primair.</summary>
#pragma warning disable CS0618
public class Given_Vertegenwoordigers
{
    private readonly VerenigingState _state;

    public Given_Vertegenwoordigers()
    {
        var fixture = new Fixture().CustomizeDomain();

        var registratie = fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithVertegenwoordigers([Frodo]);

        var samengevoegd = EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            VCode.Create(registratie.VCode),
            fixture.Create<VCode>(),
            OvergenomenGegevensPerGroep<
                Registratiedata.Vertegenwoordiger,
                SamenvoegingDubbeleVereniging.Bankrekeningnummer
            >.WithOnly(OvergenomenBasisgegevens.Geen) with
            {
                Vertegenwoordigers =
                [
                    ToegevoegdMetAanpassing(
                        "aangepast",
                        Aragorn with
                        {
                            IsPrimair = true,
                        },
                        Aragorn with
                        {
                            VertegenwoordigerId = 2,
                            IsPrimair = false,
                        }
                    ),
                ],
            },
            NietOvergenomenGegevensPerGroep.WithOnly(NietOvergenomenBasisgegevens.Geen)
        );

        _state = new VerenigingState().Apply(registratie).Apply(samengevoegd);
    }

    [Fact]
    public void Then_The_Existing_Vertegenwoordiger_Is_Kept() =>
        EventFactory
            .Vertegenwoordiger(
                _state.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == Frodo.VertegenwoordigerId)
            )
            .Should()
            .BeEquivalentTo(Frodo);

    [Fact]
    public void Then_The_Nieuwe_Waarde_Is_Added() =>
        EventFactory
            .Vertegenwoordiger(_state.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == 2))
            .Should()
            .BeEquivalentTo(Aragorn with { VertegenwoordigerId = 2, IsPrimair = false });

    [Fact]
    public void Then_The_Next_Vertegenwoordiger_Gets_A_Free_Id() => _state.Vertegenwoordigers.NextId.Should().Be(3);
}
#pragma warning restore CS0618
