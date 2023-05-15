namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using Events;
using AssociationRegistry.Framework;
using AutoFixture;
using Framework;
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

public class VerenigingWerdGeregistreerdScenario : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;

    public VerenigingWerdGeregistreerdScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
    }

    public override IEnumerable<IEvent> Events()
        => new IEvent[]
        {
            VerenigingWerdGeregistreerd,
        };
}

public class VerenigingWerdGeregistreerdWithAPrimairEmailContactgegevenScenario : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public readonly string Naam = "Hulste Huldigt";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly Startdatum = new(2023, 3, 6);
    public const int ContactgegevenId = 1;
    public readonly ContactgegevenType Type = ContactgegevenType.Email;
    public const string Waarde = "test@example.org";
    public const string Beschrijving = "";
    public const bool IsPrimair = true;

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
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
            new ContactgegevenWerdToegevoegd(ContactgegevenId, Type, Waarde, Beschrijving, IsPrimair),
        };
    }
}

public class VerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario : CommandhandlerScenarioBase
{
    public VCode VCode { get; }
    public VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd2 { get; }

    public VerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = fixture.Create<VCode>();
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>() with { IsPrimair = true };
        VertegenwoordigerWerdToegevoegd2 = fixture.Create<VertegenwoordigerWerdToegevoegd>();
    }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            VerenigingWerdGeregistreerd,
            VertegenwoordigerWerdToegevoegd,
            VertegenwoordigerWerdToegevoegd2,
        };
    }
}

public class VerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");
    public readonly string Initiator = "Een initiator";
    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd1 { get; }
    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd2 { get; }

    public VerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario()
    {
        ContactgegevenWerdToegevoegd1 = new ContactgegevenWerdToegevoegd(1, ContactgegevenType.Email, "test1@example.org", "", true);
        ContactgegevenWerdToegevoegd2 = new ContactgegevenWerdToegevoegd(2, ContactgegevenType.Email, "test2@example.org", "", false);
    }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            new VerenigingWerdGeregistreerd(
                VCode,
                "Hulste Huldigt",
                "FOud",
                string.Empty,
                new DateOnly(2023, 3, 6),
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
            ContactgegevenWerdToegevoegd1,
            ContactgegevenWerdToegevoegd2,
        };
    }
}

public class VerenigingWerdGeregistreerdWithoutVertegenwoordigers : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public const string Naam = "Hulste Huldigt";
    public readonly string KorteBeschrijving = string.Empty;
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly Startdatum = new(2023, 3, 6);
    public VerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;

    public override IEnumerable<IEvent> Events()
    {
        WerdGeregistreerd = new VerenigingWerdGeregistreerd(
            VCode,
            Naam,
            KorteNaam,
            KorteBeschrijving,
            Startdatum,
            Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>());
        return new IEvent[]
        {
            WerdGeregistreerd,
        };
    }
}

public class VerenigingWerdGeregistreerdWithRemovedContactgegevenScenario : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public const string Naam = "Hulste Huldigt";
    public readonly string KorteBeschrijving = string.Empty;
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly Startdatum = new(2023, 3, 6);
    public VerenigingWerdGeregistreerd.Contactgegeven[] Contactgegevens { get; }
    public VerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;
    public ContactgegevenWerdVerwijderd ContactgegevenWerdVerwijderd { get; private set; } = null!;

    public VerenigingWerdGeregistreerdWithRemovedContactgegevenScenario()
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
            Contactgegevens,
            Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
            Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
            Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>());
        ContactgegevenWerdVerwijderd = new ContactgegevenWerdVerwijderd(Contactgegevens[0].ContactgegevenId, Contactgegevens[0].Type, Contactgegevens[0].Waarde, Contactgegevens[0].Beschrijving, Contactgegevens[0].IsPrimair);
        return new IEvent[]
        {
            WerdGeregistreerd,
            ContactgegevenWerdVerwijderd,
        };
    }
}

public class VerenigingWerdGeregistreerdWithLocationScenario : CommandhandlerScenarioBase
{
    public readonly VCode VCode = VCode.Create("V0009002");

    public const string Naam = "Hulste Huldigt";
    public readonly string KorteBeschrijving = string.Empty;
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly DateOnly? Startdatum = null;
    public readonly VerenigingWerdGeregistreerd.Locatie Locatie;

    public VerenigingWerdGeregistreerdWithLocationScenario()
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
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                new[] { Locatie },
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()),
        };
    }
}
