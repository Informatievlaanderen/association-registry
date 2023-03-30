namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using Events;
using AssociationRegistry.Framework;
using AutoFixture;
using Events.CommonEventDataTypes;
using Framework;
using VCodes;
using Vereniging;

public abstract class CommandhandlerScenarioBase
{
    public abstract IEnumerable<IEvent> Events();

    public Vereniging GetVereniging()
    {
        var vereniging = new Vereniging();

        foreach (var evnt in Events())
        {
            vereniging.Apply((dynamic)evnt);
        }

        return vereniging;
    }
}

public class Empty_Commandhandler_ScenarioBase : CommandhandlerScenarioBase
{
    public override IEnumerable<IEvent> Events()
        => Array.Empty<IEvent>();
}

public class VerenigingWerdGeregistreerd_Commandhandler_Scenario : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string? KorteBeschrijving = null;
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = null;
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly Startdatum = new(2023, 3, 6);

    public override IEnumerable<IEvent> Events()
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
                Array.Empty<ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
        };
    }
}

public class VerenigingWerdGeregistreerd_WithAPrimairEmailContactgegeven_Commandhandler_Scenario : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string? KorteBeschrijving = null;
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = null;
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly Startdatum = new(2023, 3, 6);

    public override IEnumerable<IEvent> Events()
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
                Array.Empty<ContactInfo>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
            new ContactgegevenWerdToegevoegd(1, "email", "test@example.org", "", true),
        };
    }
}

public class VerenigingWerdGeregistreerdWithContactInfo_Commandhandler_ScenarioBase : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string? KorteBeschrijving = null;
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = null;
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly Startdatum = new(2023, 3, 6);
    public ContactInfo[] ContactInfoLijst { get; }
    public VerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;

    public VerenigingWerdGeregistreerdWithContactInfo_Commandhandler_ScenarioBase()
    {
        ContactInfoLijst = new Fixture().CustomizeAll().CreateMany<ContactInfo>().ToArray();
    }

    public override IEnumerable<IEvent> Events()
    {
        WerdGeregistreerd = new VerenigingWerdGeregistreerd(
            VCode,
            Naam,
            KorteNaam,
            KorteBeschrijving,
            Startdatum,
            KboNummer,
            ContactInfoLijst,
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>());
        return new IEvent[]
        {
            WerdGeregistreerd,
        };
    }
}

public class VerenigingWerdGeregistreerd_With_Location_Commandhandler_ScenarioBase : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string? KorteBeschrijving = null;
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = null;
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly? Startdatum = null;
    public readonly VerenigingWerdGeregistreerd.Locatie Locatie;

    public VerenigingWerdGeregistreerd_With_Location_Commandhandler_ScenarioBase()
    {
        var fixture = new Fixture().CustomizeAll();
        Locatie = fixture.Create<VerenigingWerdGeregistreerd.Locatie>();
    }

    public override IEnumerable<IEvent> Events()
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
                Array.Empty<ContactInfo>(),
                new[] { Locatie },
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
        };
    }
}
