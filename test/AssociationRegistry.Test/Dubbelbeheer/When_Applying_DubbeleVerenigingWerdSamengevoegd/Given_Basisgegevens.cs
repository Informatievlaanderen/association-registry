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

/// <summary>
///     De state moet de overgenomen basisgegevens toepassen; de niet-overgenomen gegevens laten de state ongemoeid.
/// </summary>
#pragma warning disable CS0618
public class Given_Basisgegevens
{
    private readonly VerenigingState _state;

    public Given_Basisgegevens()
    {
        var fixture = new Fixture().CustomizeDomain();

        var authentiekeVereniging = fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBasisgegevens(TestBasisgegevens.Empty with { KorteNaam = TestBasisgegevens.Rivendell.KorteNaam });

        var samengevoegd = EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            VCode.Create(authentiekeVereniging.VCode),
            fixture.Create<VCode>(),
            new OvergenomenBasisgegevens(
                KorteNaam: null,
                KorteBeschrijving: Verrijkt(Velden.KorteBeschrijving, TestBasisgegevens.Fellowship.KorteBeschrijving),
                Startdatum: Verrijkt(Velden.Startdatum, TestBasisgegevens.Fellowship.Startdatum!.Value),
                Doelgroep: Verrijkt(Velden.Doelgroep, TestBasisgegevens.Fellowship.Doelgroep)
            ),
            NietOvergenomenBasisgegevens.Geen with
            {
                KorteNaam = NietOvergenomen(Velden.KorteNaam, TestBasisgegevens.Fellowship.KorteNaam),
            }
        );

        _state = new VerenigingState().Apply(authentiekeVereniging).Apply(samengevoegd);
    }

    [Fact]
    public void Then_The_KorteNaam_Is_Kept() => _state.KorteNaam.Should().Be(TestBasisgegevens.Rivendell.KorteNaam);

    [Fact]
    public void Then_The_KorteBeschrijving_Is_Overgenomen() =>
        _state.KorteBeschrijving.Should().Be(TestBasisgegevens.Fellowship.KorteBeschrijving);

    [Fact]
    public void Then_The_Startdatum_Is_Overgenomen() =>
        _state.Startdatum.Should().Be(Datum.Hydrate(TestBasisgegevens.Fellowship.Startdatum));

    [Fact]
    public void Then_The_Doelgroep_Is_Overgenomen() =>
        _state
            .Doelgroep.Should()
            .Be(
                Doelgroep.Hydrate(
                    TestBasisgegevens.Fellowship.Doelgroep.Minimumleeftijd,
                    TestBasisgegevens.Fellowship.Doelgroep.Maximumleeftijd
                )
            );
}
#pragma warning restore CS0618
