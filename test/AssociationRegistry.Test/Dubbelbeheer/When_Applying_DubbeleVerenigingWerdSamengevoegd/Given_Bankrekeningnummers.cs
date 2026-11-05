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
using static AssociationRegistry.Test.Common.DubbelMerge.TestBankrekeningnummers;

/// <summary>Het overgenomen bankrekeningnummer komt bij de bestaande, met zijn bevestigingen.</summary>
#pragma warning disable CS0618
public class Given_Bankrekeningnummers
{
    private readonly VerenigingState _state;
    private readonly Registratiedata.Bankrekeningnummer _bestaand;

    public Given_Bankrekeningnummers()
    {
        var fixture = new Fixture().CustomizeDomain();

        _bestaand = new Registratiedata.Bankrekeningnummer(1, "BE62510007547061", "Lidgelden", ["Bilbo Baggins"]);

        var registratie = fixture
            .Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .WithBankrekeningnummers([_bestaand]);

        var samengevoegd = EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            VCode.Create(registratie.VCode),
            fixture.Create<VCode>(),
            OvergenomenGegevensPerGroep<Registratiedata.Vertegenwoordiger, Bankrekeningnummer>.WithOnly(
                OvergenomenBasisgegevens.Geen
            ) with
            {
                Bankrekeningnummers =
                [
                    new OvergenomenGegeven<Bankrekeningnummer>(
                        Acties.Toegevoegd,
                        "toegevoegd",
                        BankrekeningnummerSamwise,
                        BankrekeningnummerSamwise with
                        {
                            BankrekeningnummerId = 2,
                        }
                    ),
                ],
            },
            NietOvergenomenGegevensPerGroep.WithOnly(NietOvergenomenBasisgegevens.Geen)
        );

        _state = new VerenigingState().Apply(registratie).Apply(samengevoegd);
    }

    [Fact]
    public void Then_The_Existing_Bankrekeningnummer_Is_Kept() =>
        _state.Bankrekeningnummers.Should().ContainSingle(x => x.BankrekeningnummerId == 1 && x.Doel == _bestaand.Doel);

    [Fact]
    public void Then_The_Nieuwe_Waarde_Is_Added_With_BevestigdDoor()
    {
        var bankrekeningnummer = _state.Bankrekeningnummers.Single(x => x.BankrekeningnummerId == 2);

        bankrekeningnummer.Iban.Value.Should().Be(BankrekeningnummerSamwise.Iban);
        bankrekeningnummer.Doel.Should().Be(BankrekeningnummerSamwise.Doel);
        bankrekeningnummer.Titularissen.Value.Should().Equal(BankrekeningnummerSamwise.Titularissen);
        bankrekeningnummer
            .BevestigdDoor.Select(x => new { x.OvoCode, x.Naam })
            .Should()
            .BeEquivalentTo(BankrekeningnummerSamwise.BevestigdDoor.Select(x => new { x.OvoCode, x.Naam }));
    }

    [Fact]
    public void Then_The_Next_Bankrekeningnummer_Gets_A_Free_Id() => _state.Bankrekeningnummers.NextId.Should().Be(3);
}
#pragma warning restore CS0618
