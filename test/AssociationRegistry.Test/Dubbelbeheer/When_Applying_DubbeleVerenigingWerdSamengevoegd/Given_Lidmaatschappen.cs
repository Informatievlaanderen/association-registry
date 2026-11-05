namespace AssociationRegistry.Test.Dubbelbeheer.When_Applying_DubbeleVerenigingWerdSamengevoegd;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using static AssociationRegistry.Events.SamenvoegingDubbeleVereniging;

/// <summary>
///     Het samengevoegde lidmaatschap vervangt het bestaande met hetzelfde id; het toegevoegde komt erbij.
/// </summary>
#pragma warning disable CS0618
public class Given_Lidmaatschappen
{
    private readonly VerenigingState _state;
    private readonly Registratiedata.Lidmaatschap _verruimd;
    private readonly Registratiedata.Lidmaatschap _toegevoegd;

    public Given_Lidmaatschappen()
    {
        var fixture = new Fixture().CustomizeDomain();

        var registratie = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        var rohan = fixture.Create<VCode>();

        var bestaand = new Registratiedata.Lidmaatschap(
            LidmaatschapId: 1,
            AndereVereniging: rohan,
            AndereVerenigingNaam: "Rohan",
            DatumVan: new DateOnly(2020, 1, 1),
            DatumTot: new DateOnly(2021, 12, 31),
            Identificatie: "ROHAN",
            Beschrijving: ""
        );

        _verruimd = bestaand with { DatumVan = new DateOnly(2019, 6, 1) };

        _toegevoegd = new Registratiedata.Lidmaatschap(
            LidmaatschapId: 2,
            AndereVereniging: fixture.Create<VCode>(),
            AndereVerenigingNaam: "Erebor",
            DatumVan: new DateOnly(2024, 1, 1),
            DatumTot: null,
            Identificatie: "Dwergen",
            Beschrijving: "Lid van de Raad van Erebor"
        );

        var samengevoegd = EventFactory.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
            VCode.Create(registratie.VCode),
            fixture.Create<VCode>(),
            OvergenomenGegevensPerGroep<Registratiedata.Vertegenwoordiger, Bankrekeningnummer>.WithOnly(
                OvergenomenBasisgegevens.Geen
            ) with
            {
                Lidmaatschappen =
                [
                    Samengevoegd(
                        "samengevoegd",
                        bestaand with
                        {
                            DatumVan = new DateOnly(2019, 6, 1),
                            DatumTot = new DateOnly(2020, 6, 30),
                        },
                        _verruimd
                    ),
                    new OvergenomenGegeven<Registratiedata.Lidmaatschap>(
                        Acties.Toegevoegd,
                        "toegevoegd",
                        _toegevoegd with
                        {
                            LidmaatschapId = 7,
                        },
                        _toegevoegd
                    ),
                ],
            },
            NietOvergenomenGegevensPerGroep.WithOnly(NietOvergenomenBasisgegevens.Geen)
        );

        _state = new VerenigingState()
            .Apply(registratie)
            .Apply(new LidmaatschapWerdToegevoegd(registratie.VCode, bestaand))
            .Apply(samengevoegd);
    }

    [Fact]
    public void Then_The_Samengevoegde_Lidmaatschap_Replaces_The_Existing_One() =>
        EventFactory
            .Lidmaatschap(_state.Lidmaatschappen.Single(x => x.LidmaatschapId == 1))
            .Should()
            .BeEquivalentTo(_verruimd);

    [Fact]
    public void Then_The_Toegevoegde_Lidmaatschap_Is_Added() =>
        EventFactory
            .Lidmaatschap(_state.Lidmaatschappen.Single(x => x.LidmaatschapId == 2))
            .Should()
            .BeEquivalentTo(_toegevoegd);

    [Fact]
    public void Then_There_Are_Two_Lidmaatschappen() => _state.Lidmaatschappen.Should().HaveCount(2);

    [Fact]
    public void Then_The_Next_Lidmaatschap_Gets_A_Free_Id() => ((int)_state.Lidmaatschappen.NextId).Should().Be(3);
}
#pragma warning restore CS0618
