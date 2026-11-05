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
using static AssociationRegistry.Test.Common.DubbelMerge.TestLocaties;

/// <summary>De overgenomen locaties komen bij de bestaande, met hun nieuwe id en eventueel aangepast primair.</summary>
#pragma warning disable CS0618
public class Given_Locaties
{
    private readonly VerenigingState _state;

    public Given_Locaties()
    {
        var fixture = new Fixture().CustomizeDomain();

        var registratie = fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithLocaties([LokaalRivendell]);

        var samengevoegd = EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            VCode.Create(registratie.VCode),
            fixture.Create<VCode>(),
            OvergenomenGegevensPerGroep<
                Registratiedata.Vertegenwoordiger,
                SamenvoegingDubbeleVereniging.Bankrekeningnummer
            >.WithOnly(OvergenomenBasisgegevens.Geen) with
            {
                Locaties =
                [
                    ToegevoegdMetAanpassing(
                        "aangepast",
                        LokaalFellowship,
                        LokaalFellowship with
                        {
                            LocatieId = 2,
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
    public void Then_The_Existing_Locatie_Is_Kept() =>
        _state.Locaties.Should().ContainSingle(x => x.LocatieId == LokaalRivendell.LocatieId && x.IsPrimair);

    [Fact]
    public void Then_The_Nieuwe_Waarde_Is_Added() =>
        EventFactory
            .Locatie(_state.Locaties.Single(x => x.LocatieId == 2))
            .Should()
            .BeEquivalentTo(LokaalFellowship with { LocatieId = 2, IsPrimair = false });

    [Fact]
    public void Then_The_Next_Locatie_Gets_A_Free_Id() => _state.Locaties.NextId.Should().Be(3);
}
#pragma warning restore CS0618
