namespace AssociationRegistry.Test.Common.Scenarios.CommandHandling;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using global::AutoFixture;
using Vereniging;

public class VerenigingAanvaarddeDubbeleVerenigingScenario : CommandhandlerScenarioBase
{
    private List<IEvent> _events;

    public enum Verenigingstype
    {
        Feitelijke,
        MetRechtspersoonlijkheid
    }

    public string AuthentiekeVerenigingWerdGeregistreerdVCode { get; }
    public VerenigingAanvaarddeDubbeleVereniging VerenigingAanvaarddeDubbeleVereniging { get; set; }

    public VerenigingAanvaarddeDubbeleVerenigingScenario(Verenigingstype type)
    {
        var fixture = new Fixture().CustomizeDomain();

        _events = new List<IEvent>();
        switch (type)
        {
            case Verenigingstype.Feitelijke:
                var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
                _events.Add(feitelijkeVerenigingWerdGeregistreerd);
                AuthentiekeVerenigingWerdGeregistreerdVCode = feitelijkeVerenigingWerdGeregistreerd.VCode;
                break;

            case Verenigingstype.MetRechtspersoonlijkheid:
                var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
                _events.Add(verenigingMetRechtspersoonlijkheidWerdGeregistreerd);
                AuthentiekeVerenigingWerdGeregistreerdVCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        VerenigingAanvaarddeDubbeleVereniging = fixture.Create<VerenigingAanvaarddeDubbeleVereniging>() with
        {
            VCode = AuthentiekeVerenigingWerdGeregistreerdVCode,
        };
        _events.Add(VerenigingAanvaarddeDubbeleVereniging);
    }

    public override VCode VCode => VCode.Create(AuthentiekeVerenigingWerdGeregistreerdVCode);

    public override IEnumerable<IEvent> Events()
        => _events;
}
