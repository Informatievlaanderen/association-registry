namespace AssociationRegistry.Test.Admin.Api.TakeTwo;

using AssociationRegistry.Framework;
using Events;
using VCodes;

public interface ICommandhandlerScenario
{
    IEnumerable<IEvent> Events();
}

public class Empty_Commandhandler_Scenario : ICommandhandlerScenario
{
    public IEnumerable<IEvent> Events()
        => Array.Empty<IEvent>();
}

public class VerenigingWerdGeregistreed_Commandhandler_Scenario : ICommandhandlerScenario
{
    public readonly VCode VCode = VCode.Create("V0001002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string? KorteBeschrijving = null;
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = null;
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly? Startdatum = null;

    public IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            new VerenigingWerdGeregistreerd(
                VCode,
                Naam,
                KorteNaam,
                KorteBeschrijving,
                Startdatum,
                KboNummer,
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>()),
        };
    }
}
