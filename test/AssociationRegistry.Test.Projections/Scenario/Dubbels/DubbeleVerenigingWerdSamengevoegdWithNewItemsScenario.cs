namespace AssociationRegistry.Test.Projections.Scenario.Dubbels;

using AssociationRegistry.Test.Common.DubbelMerge;
using AutoFixture;
using Events;
using static AssociationRegistry.Test.Common.DubbelMerge.TestBankrekeningnummers;
using static AssociationRegistry.Test.Common.DubbelMerge.TestVertegenwoordigers;

/// <summary>
///     De authentieke vereniging heeft al één item per groep (met id 1). De overgenomen items krijgen andere ids,
///     dus ze worden toegevoegd naast de bestaande.
/// </summary>
#pragma warning disable CS0618
public class DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario : ScenarioBase
{
    public VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd { get; }
    public DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging Samengevoegd { get; }

    public DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario()
    {
        AuthentiekeVerenigingWerdGeregistreerd =
            AutoFixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                HoofdactiviteitenVerenigingsloket =
                [
                    new Registratiedata.HoofdactiviteitVerenigingsloket("JEUG", "Jeugd"),
                ],
                Contactgegevens = [AutoFixture.Create<Registratiedata.Contactgegeven>() with { ContactgegevenId = 1 }],
                Locaties = [AutoFixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 }],
                Vertegenwoordigers =
                [
                    AutoFixture.Create<Registratiedata.Vertegenwoordiger>() with
                    {
                        VertegenwoordigerId = 1,
                    },
                ],
                Bankrekeningnummers =
                [
                    AutoFixture.Create<Registratiedata.Bankrekeningnummer>() with
                    {
                        BankrekeningnummerId = 1,
                    },
                ],
            };

        Samengevoegd = new DubbelMergeEventBuilder(
            AuthentiekeVerenigingWerdGeregistreerd.VCode,
            AutoFixture.Create<string>()
        )
            .WithDataInAllOtherGroups()
            .WithOvergenomenVertegenwoordiger(Frodo, Samwise)
            .WithNietOvergenomenVertegenwoordiger(Aragorn.VertegenwoordigerId)
            .WithOvergenomenBankrekeningnummer(BankrekeningnummerFrodo, BankrekeningnummerSamwise)
            .WithNietOvergenomenBankrekeningnummer(BankrekeningnummerBilbo.BankrekeningnummerId)
            .Build();
    }

    public override string AggregateId => AuthentiekeVerenigingWerdGeregistreerd.VCode;

    public override EventsPerVCode[] Events => [new(AggregateId, AuthentiekeVerenigingWerdGeregistreerd, Samengevoegd)];
}
#pragma warning restore CS0618
