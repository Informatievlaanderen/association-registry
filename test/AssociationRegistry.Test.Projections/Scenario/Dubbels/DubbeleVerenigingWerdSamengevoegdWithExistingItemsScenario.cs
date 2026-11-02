namespace AssociationRegistry.Test.Projections.Scenario.Dubbels;

using AssociationRegistry.Test.Common.DubbelMerge;
using AutoFixture;
using Events;
using static AssociationRegistry.Test.Common.DubbelMerge.TestBankrekeningnummers;
using static AssociationRegistry.Test.Common.DubbelMerge.TestVertegenwoordigers;

/// <summary>
///     De authentieke vereniging heeft al een item met hetzelfde id (of dezelfde code) als elk overgenomen item,
///     maar met andere waarden. De samenvoeging moet die items vervangen door de nieuwe waarde.
/// </summary>
#pragma warning disable CS0618
public class DubbeleVerenigingWerdSamengevoegdWithExistingItemsScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public WerkingsgebiedenWerdenBepaald WerkingsgebiedenWerdenBepaald { get; }
    public ErkenningWerdGeregistreerd ErkenningWerdGeregistreerd { get; }
    public LidmaatschapWerdToegevoegd LidmaatschapWerdToegevoegd { get; }
    public VerenigingssubtypeWerdVerfijndNaarSubvereniging VerfijndNaarSubvereniging { get; }
    public DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging Samengevoegd { get; }

    public DubbeleVerenigingWerdSamengevoegdWithExistingItemsScenario()
    {
        var registratie = AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        var vCode = registratie.VCode;

        Samengevoegd = new DubbelMergeEventBuilder(vCode, AutoFixture.Create<string>())
            .WithDataInAllOtherGroups()
            .WithOvergenomenVertegenwoordiger(Frodo, Samwise)
            .WithOvergenomenBankrekeningnummer(BankrekeningnummerFrodo, BankrekeningnummerSamwise)
            .Build();

        var overgenomen = Samengevoegd.OvergenomenGegevens;

        AuthentiekeVerenigingWerdGeregistreerd = registratie with
        {
            HoofdactiviteitenVerenigingsloket =
            [
                overgenomen.Hoofdactiviteiten.Single().NieuweWaarde with
                {
                    Naam = "Oude naam",
                },
            ],
            Contactgegevens =
            [
                AutoFixture.Create<Registratiedata.Contactgegeven>() with
                {
                    ContactgegevenId = overgenomen.Contactgegevens.Single().NieuweWaarde.ContactgegevenId,
                },
            ],
            Locaties =
            [
                AutoFixture.Create<Registratiedata.Locatie>() with
                {
                    LocatieId = overgenomen.Locaties.Single().NieuweWaarde.LocatieId,
                },
            ],
            Vertegenwoordigers =
            [
                AutoFixture.Create<Registratiedata.Vertegenwoordiger>() with
                {
                    VertegenwoordigerId = overgenomen.Vertegenwoordigers.Single().NieuweWaarde.VertegenwoordigerId,
                },
            ],
            Bankrekeningnummers =
            [
                AutoFixture.Create<Registratiedata.Bankrekeningnummer>() with
                {
                    BankrekeningnummerId = overgenomen.Bankrekeningnummers.Single().NieuweWaarde.BankrekeningnummerId,
                },
            ],
        };

        WerkingsgebiedenWerdenBepaald = new WerkingsgebiedenWerdenBepaald(
            vCode,
            [overgenomen.Werkingsgebieden.Single().NieuweWaarde with { Naam = "Oude naam" }]
        );

        ErkenningWerdGeregistreerd = AutoFixture.Create<ErkenningWerdGeregistreerd>() with
        {
            ErkenningId = overgenomen.Erkenningen.Single().NieuweWaarde.ErkenningId,
        };

        LidmaatschapWerdToegevoegd = new LidmaatschapWerdToegevoegd(
            vCode,
            AutoFixture.Create<Registratiedata.Lidmaatschap>() with
            {
                LidmaatschapId = overgenomen.Lidmaatschappen.Single().NieuweWaarde.LidmaatschapId,
            }
        );

        VerfijndNaarSubvereniging = AutoFixture.Create<VerenigingssubtypeWerdVerfijndNaarSubvereniging>() with
        {
            VCode = vCode,
        };
    }

    public override string AggregateId => AuthentiekeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                AuthentiekeVerenigingWerdGeregistreerd,
                WerkingsgebiedenWerdenBepaald,
                ErkenningWerdGeregistreerd,
                LidmaatschapWerdToegevoegd,
                VerfijndNaarSubvereniging,
                Samengevoegd
            ),
        ];
}
#pragma warning restore CS0618
