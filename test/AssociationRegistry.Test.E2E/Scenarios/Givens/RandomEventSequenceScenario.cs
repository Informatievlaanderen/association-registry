namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using MartenDb.Store;

public class RandomEventSequenceScenario : Framework.TestClasses.IScenario
{
    private CommandMetadata Metadata;
    private readonly Random _random;
    private readonly int _numberOfAssociations;
    private readonly int _maxEventsPerAssociation;

    public RandomEventSequenceScenario(int numberOfAssociations = 100, int maxEventsPerAssociation = 50, int? seed = null)
    {
        _numberOfAssociations = numberOfAssociations;
        _maxEventsPerAssociation = maxEventsPerAssociation;
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var fixture = new Fixture().CustomizeAdminApi();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };

        var result = new List<KeyValuePair<string, IEvent[]>>();

        for (int i = 0; i < _numberOfAssociations; i++)
        {
            var vCode = $"V{i:D6}";
            var events = GenerateEventSequenceForAssociation(vCode, fixture);
            result.Add(new KeyValuePair<string, IEvent[]>(vCode, events.ToArray()));
        }

        return result.ToArray();
    }

    private List<IEvent> GenerateEventSequenceForAssociation(string vCode, Fixture fixture)
    {
        var events = new List<IEvent>();
        var state = new AssociationState(vCode);

        // Always start with registration
        var registrationEvent = CreateRegistrationEvent(vCode, fixture, state);
        events.Add(registrationEvent);

        var numberOfEvents = _random.Next(1, _maxEventsPerAssociation);

        for (int i = 0; i < numberOfEvents; i++)
        {
            if (state.IsDeleted)
                break; // No events after deletion

            var nextEvent = GenerateNextValidEvent(vCode, fixture, state);
            if (nextEvent != null)
            {
                events.Add(nextEvent);
            }
        }

        return events;
    }

    private IEvent CreateRegistrationEvent(string vCode, Fixture fixture, AssociationState state)
    {
        var registrationEvent = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = vCode
        };

        state.ApplyRegistration(registrationEvent);
        return registrationEvent;
    }

    private IEvent? GenerateNextValidEvent(string vCode, Fixture fixture, AssociationState state)
    {
        var availableEventTypes = GetAvailableEventTypes(state);
        if (!availableEventTypes.Any())
            return null;

        var eventType = availableEventTypes[_random.Next(availableEventTypes.Count)];

        return eventType switch
        {
            EventType.NaamWijziging => CreateNaamWijziging(vCode, fixture, state),
            EventType.ContactgegevenToevoegen => CreateContactgegevenToevoegen(fixture, state),
            EventType.ContactgegevenWijzigen => CreateContactgegevenWijzigen(fixture, state),
            EventType.ContactgegevenVerwijderen => CreateContactgegevenVerwijderen(state),
            EventType.LocatieToevoegen => CreateLocatieToevoegen(fixture, state),
            EventType.LocatieWijzigen => CreateLocatieWijzigen(fixture, state),
            EventType.LocatieVerwijderen => CreateLocatieVerwijderen(vCode, state),
            EventType.VertegenwoordigerToevoegen => CreateVertegenwoordigerToevoegen(fixture, state),
            EventType.VertegenwoordigerWijzigen => CreateVertegenwoordigerWijzigen(fixture, state),
            EventType.VertegenwoordigerVerwijderen => CreateVertegenwoordigerVerwijderen(state),
            EventType.DoelgroepWijzigen => CreateDoelgroepWijzigen(fixture, state),
            EventType.StartdatumWijzigen => CreateStartdatumWijzigen(vCode, fixture, state),
            EventType.VerenigingVerwijderen => CreateVerenigingVerwijderen(vCode, state),
            _ => null
        };
    }

    private List<EventType> GetAvailableEventTypes(AssociationState state)
    {
        var available = new List<EventType>
        {
            EventType.NaamWijziging,
            EventType.ContactgegevenToevoegen,
            EventType.LocatieToevoegen,
            EventType.VertegenwoordigerToevoegen,
            EventType.DoelgroepWijzigen,
            EventType.StartdatumWijzigen
        };

        // Add conditional events
        if (state.Contactgegevens.Any())
        {
            available.Add(EventType.ContactgegevenWijzigen);
            available.Add(EventType.ContactgegevenVerwijderen);
        }

        if (state.Locaties.Any())
        {
            available.Add(EventType.LocatieWijzigen);
            available.Add(EventType.LocatieVerwijderen);
        }

        if (state.Vertegenwoordigers.Any())
        {
            available.Add(EventType.VertegenwoordigerWijzigen);
            available.Add(EventType.VertegenwoordigerVerwijderen);
        }

        // Low probability of deletion
        if (_random.NextDouble() < 0.05) // 5% chance
        {
            available.Add(EventType.VerenigingVerwijderen);
        }

        return available;
    }

    #region Event Creation Methods

    private IEvent CreateNaamWijziging(string vCode, Fixture fixture, AssociationState state)
    {
        var newName = fixture.Create<string>();
        state.Naam = newName;
        return new NaamWerdGewijzigd(vCode, newName);
    }

    private IEvent CreateContactgegevenToevoegen(Fixture fixture, AssociationState state)
    {
        var contactgegeven = fixture.Create<Registratiedata.Contactgegeven>();
        state.Contactgegevens.Add(contactgegeven);

        return new ContactgegevenWerdToegevoegd(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            contactgegeven.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);
    }

    private IEvent CreateContactgegevenWijzigen(Fixture fixture, AssociationState state)
    {
        var existing = state.Contactgegevens[_random.Next(state.Contactgegevens.Count)];
        var updated = fixture.Create<Registratiedata.Contactgegeven>() with { ContactgegevenId = existing.ContactgegevenId };

        var index = state.Contactgegevens.FindIndex(c => c.ContactgegevenId == existing.ContactgegevenId);
        state.Contactgegevens[index] = updated;

        return new ContactgegevenWerdGewijzigd(
            updated.ContactgegevenId,
            updated.Contactgegeventype,
            updated.Waarde,
            updated.Beschrijving,
            updated.IsPrimair);
    }

    private IEvent CreateContactgegevenVerwijderen(AssociationState state)
    {
        var toRemove = state.Contactgegevens[_random.Next(state.Contactgegevens.Count)];
        state.Contactgegevens.Remove(toRemove);

        return new ContactgegevenWerdVerwijderd(
            toRemove.ContactgegevenId,
            toRemove.Contactgegeventype,
            toRemove.Waarde,
            toRemove.Beschrijving,
            toRemove.IsPrimair);
    }

    private IEvent CreateLocatieToevoegen(Fixture fixture, AssociationState state)
    {
        var locatie = fixture.Create<Registratiedata.Locatie>();
        state.Locaties.Add(locatie);

        return new LocatieWerdToegevoegd(locatie);
    }

    private IEvent CreateLocatieWijzigen(Fixture fixture, AssociationState state)
    {
        var existing = state.Locaties[_random.Next(state.Locaties.Count)];
        var updated = fixture.Create<Registratiedata.Locatie>() with { LocatieId = existing.LocatieId };

        var index = state.Locaties.FindIndex(l => l.LocatieId == existing.LocatieId);
        state.Locaties[index] = updated;

        return new LocatieWerdGewijzigd(updated);
    }

    private IEvent CreateLocatieVerwijderen(string vCode, AssociationState state)
    {
        var toRemove = state.Locaties[_random.Next(state.Locaties.Count)];
        state.Locaties.Remove(toRemove);

        return new LocatieWerdVerwijderd(vCode, toRemove);
    }

    private IEvent CreateVertegenwoordigerToevoegen(Fixture fixture, AssociationState state)
    {
        var vertegenwoordiger = fixture.Create<Registratiedata.Vertegenwoordiger>();
        state.Vertegenwoordigers.Add(vertegenwoordiger);

        return new VertegenwoordigerWerdToegevoegd(
            vertegenwoordiger.VertegenwoordigerId,
            vertegenwoordiger.Insz,
            vertegenwoordiger.IsPrimair,
            vertegenwoordiger.Roepnaam,
            vertegenwoordiger.Rol,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            vertegenwoordiger.Email,
            vertegenwoordiger.Telefoon,
            vertegenwoordiger.Mobiel,
            vertegenwoordiger.SocialMedia);
    }

    private IEvent CreateVertegenwoordigerWijzigen(Fixture fixture, AssociationState state)
    {
        var existing = state.Vertegenwoordigers[_random.Next(state.Vertegenwoordigers.Count)];
        var updated = fixture.Create<Registratiedata.Vertegenwoordiger>() with
        {
            VertegenwoordigerId = existing.VertegenwoordigerId,
            Insz = existing.Insz // Keep INSZ constant
        };

        var index = state.Vertegenwoordigers.FindIndex(v => v.VertegenwoordigerId == existing.VertegenwoordigerId);
        state.Vertegenwoordigers[index] = updated;

        return new VertegenwoordigerWerdGewijzigd(
            updated.VertegenwoordigerId,
            updated.IsPrimair,
            updated.Roepnaam,
            updated.Rol,
            updated.Voornaam,
            updated.Achternaam,
            updated.Email,
            updated.Telefoon,
            updated.Mobiel,
            updated.SocialMedia);
    }

    private IEvent CreateVertegenwoordigerVerwijderen(AssociationState state)
    {
        var toRemove = state.Vertegenwoordigers[_random.Next(state.Vertegenwoordigers.Count)];
        state.Vertegenwoordigers.Remove(toRemove);

        return new VertegenwoordigerWerdVerwijderd(
            toRemove.VertegenwoordigerId,
            toRemove.Insz,
            toRemove.Voornaam,
            toRemove.Achternaam);
    }

    private IEvent CreateDoelgroepWijzigen(Fixture fixture, AssociationState state)
    {
        var doelgroep = fixture.Create<Registratiedata.Doelgroep>();
        state.Doelgroep = doelgroep;
        return new DoelgroepWerdGewijzigd(doelgroep);
    }

    private IEvent CreateStartdatumWijzigen(string vCode, Fixture fixture, AssociationState state)
    {
        var startdatum = fixture.Create<DateOnly?>();
        state.Startdatum = startdatum;
        return new StartdatumWerdGewijzigd(vCode, startdatum);
    }

    private IEvent CreateVerenigingVerwijderen(string vCode, AssociationState state)
    {
        state.IsDeleted = true;
        return new VerenigingWerdVerwijderd(vCode, "Random test deletion");
    }

    #endregion

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata() => Metadata;

    #region Supporting Types

    private enum EventType
    {
        NaamWijziging,
        ContactgegevenToevoegen,
        ContactgegevenWijzigen,
        ContactgegevenVerwijderen,
        LocatieToevoegen,
        LocatieWijzigen,
        LocatieVerwijderen,
        VertegenwoordigerToevoegen,
        VertegenwoordigerWijzigen,
        VertegenwoordigerVerwijderen,
        DoelgroepWijzigen,
        StartdatumWijzigen,
        VerenigingVerwijderen
    }

    private class AssociationState
    {
        public string VCode { get; }
        public string Naam { get; set; } = string.Empty;
        public DateOnly? Startdatum { get; set; }
        public Registratiedata.Doelgroep? Doelgroep { get; set; }
        public List<Registratiedata.Contactgegeven> Contactgegevens { get; } = new();
        public List<Registratiedata.Locatie> Locaties { get; } = new();
        public List<Registratiedata.Vertegenwoordiger> Vertegenwoordigers { get; } = new();
        public bool IsDeleted { get; set; }

        public AssociationState(string vCode)
        {
            VCode = vCode;
        }

        public void ApplyRegistration(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd @event)
        {
            Naam = @event.Naam;
            Startdatum = @event.Startdatum;
            Doelgroep = @event.Doelgroep;
            Contactgegevens.AddRange(@event.Contactgegevens);
            Locaties.AddRange(@event.Locaties);
            Vertegenwoordigers.AddRange(@event.Vertegenwoordigers);
        }
    }

    #endregion
}
