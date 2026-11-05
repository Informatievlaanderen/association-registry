namespace AssociationRegistry.Test.Dubbelbeheer.When_Applying_DubbeleVerenigingWerdSamengevoegd;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using static AssociationRegistry.Events.SamenvoegingDubbeleVereniging;
using Erkenning = AssociationRegistry.Events.SamenvoegingDubbeleVereniging.Erkenning;

/// <summary>De overgenomen erkenning komt bij de bestaande, met de nieuwe id.</summary>
#pragma warning disable CS0618
public class Given_Erkenningen
{
    private static readonly GegevensInitiator Gondor = GegevensInitiator.Hydrate("OVO000202", "Gondor");

    private readonly VerenigingState _state;
    private readonly ErkenningWerdGeregistreerd _bestaand;
    private readonly Erkenning _overgenomen;

    public Given_Erkenningen()
    {
        var fixture = new Fixture().CustomizeDomain();

        var registratie = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        _bestaand = new ErkenningWerdGeregistreerd(
            ErkenningId: 1,
            IpdcProduct: new IpdcProduct { Nummer = "1001", Naam = "Lembas bakken" },
            Startdatum: new DateOnly(2020, 1, 1),
            Einddatum: new DateOnly(2021, 12, 31),
            Hernieuwingsdatum: null,
            HernieuwingsUrl: string.Empty,
            GeregistreerdDoor: GegevensInitiator.Hydrate("OVO000101", "Rivendell"),
            Status: ErkenningStatus.Verlopen.Value
        );

        _overgenomen = new Erkenning(
            ErkenningId: 2,
            IpdcProduct: new IpdcProduct { Nummer = "1002", Naam = "Ringdragers begeleiden" },
            Startdatum: new DateOnly(2024, 1, 1),
            Einddatum: null,
            Hernieuwingsdatum: new DateOnly(2030, 1, 1),
            HernieuwingsUrl: "https://www.minas-tirith.gondor/hernieuwen",
            GeregistreerdDoor: Gondor,
            RedenSchorsing: string.Empty,
            Status: ErkenningStatus.Actief.Value
        );

        var samengevoegd = EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            VCode.Create(registratie.VCode),
            fixture.Create<VCode>(),
            OvergenomenGegevensPerGroep<Registratiedata.Vertegenwoordiger, Bankrekeningnummer>.WithOnly(
                OvergenomenBasisgegevens.Geen
            ) with
            {
                Erkenningen =
                [
                    new OvergenomenGegeven<Erkenning>(
                        Acties.Toegevoegd,
                        "toegevoegd",
                        _overgenomen with
                        {
                            ErkenningId = 5,
                        },
                        _overgenomen
                    ),
                ],
            },
            NietOvergenomenGegevensPerGroep.WithOnly(NietOvergenomenBasisgegevens.Geen)
        );

        _state = new VerenigingState().Apply(registratie).Apply(_bestaand).Apply(samengevoegd);
    }

    [Fact]
    public void Then_The_Existing_Erkenning_Is_Kept() =>
        _state.Erkenningen.Should().ContainSingle(x => x.ErkenningId == 1 && x.IpdcProduct.Nummer == "1001");

    [Fact]
    public void Then_The_Nieuwe_Waarde_Is_Added()
    {
        var erkenning = _state.Erkenningen.Single(x => x.ErkenningId == 2);

        erkenning.IpdcProduct.Should().Be(_overgenomen.IpdcProduct);
        erkenning.GeregistreerdDoor.Should().Be(Gondor);
        erkenning
            .ErkenningsPeriode.Should()
            .Be(ErkenningsPeriode.Hydrate(_overgenomen.Startdatum, _overgenomen.Einddatum));
        erkenning.Hernieuwingsdatum.Should().Be(Hernieuwingsdatum.Hydrate(_overgenomen.Hernieuwingsdatum));
        erkenning.HernieuwingsUrl.Should().Be(HernieuwingsUrl.Hydrate(_overgenomen.HernieuwingsUrl));
        erkenning.Status.Should().Be(ErkenningStatus.Actief);
    }

    [Fact]
    public void Then_The_Next_Erkenning_Gets_A_Free_Id() => _state.Erkenningen.NextId.Should().Be(3);
}
#pragma warning restore CS0618
