namespace AssociationRegistry.Test.Projections.Scenario.VertegenwoordigerPersoonsgegevens.Samenvoeging;

using AssociationRegistry.Events;
using AutoFixture;
using Dubbels;
using static AssociationRegistry.Test.Common.DubbelMerge.TestVertegenwoordigers;

/// <summary>
///     Na de samenvoeging verloopt de bewaartermijn van een vertegenwoordiger-id op de authentieke vereniging.
///     In de samenvoeging is Frodo (id 1) de originele waarde, Samwise (id 3) de nieuwe waarde en Aragorn (id 2)
///     niet overgenomen.
/// </summary>
#pragma warning disable CS0618
public abstract class VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterSamenvoegingScenario : ScenarioBase
{
    private readonly DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario _scenario = new();

    public DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging Samengevoegd => _scenario.Samengevoegd;
    public VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd { get; }
    public int VertegenwoordigerIdDieGeanonimiseerdWerd { get; }

    protected VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterSamenvoegingScenario(int vertegenwoordigerId)
    {
        VertegenwoordigerIdDieGeanonimiseerdWerd = vertegenwoordigerId;

        VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd =
            AutoFixture.Create<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd>() with
            {
                VCode = _scenario.AggregateId,
                VertegenwoordigerId = vertegenwoordigerId,
            };
    }

    public override string AggregateId => _scenario.AggregateId;

    public override EventsPerVCode[] Events =>
        [
            new(
                AggregateId,
                _scenario.AuthentiekeVerenigingWerdGeregistreerd,
                _scenario.Samengevoegd,
                VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd
            ),
        ];
}

public class VertegenwoordigerPersoonsgegevensVanDeNieuweWaardeWerdenGeanonimiseerdScenario()
    : VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterSamenvoegingScenario(Samwise.VertegenwoordigerId);

public class VertegenwoordigerPersoonsgegevensVanDeOrigineleWaardeWerdenGeanonimiseerdScenario()
    : VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterSamenvoegingScenario(Frodo.VertegenwoordigerId);

public class VertegenwoordigerPersoonsgegevensVanEenNietOvergenomenVertegenwoordigerWerdenGeanonimiseerdScenario()
    : VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterSamenvoegingScenario(Aragorn.VertegenwoordigerId);

/// <summary>Een id dat niet in de samenvoeging voorkomt.</summary>
public class VertegenwoordigerPersoonsgegevensBuitenDeSamenvoegingWerdenGeanonimiseerdScenario()
    : VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdAfterSamenvoegingScenario(
        VertegenwoordigerIdNietInSamenvoeging
    )
{
    public const int VertegenwoordigerIdNietInSamenvoeging = 99;
}
#pragma warning restore CS0618
