namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework;
using Vereniging;

public abstract class CommandhandlerScenarioBase
{
    public abstract IEnumerable<IEvent> Events();

    public Vereniging GetVereniging()
    {
        var vereniging = new Vereniging();

        foreach (var evnt in Events()) vereniging.Apply((dynamic)evnt);

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
    public const int ContactgegevenId = 1;
    public const string Waarde = "test@example.org";
    public const string Beschrijving = "";
    public const bool IsPrimair = true;
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly string KorteNaam = "FOud";

    public readonly string Naam = "Hulste Huldigt";
    public readonly DateOnly Startdatum = new(year: 2023, month: 3, day: 6);
    public readonly ContactgegevenType Type = ContactgegevenType.Email;
    public readonly VCode VCode = VCode.Create("V0009002");

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            new VerenigingWerdGeregistreerd(
                VCode,
                VerenigingsType.FeitelijkeVereniging.Code,
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
    public VerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = fixture.Create<VCode>();
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with { VCode = VCode };
        VertegenwoordigerWerdToegevoegd = fixture.Create<VertegenwoordigerWerdToegevoegd>() with { IsPrimair = true };
        VertegenwoordigerWerdToegevoegd2 = fixture.Create<VertegenwoordigerWerdToegevoegd>();
    }

    public VCode VCode { get; }
    public VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd { get; }
    public VertegenwoordigerWerdToegevoegd VertegenwoordigerWerdToegevoegd2 { get; }

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
    public readonly string Initiator = "Een initiator";
    public readonly VCode VCode = VCode.Create("V0009002");

    public VerenigingWerdGeregistreerd_WithMultipleContactgegevens_Commandhandler_Scenario()
    {
        ContactgegevenWerdToegevoegd1 = new ContactgegevenWerdToegevoegd(ContactgegevenId: 1, ContactgegevenType.Email, "test1@example.org", "", IsPrimair: true);
        ContactgegevenWerdToegevoegd2 = new ContactgegevenWerdToegevoegd(ContactgegevenId: 2, ContactgegevenType.Email, "test2@example.org", "", IsPrimair: false);
    }

    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd1 { get; }
    public ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd2 { get; }

    public override IEnumerable<IEvent> Events()
    {
        return new IEvent[]
        {
            new VerenigingWerdGeregistreerd(
                VCode,
                VerenigingsType.FeitelijkeVereniging.Code,
                "Hulste Huldigt",
                "FOud",
                string.Empty,
                new DateOnly(year: 2023, month: 3, day: 6),
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
    public const string Naam = "Hulste Huldigt";
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly DateOnly Startdatum = new(year: 2023, month: 3, day: 6);
    public readonly VCode VCode = VCode.Create("V0009002");
    public VerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;

    public override IEnumerable<IEvent> Events()
    {
        WerdGeregistreerd = new VerenigingWerdGeregistreerd(
            VCode,
            VerenigingsType.FeitelijkeVereniging.Code,
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
    public const string Naam = "Hulste Huldigt";
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly DateOnly Startdatum = new(year: 2023, month: 3, day: 6);
    public readonly VCode VCode = VCode.Create("V0009002");

    public VerenigingWerdGeregistreerdWithRemovedContactgegevenScenario()
    {
        Contactgegevens = new[] { new Fixture().CustomizeAll().Create<VerenigingWerdGeregistreerd.Contactgegeven>() with { ContactgegevenId = 1 } };
    }

    public VerenigingWerdGeregistreerd.Contactgegeven[] Contactgegevens { get; }
    public VerenigingWerdGeregistreerd WerdGeregistreerd { get; private set; } = null!;
    public ContactgegevenWerdVerwijderd ContactgegevenWerdVerwijderd { get; private set; } = null!;

    public override IEnumerable<IEvent> Events()
    {
        WerdGeregistreerd = new VerenigingWerdGeregistreerd(
            VCode,
            VerenigingsType.FeitelijkeVereniging.Code,
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
    public const string Naam = "Hulste Huldigt";
    public const string KorteNaam = "FOud";
    public readonly string Initiator = "Een initiator";
    public readonly string KorteBeschrijving = string.Empty;
    public readonly VerenigingWerdGeregistreerd.Locatie Locatie;
    public readonly DateOnly? Startdatum = null;
    public readonly VCode VCode = VCode.Create("V0009002");

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
                VerenigingsType.FeitelijkeVereniging.Code,
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
