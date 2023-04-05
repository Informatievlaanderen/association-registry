namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using Events;
using AssociationRegistry.Framework;
using AutoFixture;
using ContactGegevens;
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
            try
            {
                vereniging.Apply((dynamic)evnt);
            }
            catch
            {
                // ignored
            }
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
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
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
    public readonly int ContactgegevenId = 1;
    public readonly ContactgegevenType Type = ContactgegevenType.Email;
    public readonly string Waarde = "test@example.org";
    public readonly string Omschrijving = "";
    public readonly bool IsPrimair = true;

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
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
            new ContactgegevenWerdToegevoegd(ContactgegevenId, Type, Waarde, Omschrijving, IsPrimair),
        };
    }
}

public class VerenigingWerdGeregistreerdWithContactgegeven_Commandhandler_ScenarioBase : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string? KorteBeschrijving = null;
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = null;
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly Startdatum = new(2023, 3, 6);
    public VerenigingWerdGeregistreerd.Contactgegeven[] Contactgegevens { get; }
    public VerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;

    public VerenigingWerdGeregistreerdWithContactgegeven_Commandhandler_ScenarioBase()
    {
        Contactgegevens = new Fixture().CustomizeAll().CreateMany<VerenigingWerdGeregistreerd.Contactgegeven>().ToArray();
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
            Contactgegevens,
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>());
        return new IEvent[]
        {
            WerdGeregistreerd,
        };
    }
}

public class VerenigingWerdGeregistreerdWithRemovedContactgegeven_Commandhandler_ScenarioBase : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string? KorteBeschrijving = null;
    public readonly string? KorteNaam = "FOud";
    public readonly string? KboNummer = null;
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly Startdatum = new(2023, 3, 6);
    public VerenigingWerdGeregistreerd.Contactgegeven[] Contactgegevens { get; }
    public VerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;
    public ContactgegevenWerdVerwijderd ContactgegevenWerdVerwijderd { get; private set; } = null!;

    public VerenigingWerdGeregistreerdWithRemovedContactgegeven_Commandhandler_ScenarioBase()
    {
        Contactgegevens = new[] { new Fixture().CustomizeAll().Create<VerenigingWerdGeregistreerd.Contactgegeven>() with { ContactgegevenId = 1 } };
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
            Contactgegevens,
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>());
        ContactgegevenWerdVerwijderd = new ContactgegevenWerdVerwijderd(Contactgegevens[0].ContactgegevenId, Contactgegevens[0].Type, Contactgegevens[0].Waarde, Contactgegevens[0].Omschrijving, Contactgegevens[0].IsPrimair);
        return new IEvent[]
        {
            WerdGeregistreerd,
            ContactgegevenWerdVerwijderd,
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
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                new[] { Locatie },
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
        };
    }
}
