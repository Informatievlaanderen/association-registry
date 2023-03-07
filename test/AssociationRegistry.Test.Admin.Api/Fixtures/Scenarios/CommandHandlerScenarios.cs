namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using Events;
using AssociationRegistry.Framework;
using AutoFixture;
using Framework;
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

public class VerenigingWerdGeregistreerd_Commandhandler_Scenario : ICommandhandlerScenario
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string? KorteBeschrijving = null;
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = null;
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly Startdatum = new(2023, 3, 6);

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
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
        };
    }
}

public class VerenigingWerdGeregistreerd_With_Location_Commandhandler_Scenario : ICommandhandlerScenario
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string? KorteBeschrijving = null;
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = null;
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly? Startdatum = null;
    public readonly VerenigingWerdGeregistreerd.Locatie Locatie;

    public VerenigingWerdGeregistreerd_With_Location_Commandhandler_Scenario()
    {
        var fixture = new Fixture().CustomizeAll();
        Locatie = fixture.Create<VerenigingWerdGeregistreerd.Locatie>();
    }

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
                new[] { Locatie },
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
        };
    }
}
