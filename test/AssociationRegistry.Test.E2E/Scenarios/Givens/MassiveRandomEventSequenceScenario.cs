// namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;
//
// using Events;
// using EventStore;
// using AssociationRegistry.Framework;
// using AssociationRegistry.Test.Common.AutoFixture;
// using Vereniging;
// using AutoFixture;
//
// public class MassiveRandomEventSequenceScenario : Framework.TestClasses.IScenario
// {
//     private CommandMetadata Metadata;
//     private readonly Random _random;
//     private readonly int _numberOfAssociations;
//     private readonly int _maxEventsPerAssociation;
//     private readonly EventDistribution _distribution;
//     private readonly Fixture _fixture;
//
//     public MassiveRandomEventSequenceScenario(
//         int numberOfAssociations = 1000,
//         int maxEventsPerAssociation = 200,
//         int? seed = null,
//         EventDistribution? distribution = null)
//     {
//         _numberOfAssociations = numberOfAssociations;
//         _maxEventsPerAssociation = maxEventsPerAssociation;
//         _random = seed.HasValue ? new Random(seed.Value) : new Random();
//         _distribution = distribution ?? EventDistribution.Default();
//         _fixture = new Fixture().CustomizeAdminApi();
//     }
//
//     public async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
//     {
//         Metadata = _fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
//
//         var result = new List<KeyValuePair<string, IEvent[]>>();
//         var totalEvents = 0;
//
//         Console.WriteLine($"Generating events for {_numberOfAssociations} associations...");
//
//         for (int i = 0; i < _numberOfAssociations; i++)
//         {
//             var vCode = $"V{i:D6}";
//             var events = GenerateEventSequenceForAssociation(vCode);
//             result.Add(new KeyValuePair<string, IEvent[]>(vCode, events.ToArray()));
//             totalEvents += events.Count;
//
//             if (i % 100 == 0)
//                 Console.WriteLine($"Generated {i} associations, {totalEvents} total events so far...");
//         }
//
//         Console.WriteLine($"Generated {totalEvents} total events across {_numberOfAssociations} associations");
//         return result.ToArray();
//     }
//
//     private List<IEvent> GenerateEventSequenceForAssociation(string vCode)
//     {
//         var events = new List<IEvent>();
//         var state = new AssociationState(vCode);
//
//         // Determine association type and initial registration
//         var associationType = DetermineAssociationType();
//         var registrationEvent = CreateRegistrationEvent(vCode, state, associationType);
//         events.Add(registrationEvent);
//
//         var numberOfEvents = _random.Next(5, _maxEventsPerAssociation);
//         var eventsSinceLastMajorChange = 0;
//
//         for (int i = 0; i < numberOfEvents; i++)
//         {
//             if (state.IsDeleted || state.IsStopped)
//                 break;
//
//             var nextEvent = GenerateNextValidEvent(vCode, state, eventsSinceLastMajorChange);
//             if (nextEvent != null)
//             {
//                 events.Add(nextEvent);
//
//                 if (IsLifecycleEvent(nextEvent))
//                     eventsSinceLastMajorChange = 0;
//                 else
//                     eventsSinceLastMajorChange++;
//             }
//         }
//
//         return events;
//     }
//
//     private AssociationType DetermineAssociationType()
//     {
//         var roll = _random.NextDouble();
//         return roll switch
//         {
//             < 0.7 => AssociationType.VerenigingZonderRechtspersoonlijkheid,
//             < 0.85 => AssociationType.FeitelijkeVereniging,
//             _ => AssociationType.VerenigingMetRechtspersoonlijkheid
//         };
//     }
//
//     private IEvent CreateRegistrationEvent(string vCode, AssociationState state, AssociationType type)
//     {
//         return type switch
//         {
//             AssociationType.VerenigingZonderRechtspersoonlijkheid => CreateVerenigingZonderRechtspersoonlijkheidRegistration(vCode, state),
//             AssociationType.FeitelijkeVereniging => CreateFeitelijkeVerenigingRegistration(vCode, state),
//             AssociationType.VerenigingMetRechtspersoonlijkheid => CreateVerenigingMetRechtspersoonlijkheidRegistration(vCode, state),
//             _ => throw new ArgumentOutOfRangeException()
//         };
//     }
//
//     private IEvent CreateVerenigingZonderRechtspersoonlijkheidRegistration(string vCode, AssociationState state)
//     {
//         var registrationEvent = _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = vCode };
//         state.ApplyRegistration(registrationEvent);
//         return registrationEvent;
//     }
//
//     private IEvent CreateFeitelijkeVerenigingRegistration(string vCode, AssociationState state)
//     {
//         var registrationEvent = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with { VCode = vCode };
//         state.ApplyFeitelijkeVerenigingRegistration(registrationEvent);
//         return registrationEvent;
//     }
//
//     private IEvent CreateVerenigingMetRechtspersoonlijkheidRegistration(string vCode, AssociationState state)
//     {
//         var registrationEvent = _fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with { VCode = vCode };
//         state.ApplyKboRegistration(registrationEvent);
//         return registrationEvent;
//     }
//
//     private IEvent? GenerateNextValidEvent(string vCode, AssociationState state, int eventsSinceLastMajorChange)
//     {
//         var availableEventTypes = GetAvailableEventTypes(state, eventsSinceLastMajorChange);
//         if (!availableEventTypes.Any())
//             return null;
//
//         var eventType = SelectEventTypeByProbability(availableEventTypes);
//
//         return eventType switch
//         {
//             // Basic modification events
//             EventType.NaamWijziging => new NaamWerdGewijzigd(vCode, _fixture.Create<string>()),
//             EventType.KorteNaamWijziging => new KorteNaamWerdGewijzigd(vCode, _fixture.Create<string>()),
//             EventType.KorteBeschrijvingWijziging => new KorteBeschrijvingWerdGewijzigd(vCode, _fixture.Create<string>()),
//             EventType.RoepnaamWijziging => new RoepnaamWerdGewijzigd(_fixture.Create<string>()),
//             EventType.DoelgroepWijziging => new DoelgroepWerdGewijzigd(_fixture.Create<Registratiedata.Doelgroep>()),
//             EventType.StartdatumWijziging => new StartdatumWerdGewijzigd(vCode, _fixture.Create<DateOnly?>()),
//             EventType.EinddatumWijziging => new EinddatumWerdGewijzigd(_fixture.Create<DateOnly>()),
//
//             // Contact events
//             EventType.ContactgegevenToevoegen => CreateContactgegevenToevoegen(state),
//             EventType.ContactgegevenWijzigen => CreateContactgegevenWijzigen(state),
//             EventType.ContactgegevenVerwijderen => CreateContactgegevenVerwijderen(state),
//
//             // Location events
//             EventType.LocatieToevoegen => CreateLocatieToevoegen(state),
//             EventType.LocatieWijzigen => CreateLocatieWijzigen(state),
//             EventType.LocatieVerwijderen => CreateLocatieVerwijderen(vCode, state),
//
//             // Representative events
//             EventType.VertegenwoordigerToevoegen => CreateVertegenwoordigerToevoegen(state),
//             EventType.VertegenwoordigerWijzigen => CreateVertegenwoordigerWijzigen(state),
//             EventType.VertegenwoordigerVerwijderen => CreateVertegenwoordigerVerwijderen(state),
//
//             // Activity events
//             EventType.HoofdactiviteitenWijzigen => new HoofdactiviteitenVerenigingsloketWerdenGewijzigd(_fixture.CreateMany<Registratiedata.HoofdactiviteitVerenigingsloket>().ToArray()),
//             EventType.WerkingsgebiedenWijzigen => CreateWerkingsgebiedenWijzigen(vCode, state),
//             EventType.WerkingsgebiedenBepalen => CreateWerkingsgebiedenBepalen(vCode, state),
//             EventType.WerkingsgebiedenNietVanToepassing => new WerkingsgebiedenWerdenNietVanToepassing(vCode),
//
//             // Address registry events
//             EventType.AdresOvernemenUitAdressenregister => CreateAdresOvernemenUitAdressenregister(vCode, state),
//             EventType.AdresWijzigenInAdressenregister => CreateAdresWijzigenInAdressenregister(vCode, state),
//             EventType.AdresOntkoppelenVanAdressenregister => CreateAdresOntkoppelenVanAdressenregister(vCode, state),
//             EventType.AdresNietGevondenInAdressenregister => CreateAdresNietGevondenInAdressenregister(vCode, state),
//
//             // KBO events (only for KBO associations)
//             EventType.KboSynchronisatieSuccesvol => state.HasKboNumber ? new SynchronisatieMetKboWasSuccesvol(state.KboNummer!) : null,
//             EventType.NaamWijzigenInKbo => state.HasKboNumber ? new NaamWerdGewijzigdInKbo(_fixture.Create<string>()) : null,
//             EventType.KorteNaamWijzigenInKbo => state.HasKboNumber ? new KorteNaamWerdGewijzigdInKbo(_fixture.Create<string>()) : null,
//             EventType.StartdatumWijzigenInKbo => state.HasKboNumber ? new StartdatumWerdGewijzigdInKbo(_fixture.Create<DateOnly?>()) : null,
//             EventType.ContactgegevenOvernemenUitKbo => CreateContactgegevenOvernemenUitKbo(state),
//             EventType.ContactgegevenWijzigenInKbo => CreateContactgegevenWijzigenInKbo(state),
//             EventType.ContactgegevenVerwijderenUitKbo => CreateContactgegevenVerwijderenUitKbo(state),
//             EventType.MaatschappelijkeZetelOvernemenUitKbo => CreateMaatschappelijkeZetelOvernemenUitKbo(state),
//             EventType.MaatschappelijkeZetelWijzigenInKbo => CreateMaatschappelijkeZetelWijzigenInKbo(state),
//             EventType.VertegenwoordigerOvernemenUitKbo => CreateVertegenwoordigerOvernemenUitKbo(state),
//             EventType.RechtsvormWijzigenInKbo => state.HasKboNumber ? _fixture.Create<RechtsvormWerdGewijzigdInKBO>() : null,
//
//             // Membership events
//             EventType.LidmaatschapToevoegen => CreateLidmaatschapToevoegen(vCode, state),
//             EventType.LidmaatschapWijzigen => CreateLidmaatschapWijzigen(vCode, state),
//             EventType.LidmaatschapVerwijderen => CreateLidmaatschapVerwijderen(vCode, state),
//
//             // Association type changes
//             EventType.VerenigingssubtypeVerfijnenNaarFeitelijkeVereniging => CreateVerenigingssubtypeVerfijning(vCode, state),
//             EventType.VerenigingssubtypeVerfijnenNaarSubvereniging => CreateSubverenigingVerfijning(vCode, state),
//             EventType.FeitelijkeVerenigingMigrerenNaarVerenigingZonderRechtspersoonlijkheid => CreateMigratieNaarVerenigingZonderRechtspersoonlijkheid(vCode, state),
//
//             // Publication events
//             EventType.InschrijvenInPubliekeDatastroom => CreateInschrijvenInPubliekeDatastroom(state),
//             EventType.UitschrijvenUitPubliekeDatastroom => CreateUitschrijvenUitPubliekeDatastroom(state),
//
//             // Duplicate handling
//             EventType.MarkerenAlsDubbeleVereniging => CreateMarkerenAlsDubbeleVereniging(vCode, state),
//             EventType.CorrigeerDubbeleVereniging => CreateCorrigeerDubbeleVereniging(vCode, state),
//
//             // Lifecycle end events
//             EventType.VerenigingStoppen => CreateVerenigingStoppen(state),
//             EventType.VerenigingStoppenInKbo => CreateVerenigingStoppenInKbo(state),
//             EventType.VerenigingVerwijderen => CreateVerenigingVerwijderen(vCode, state),
//
//             _ => null
//         };
//     }
//
//     private List<EventType> GetAvailableEventTypes(AssociationState state, int eventsSinceLastMajorChange)
//     {
//         var available = new List<EventType>();
//
//         // Basic modification events (always available)
//         available.AddRange(new[]
//         {
//             EventType.NaamWijziging,
//             EventType.KorteNaamWijziging,
//             EventType.KorteBeschrijvingWijziging,
//             EventType.DoelgroepWijziging,
//             EventType.StartdatumWijziging,
//             EventType.ContactgegevenToevoegen,
//             EventType.LocatieToevoegen,
//             EventType.VertegenwoordigerToevoegen,
//             EventType.HoofdactiviteitenWijzigen
//         });
//
//         // Conditional events based on existing data
//         if (state.Contactgegevens.Any())
//         {
//             available.AddRange(new[] { EventType.ContactgegevenWijzigen, EventType.ContactgegevenVerwijderen });
//         }
//
//         if (state.Locaties.Any())
//         {
//             available.AddRange(new[] { EventType.LocatieWijzigen, EventType.LocatieVerwijderen });
//
//             // Address registry events for locations
//             available.AddRange(new[]
//             {
//                 EventType.AdresOvernemenUitAdressenregister,
//                 EventType.AdresNietGevondenInAdressenregister
//             });
//
//             if (state.Locaties.Any(l => l.AdresId != null))
//             {
//                 available.AddRange(new[]
//                 {
//                     EventType.AdresWijzigenInAdressenregister,
//                     EventType.AdresOntkoppelenVanAdressenregister
//                 });
//             }
//         }
//
//         if (state.Vertegenwoordigers.Any())
//         {
//             available.AddRange(new[] { EventType.VertegenwoordigerWijzigen, EventType.VertegenwoordigerVerwijderen });
//         }
//
//         if (state.Lidmaatschappen.Any())
//         {
//             available.AddRange(new[] { EventType.LidmaatschapWijzigen, EventType.LidmaatschapVerwijderen });
//         }
//
//         // KBO-specific events
//         if (state.HasKboNumber)
//         {
//             available.AddRange(new[]
//             {
//                 EventType.KboSynchronisatieSuccesvol,
//                 EventType.NaamWijzigenInKbo,
//                 EventType.KorteNaamWijzigenInKbo,
//                 EventType.StartdatumWijzigenInKbo,
//                 EventType.ContactgegevenOvernemenUitKbo,
//                 EventType.MaatschappelijkeZetelOvernemenUitKbo,
//                 EventType.VertegenwoordigerOvernemenUitKbo,
//                 EventType.RechtsvormWijzigenInKbo
//             });
//
//             if (state.ContactgegevensFromKbo.Any())
//             {
//                 available.AddRange(new[]
//                 {
//                     EventType.ContactgegevenWijzigenInKbo,
//                     EventType.ContactgegevenVerwijderenUitKbo
//                 });
//             }
//
//             if (state.MaatschappelijkeZetelFromKbo != null)
//             {
//                 available.Add(EventType.MaatschappelijkeZetelWijzigenInKbo);
//             }
//         }
//
//         // Workingsgebieden events
//         available.AddRange(new[]
//         {
//             EventType.WerkingsgebiedenWijzigen,
//             EventType.WerkingsgebiedenBepalen,
//             EventType.WerkingsgebiedenNietVanToepassing
//         });
//
//         // Membership events
//         available.Add(EventType.LidmaatschapToevoegen);
//
//         // Association type refinements (only early in lifecycle)
//         if (eventsSinceLastMajorChange < 10 && state.AssociationType == AssociationType.VerenigingZonderRechtspersoonlijkheid)
//         {
//             available.AddRange(new[]
//             {
//                 EventType.VerenigingssubtypeVerfijnenNaarFeitelijkeVereniging,
//                 EventType.VerenigingssubtypeVerfijnenNaarSubvereniging
//             });
//         }
//
//         if (state.AssociationType == AssociationType.FeitelijkeVereniging && eventsSinceLastMajorChange < 5)
//         {
//             available.Add(EventType.FeitelijkeVerenigingMigrerenNaarVerenigingZonderRechtspersoonlijkheid);
//         }
//
//         // Publication events
//         if (!state.IsInPubliekeDatastroom)
//             available.Add(EventType.InschrijvenInPubliekeDatastroom);
//         else
//             available.Add(EventType.UitschrijvenUitPubliekeDatastroom);
//
//         // Duplicate handling (rare)
//         if (_random.NextDouble() < 0.02)
//         {
//             available.AddRange(new[]
//             {
//                 EventType.MarkerenAlsDubbeleVereniging,
//                 EventType.CorrigeerDubbeleVereniging
//             });
//         }
//
//         // Lifecycle end events (very rare, but increase probability over time)
//         var endProbability = Math.Min(0.1, eventsSinceLastMajorChange * 0.002);
//         if (_random.NextDouble() < endProbability)
//         {
//             if (state.HasKboNumber && _random.NextDouble() < 0.3)
//                 available.Add(EventType.VerenigingStoppenInKbo);
//             else
//                 available.Add(EventType.VerenigingStoppen);
//
//             if (_random.NextDouble() < 0.1)
//                 available.Add(EventType.VerenigingVerwijderen);
//         }
//
//         return available;
//     }
//
//     private EventType SelectEventTypeByProbability(List<EventType> availableEventTypes)
//     {
//         var weights = availableEventTypes.Select(et => _distribution.GetWeight(et)).ToArray();
//         var totalWeight = weights.Sum();
//         var randomValue = _random.NextDouble() * totalWeight;
//
//         var cumulativeWeight = 0.0;
//         for (int i = 0; i < availableEventTypes.Count; i++)
//         {
//             cumulativeWeight += weights[i];
//             if (randomValue <= cumulativeWeight)
//                 return availableEventTypes[i];
//         }
//
//         return availableEventTypes.Last();
//     }
//
//     #region Event Creation Methods
//
//     private IEvent CreateContactgegevenToevoegen(AssociationState state)
//     {
//         var contactgegeven = _fixture.Create<Registratiedata.Contactgegeven>() with { ContactgegevenId = state.GetNextContactgegevenId() };
//         state.Contactgegevens.Add(contactgegeven);
//
//         return new ContactgegevenWerdToegevoegd(
//             contactgegeven.ContactgegevenId,
//             contactgegeven.Contactgegeventype,
//             contactgegeven.Waarde,
//             contactgegeven.Beschrijving,
//             contactgegeven.IsPrimair);
//     }
//
//     private IEvent? CreateContactgegevenWijzigen(AssociationState state)
//     {
//         if (!state.Contactgegevens.Any()) return null;
//
//         var existing = state.Contactgegevens[_random.Next(state.Contactgegevens.Count)];
//         var updated = _fixture.Create<Registratiedata.Contactgegeven>() with
//         {
//             ContactgegevenId = existing.ContactgegevenId,
//             Contactgegeventype = existing.Contactgegeventype
//         };
//
//         var index = state.Contactgegevens.FindIndex(c => c.ContactgegevenId == existing.ContactgegevenId);
//         state.Contactgegevens[index] = updated;
//
//         return new ContactgegevenWerdGewijzigd(
//             updated.ContactgegevenId,
//             updated.Contactgegeventype,
//             updated.Waarde,
//             updated.Beschrijving,
//             updated.IsPrimair);
//     }
//
//     private IEvent? CreateContactgegevenVerwijderen(AssociationState state)
//     {
//         if (!state.Contactgegevens.Any()) return null;
//
//         var toRemove = state.Contactgegevens[_random.Next(state.Contactgegevens.Count)];
//         state.Contactgegevens.Remove(toRemove);
//
//         return new ContactgegevenWerdVerwijderd(
//             toRemove.ContactgegevenId,
//             toRemove.Contactgegeventype,
//             toRemove.Waarde,
//             toRemove.Beschrijving,
//             toRemove.IsPrimair);
//     }
//
//     private IEvent CreateLocatieToevoegen(AssociationState state)
//     {
//         var locatie = _fixture.Create<Registratiedata.Locatie>() with { LocatieId = state.GetNextLocatieId() };
//         state.Locaties.Add(locatie);
//
//         return new LocatieWerdToegevoegd(locatie);
//     }
//
//     private IEvent? CreateLocatieWijzigen(AssociationState state)
//     {
//         if (!state.Locaties.Any()) return null;
//
//         var existing = state.Locaties[_random.Next(state.Locaties.Count)];
//         var updated = _fixture.Create<Registratiedata.Locatie>() with { LocatieId = existing.LocatieId };
//
//         var index = state.Locaties.FindIndex(l => l.LocatieId == existing.LocatieId);
//         state.Locaties[index] = updated;
//
//         return new LocatieWerdGewijzigd(updated);
//     }
//
//     private IEvent? CreateLocatieVerwijderen(string vCode, AssociationState state)
//     {
//         if (!state.Locaties.Any()) return null;
//
//         var toRemove = state.Locaties[_random.Next(state.Locaties.Count)];
//         state.Locaties.Remove(toRemove);
//
//         return new LocatieWerdVerwijderd(vCode, toRemove);
//     }
//
//     private IEvent CreateVertegenwoordigerToevoegen(AssociationState state)
//     {
//         var vertegenwoordiger = _fixture.Create<Registratiedata.Vertegenwoordiger>() with { VertegenwoordigerId = state.GetNextVertegenwoordigerId() };
//         state.Vertegenwoordigers.Add(vertegenwoordiger);
//
//         return new VertegenwoordigerWerdToegevoegd(
//             vertegenwoordiger.VertegenwoordigerId,
//             vertegenwoordiger.Insz,
//             vertegenwoordiger.IsPrimair,
//             vertegenwoordiger.Roepnaam,
//             vertegenwoordiger.Rol,
//             vertegenwoordiger.Voornaam,
//             vertegenwoordiger.Achternaam,
//             vertegenwoordiger.Email,
//             vertegenwoordiger.Telefoon,
//             vertegenwoordiger.Mobiel,
//             vertegenwoordiger.SocialMedia);
//     }
//
//     private IEvent? CreateVertegenwoordigerWijzigen(AssociationState state)
//     {
//         if (!state.Vertegenwoordigers.Any()) return null;
//
//         var existing = state.Vertegenwoordigers[_random.Next(state.Vertegenwoordigers.Count)];
//         var updated = _fixture.Create<Registratiedata.Vertegenwoordiger>() with
//         {
//             VertegenwoordigerId = existing.VertegenwoordigerId,
//             Insz = existing.Insz
//         };
//
//         var index = state.Vertegenwoordigers.FindIndex(v => v.VertegenwoordigerId == existing.VertegenwoordigerId);
//         state.Vertegenwoordigers[index] = updated;
//
//         return new VertegenwoordigerWerdGewijzigd(
//             updated.VertegenwoordigerId,
//             updated.IsPrimair,
//             updated.Roepnaam,
//             updated.Rol,
//             updated.Voornaam,
//             updated.Achternaam,
//             updated.Email,
//             updated.Telefoon,
//             updated.Mobiel,
//             updated.SocialMedia);
//     }
//
//     private IEvent? CreateVertegenwoordigerVerwijderen(AssociationState state)
//     {
//         if (!state.Vertegenwoordigers.Any()) return null;
//
//         var toRemove = state.Vertegenwoordigers[_random.Next(state.Vertegenwoordigers.Count)];
//         state.Vertegenwoordigers.Remove(toRemove);
//
//         return new VertegenwoordigerWerdVerwijderd(
//             toRemove.VertegenwoordigerId,
//             toRemove.Insz,
//             toRemove.Voornaam,
//             toRemove.Achternaam);
//     }
//
//     private IEvent CreateWerkingsgebiedenWijzigen(string vCode, AssociationState state)
//     {
//         var werkingsgebieden = _fixture.CreateMany<Registratiedata.Werkingsgebied>().ToArray();
//         state.Werkingsgebieden = werkingsgebieden;
//         return new WerkingsgebiedenWerdenGewijzigd(vCode, werkingsgebieden);
//     }
//
//     private IEvent CreateWerkingsgebiedenBepalen(string vCode, AssociationState state)
//     {
//         var werkingsgebieden = _fixture.CreateMany<Registratiedata.Werkingsgebied>().ToArray();
//         state.Werkingsgebieden = werkingsgebieden;
//         return new WerkingsgebiedenWerdenBepaald(vCode, werkingsgebieden);
//     }
//
//     private IEvent? CreateAdresOvernemenUitAdressenregister(string vCode, AssociationState state)
//     {
//         if (!state.Locaties.Any()) return null;
//
//         var locatie = state.Locaties[_random.Next(state.Locaties.Count)];
//         var adresId = _fixture.Create<Registratiedata.AdresId>();
//         var adres = _fixture.Create<Registratiedata.AdresUitAdressenregister>();
//
//         var index = state.Locaties.FindIndex(l => l.LocatieId == locatie.LocatieId);
//         state.Locaties[index] = locatie with { AdresId = adresId };
//
//         return new AdresWerdOvergenomenUitAdressenregister(vCode, locatie.LocatieId, adresId, adres);
//     }
//
//     private IEvent? CreateAdresWijzigenInAdressenregister(string vCode, AssociationState state)
//     {
//         var locatieMetAdres = state.Locaties.FirstOrDefault(l => l.AdresId != null);
//         if (locatieMetAdres == null) return null;
//
//         var adres = _fixture.Create<Registratiedata.AdresUitAdressenregister>();
//         var idempotenceKey = _fixture.Create<string>();
//
//         return new AdresWerdGewijzigdInAdressenregister(vCode, locatieMetAdres.LocatieId, locatieMetAdres.AdresId, adres, idempotenceKey);
//     }
//
//     private IEvent? CreateAdresOntkoppelenVanAdressenregister(string vCode, AssociationState state)
//     {
//         var locatieMetAdres = state.Locaties.FirstOrDefault(l => l.AdresId != null);
//         if (locatieMetAdres == null) return null;
//
//         var index = state.Locaties.FindIndex(l => l.LocatieId == locatieMetAdres.LocatieId);
//         state.Locaties[index] = locatieMetAdres with { AdresId = null };
//
//         return new AdresWerdOntkoppeldVanAdressenregister(vCode, locatieMetAdres.LocatieId, locatieMetAdres.AdresId, locatieMetAdres.Adres);
//     }
//
//     private IEvent? CreateAdresNietGevondenInAdressenregister(string vCode, AssociationState state)
//     {
//         if (!state.Locaties.Any()) return null;
//
//         var locatie = state.Locaties[_random.Next(state.Locaties.Count)];
//         var adres = locatie.Adres ?? _fixture.Create<Registratiedata.Adres>();
//
//         return new AdresWerdNietGevondenInAdressenregister(
//             vCode,
//             locatie.LocatieId,
//             adres.Straatnaam,
//             adres.Huisnummer,
//             adres.Busnummer,
//             adres.Postcode,
//             adres.Gemeente);
//     }
//
//     private IEvent? CreateContactgegevenOvernemenUitKbo(AssociationState state)
//     {
//         if (!state.HasKboNumber) return null;
//
//         var contactgegeven = _fixture.Create<Registratiedata.Contactgegeven>() with { ContactgegevenId = state.GetNextContactgegevenId() };
//         var typeVolgensKbo = contactgegeven.Contactgegeventype + "_KBO";
//
//         state.ContactgegevensFromKbo.Add(contactgegeven);
//
//         return new ContactgegevenWerdOvergenomenUitKBO(
//             contactgegeven.ContactgegevenId,
//             contactgegeven.Contactgegeventype,
//             typeVolgensKbo,
//             contactgegeven.Waarde);
//     }
//
//     private IEvent? CreateContactgegevenWijzigenInKbo(AssociationState state)
//     {
//         if (!state.ContactgegevensFromKbo.Any()) return null;
//
//         var existing = state.ContactgegevensFromKbo[_random.Next(state.ContactgegevensFromKbo.Count)];
//         var typeVolgensKbo = existing.Contactgegeventype + "_KBO";
//         var nieuweWaarde = _fixture.Create<string>();
//
//         return new ContactgegevenWerdGewijzigdInKbo(
//             existing.ContactgegevenId,
//             existing.Contactgegeventype,
//             typeVolgensKbo,
//             nieuweWaarde);
//     }
//
//     private IEvent? CreateContactgegevenVerwijderenUitKbo(AssociationState state)
//     {
//         if (!state.ContactgegevensFromKbo.Any()) return null;
//
//         var toRemove = state.ContactgegevensFromKbo[_random.Next(state.ContactgegevensFromKbo.Count)];
//         state.ContactgegevensFromKbo.Remove(toRemove);
//         var typeVolgensKbo = toRemove.Contactgegeventype + "_KBO";
//
//         return new ContactgegevenWerdVerwijderdUitKBO(
//             toRemove.ContactgegevenId,
//             toRemove.Contactgegeventype,
//             typeVolgensKbo,
//             toRemove.Waarde);
//     }
//
//     private IEvent? CreateMaatschappelijkeZetelOvernemenUitKbo(AssociationState state)
//     {
//         if (!state.HasKboNumber) return null;
//
//         var locatie = _fixture.Create<Registratiedata.Locatie>() with
//         {
//             LocatieId = state.GetNextLocatieId(),
//             Locatietype = "Maatschappelijke zetel"
//         };
//         state.MaatschappelijkeZetelFromKbo = locatie;
//         state.Locaties.Add(locatie);
//
//         return new MaatschappelijkeZetelWerdOvergenomenUitKbo(locatie);
//     }
//
//     private IEvent? CreateMaatschappelijkeZetelWijzigenInKbo(AssociationState state)
//     {
//         if (state.MaatschappelijkeZetelFromKbo == null) return null;
//
//         var updated = _fixture.Create<Registratiedata.Locatie>() with
//         {
//             LocatieId = state.MaatschappelijkeZetelFromKbo.LocatieId,
//             Locatietype = "Maatschappelijke zetel"
//         };
//
//         var index = state.Locaties.FindIndex(l => l.LocatieId == state.MaatschappelijkeZetelFromKbo.LocatieId);
//         if (index >= 0) state.Locaties[index] = updated;
//         state.MaatschappelijkeZetelFromKbo = updated;
//
//         return new MaatschappelijkeZetelWerdGewijzigdInKbo(updated);
//     }
//
//     private IEvent? CreateVertegenwoordigerOvernemenUitKbo(AssociationState state)
//     {
//         if (!state.HasKboNumber) return null;
//
//         var vertegenwoordiger = _fixture.Create<Registratiedata.Vertegenwoordiger>() with { VertegenwoordigerId = state.GetNextVertegenwoordigerId() };
//         state.VertegenwoordigersFromKbo.Add(vertegenwoordiger);
//
//         return new VertegenwoordigerWerdOvergenomenUitKBO(
//             vertegenwoordiger.VertegenwoordigerId,
//             vertegenwoordiger.Insz,
//             vertegenwoordiger.Voornaam,
//             vertegenwoordiger.Achternaam);
//     }
//
//     private IEvent CreateLidmaatschapToevoegen(string vCode, AssociationState state)
//     {
//         var lidmaatschap = _fixture.Create<Registratiedata.Lidmaatschap>() with { LidmaatschapId = state.GetNextLidmaatschapId() };
//         state.Lidmaatschappen.Add(lidmaatschap);
//
//         return new LidmaatschapWerdToegevoegd(vCode, lidmaatschap);
//     }
//
//     private IEvent? CreateLidmaatschapWijzigen(string vCode, AssociationState state)
//     {
//         if (!state.Lidmaatschappen.Any()) return null;
//
//         var existing = state.Lidmaatschappen[_random.Next(state.Lidmaatschappen.Count)];
//         var updated = _fixture.Create<Registratiedata.Lidmaatschap>() with
//         {
//             LidmaatschapId = existing.LidmaatschapId,
//             AndereVereniging = existing.AndereVereniging
//         };
//
//         var index = state.Lidmaatschappen.FindIndex(l => l.LidmaatschapId == existing.LidmaatschapId);
//         state.Lidmaatschappen[index] = updated;
//
//         return new LidmaatschapWerdGewijzigd(vCode, updated);
//     }
//
//     private IEvent? CreateLidmaatschapVerwijderen(string vCode, AssociationState state)
//     {
//         if (!state.Lidmaatschappen.Any()) return null;
//
//         var toRemove = state.Lidmaatschappen[_random.Next(state.Lidmaatschappen.Count)];
//         state.Lidmaatschappen.Remove(toRemove);
//
//         return new LidmaatschapWerdVerwijderd(vCode, toRemove);
//     }
//
//     private IEvent CreateVerenigingssubtypeVerfijning(string vCode, AssociationState state)
//     {
//         state.AssociationType = AssociationType.FeitelijkeVereniging;
//         return new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(vCode);
//     }
//
//     private IEvent CreateSubverenigingVerfijning(string vCode, AssociationState state)
//     {
//         var moedervereniging = _fixture.Create<Registratiedata.SubverenigingVan>();
//         state.AssociationType = AssociationType.Subvereniging;
//         state.SubverenigingVan = moedervereniging;
//
//         return new VerenigingssubtypeWerdVerfijndNaarSubvereniging(vCode, moedervereniging);
//     }
//
//     private IEvent CreateMigratieNaarVerenigingZonderRechtspersoonlijkheid(string vCode, AssociationState state)
//     {
//         state.AssociationType = AssociationType.VerenigingZonderRechtspersoonlijkheid;
//         return new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(vCode);
//     }
//
//     private IEvent CreateInschrijvenInPubliekeDatastroom(AssociationState state)
//     {
//         state.IsInPubliekeDatastroom = true;
//         return new VerenigingWerdIngeschrevenInPubliekeDatastroom();
//     }
//
//     private IEvent CreateUitschrijvenUitPubliekeDatastroom(AssociationState state)
//     {
//         state.IsInPubliekeDatastroom = false;
//         return new VerenigingWerdUitgeschrevenUitPubliekeDatastroom();
//     }
//
//     private IEvent CreateMarkerenAlsDubbeleVereniging(string vCode, AssociationState state)
//     {
//         var authentiekeVereniging = _fixture.Create<VCode>().ToString();
//         state.IsMarkedAsDuplicate = true;
//         state.AuthentiekeVereniging = authentiekeVereniging;
//
//         return new VerenigingWerdGemarkeerdAlsDubbelVan(vCode, authentiekeVereniging);
//     }
//
//     private IEvent? CreateCorrigeerDubbeleVereniging(string vCode, AssociationState state)
//     {
//         if (!state.IsMarkedAsDuplicate) return null;
//
//         var authentiekeVereniging = state.AuthentiekeVereniging ?? _fixture.Create<VCode>().ToString();
//         var vorigeStatus = "Dubbel";
//         state.IsMarkedAsDuplicate = false;
//         state.AuthentiekeVereniging = null;
//
//         return new MarkeringDubbeleVerengingWerdGecorrigeerd(vCode, authentiekeVereniging, vorigeStatus);
//     }
//
//     private IEvent CreateVerenigingStoppen(AssociationState state)
//     {
//         var einddatum = _fixture.Create<DateOnly>();
//         state.IsStopped = true;
//         state.Einddatum = einddatum;
//
//         return new VerenigingWerdGestopt(einddatum);
//     }
//
//     private IEvent? CreateVerenigingStoppenInKbo(AssociationState state)
//     {
//         if (!state.HasKboNumber) return null;
//
//         var einddatum = _fixture.Create<DateOnly>();
//         state.IsStopped = true;
//         state.Einddatum = einddatum;
//
//         return new VerenigingWerdGestoptInKBO(einddatum);
//     }
//
//     private IEvent CreateVerenigingVerwijderen(string vCode, AssociationState state)
//     {
//         state.IsDeleted = true;
//         var redenen = new[] { "Duplicate", "Invalid registration", "Administrative cleanup", "Merged with other association" };
//         var reden = redenen[_random.Next(redenen.Length)];
//
//         return new VerenigingWerdVerwijderd(vCode, reden);
//     }
//
//     #endregion
//
//     #region Helper Methods
//
//     private bool IsLifecycleEvent(IEvent @event)
//     {
//         return @event switch
//         {
//             VerenigingWerdGestopt => true,
//             VerenigingWerdGestoptInKBO => true,
//             VerenigingWerdVerwijderd => true,
//             VerenigingWerdGemarkeerdAlsDubbelVan => true,
//             FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid => true,
//             VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging => true,
//             VerenigingssubtypeWerdVerfijndNaarSubvereniging => true,
//             _ => false
//         };
//     }
//
//     #endregion
//
//     public StreamActionResult Result { get; set; } = null!;
//
//     public CommandMetadata GetCommandMetadata() => Metadata;
//
//     #region Supporting Types
//
//     private enum AssociationType
//     {
//         VerenigingZonderRechtspersoonlijkheid,
//         FeitelijkeVereniging,
//         VerenigingMetRechtspersoonlijkheid,
//         Subvereniging
//     }
//
//     private enum EventType
//     {
//         // Basic modifications
//         NaamWijziging,
//         KorteNaamWijziging,
//         KorteBeschrijvingWijziging,
//         RoepnaamWijziging,
//         DoelgroepWijziging,
//         StartdatumWijziging,
//         EinddatumWijziging,
//
//         // Contact management
//         ContactgegevenToevoegen,
//         ContactgegevenWijzigen,
//         ContactgegevenVerwijderen,
//
//         // Location management
//         LocatieToevoegen,
//         LocatieWijzigen,
//         LocatieVerwijderen,
//
//         // Representative management
//         VertegenwoordigerToevoegen,
//         VertegenwoordigerWijzigen,
//         VertegenwoordigerVerwijderen,
//
//         // Activities and areas
//         HoofdactiviteitenWijzigen,
//         WerkingsgebiedenWijzigen,
//         WerkingsgebiedenBepalen,
//         WerkingsgebiedenNietVanToepassing,
//
//         // Address registry
//         AdresOvernemenUitAdressenregister,
//         AdresWijzigenInAdressenregister,
//         AdresOntkoppelenVanAdressenregister,
//         AdresNietGevondenInAdressenregister,
//
//         // KBO integration
//         KboSynchronisatieSuccesvol,
//         NaamWijzigenInKbo,
//         KorteNaamWijzigenInKbo,
//         StartdatumWijzigenInKbo,
//         ContactgegevenOvernemenUitKbo,
//         ContactgegevenWijzigenInKbo,
//         ContactgegevenVerwijderenUitKbo,
//         MaatschappelijkeZetelOvernemenUitKbo,
//         MaatschappelijkeZetelWijzigenInKbo,
//         VertegenwoordigerOvernemenUitKbo,
//         RechtsvormWijzigenInKbo,
//
//         // Membership
//         LidmaatschapToevoegen,
//         LidmaatschapWijzigen,
//         LidmaatschapVerwijderen,
//
//         // Association type changes
//         VerenigingssubtypeVerfijnenNaarFeitelijkeVereniging,
//         VerenigingssubtypeVerfijnenNaarSubvereniging,
//         FeitelijkeVerenigingMigrerenNaarVerenigingZonderRechtspersoonlijkheid,
//
//         // Publication
//         InschrijvenInPubliekeDatastroom,
//         UitschrijvenUitPubliekeDatastroom,
//
//         // Duplicate handling
//         MarkerenAlsDubbeleVereniging,
//         CorrigeerDubbeleVereniging,
//
//         // Lifecycle end
//         VerenigingStoppen,
//         VerenigingStoppenInKbo,
//         VerenigingVerwijderen
//     }
//
//     public class EventDistribution
//     {
//         private readonly Dictionary<EventType, double> _weights;
//
//         private EventDistribution(Dictionary<EventType, double> weights)
//         {
//             _weights = weights;
//         }
//
//         public static EventDistribution Default()
//         {
//             return new EventDistribution(new Dictionary<EventType, double>
//             {
//                 // High frequency events (common modifications)
//                 [EventType.NaamWijziging] = 0.8,
//                 [EventType.ContactgegevenToevoegen] = 1.0,
//                 [EventType.ContactgegevenWijzigen] = 1.2,
//                 [EventType.LocatieToevoegen] = 0.9,
//                 [EventType.LocatieWijzigen] = 1.1,
//                 [EventType.VertegenwoordigerToevoegen] = 0.8,
//                 [EventType.VertegenwoordigerWijzigen] = 1.0,
//                 [EventType.HoofdactiviteitenWijzigen] = 0.7,
//
//                 // Medium frequency events
//                 [EventType.KorteNaamWijziging] = 0.5,
//                 [EventType.KorteBeschrijvingWijziging] = 0.6,
//                 [EventType.DoelgroepWijziging] = 0.4,
//                 [EventType.StartdatumWijziging] = 0.3,
//                 [EventType.ContactgegevenVerwijderen] = 0.6,
//                 [EventType.LocatieVerwijderen] = 0.5,
//                 [EventType.VertegenwoordigerVerwijderen] = 0.4,
//                 [EventType.WerkingsgebiedenWijzigen] = 0.5,
//                 [EventType.WerkingsgebiedenBepalen] = 0.3,
//                 [EventType.LidmaatschapToevoegen] = 0.4,
//                 [EventType.LidmaatschapWijzigen] = 0.3,
//
//                 // Address registry events
//                 [EventType.AdresOvernemenUitAdressenregister] = 0.3,
//                 [EventType.AdresWijzigenInAdressenregister] = 0.2,
//                 [EventType.AdresOntkoppelenVanAdressenregister] = 0.1,
//                 [EventType.AdresNietGevondenInAdressenregister] = 0.2,
//
//                 // KBO integration events (when applicable)
//                 [EventType.KboSynchronisatieSuccesvol] = 0.4,
//                 [EventType.NaamWijzigenInKbo] = 0.3,
//                 [EventType.KorteNaamWijzigenInKbo] = 0.2,
//                 [EventType.StartdatumWijzigenInKbo] = 0.1,
//                 [EventType.ContactgegevenOvernemenUitKbo] = 0.3,
//                 [EventType.ContactgegevenWijzigenInKbo] = 0.2,
//                 [EventType.ContactgegevenVerwijderenUitKbo] = 0.1,
//                 [EventType.MaatschappelijkeZetelOvernemenUitKbo] = 0.2,
//                 [EventType.MaatschappelijkeZetelWijzigenInKbo] = 0.1,
//                 [EventType.VertegenwoordigerOvernemenUitKbo] = 0.2,
//                 [EventType.RechtsvormWijzigenInKbo] = 0.1,
//
//                 // Low frequency events
//                 [EventType.RoepnaamWijziging] = 0.2,
//                 [EventType.EinddatumWijziging] = 0.1,
//                 [EventType.LidmaatschapVerwijderen] = 0.2,
//                 [EventType.WerkingsgebiedenNietVanToepassing] = 0.1,
//
//                 // Association type changes (rare, early lifecycle only)
//                 [EventType.VerenigingssubtypeVerfijnenNaarFeitelijkeVereniging] = 0.05,
//                 [EventType.VerenigingssubtypeVerfijnenNaarSubvereniging] = 0.03,
//                 [EventType.FeitelijkeVerenigingMigrerenNaarVerenigingZonderRechtspersoonlijkheid] = 0.02,
//
//                 // Publication events
//                 [EventType.InschrijvenInPubliekeDatastroom] = 0.2,
//                 [EventType.UitschrijvenUitPubliekeDatastroom] = 0.1,
//
//                 // Very rare events
//                 [EventType.MarkerenAlsDubbeleVereniging] = 0.01,
//                 [EventType.CorrigeerDubbeleVereniging] = 0.01,
//
//                 // Lifecycle end events (very rare)
//                 [EventType.VerenigingStoppen] = 0.02,
//                 [EventType.VerenigingStoppenInKbo] = 0.01,
//                 [EventType.VerenigingVerwijderen] = 0.005
//             });
//         }
//
//         public double GetWeight(EventType eventType)
//         {
//             return _weights.TryGetValue(eventType, out var weight) ? weight : 0.1;
//         }
//     }
//
//     private class AssociationState
//     {
//         // Basic properties
//         public string VCode { get; }
//         public string Naam { get; set; } = string.Empty;
//         public string KorteNaam { get; set; } = string.Empty;
//         public string KorteBeschrijving { get; set; } = string.Empty;
//         public DateOnly? Startdatum { get; set; }
//         public DateOnly? Einddatum { get; set; }
//         public Registratiedata.Doelgroep? Doelgroep { get; set; }
//         public AssociationType AssociationType { get; set; }
//
//         // KBO-specific properties
//         public string? KboNummer { get; set; }
//         public string? Rechtsvorm { get; set; }
//         public bool HasKboNumber => !string.IsNullOrEmpty(KboNummer);
//
//         // Collections
//         public List<Registratiedata.Contactgegeven> Contactgegevens { get; } = new();
//         public List<Registratiedata.Contactgegeven> ContactgegevensFromKbo { get; } = new();
//         public List<Registratiedata.Locatie> Locaties { get; } = new();
//         public Registratiedata.Locatie? MaatschappelijkeZetelFromKbo { get; set; }
//         public List<Registratiedata.Vertegenwoordiger> Vertegenwoordigers { get; } = new();
//         public List<Registratiedata.Vertegenwoordiger> VertegenwoordigersFromKbo { get; } = new();
//         public List<Registratiedata.Lidmaatschap> Lidmaatschappen { get; } = new();
//         public Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>();
//         public Registratiedata.Werkingsgebied[] Werkingsgebieden { get; set; } = Array.Empty<Registratiedata.Werkingsgebied>();
//
//         // State flags
//         public bool IsDeleted { get; set; }
//         public bool IsStopped { get; set; }
//         public bool IsInPubliekeDatastroom { get; set; }
//         public bool IsMarkedAsDuplicate { get; set; }
//         public string? AuthentiekeVereniging { get; set; }
//
//         // Subvereniging properties
//         public Registratiedata.SubverenigingVan? SubverenigingVan { get; set; }
//
//         // ID generators
//         private int _nextContactgegevenId = 1;
//         private int _nextLocatieId = 1;
//         private int _nextVertegenwoordigerId = 1;
//         private int _nextLidmaatschapId = 1;
//
//         public AssociationState(string vCode)
//         {
//             VCode = vCode;
//         }
//
//         public void ApplyRegistration(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd @event)
//         {
//             Naam = @event.Naam;
//             KorteNaam = @event.KorteNaam;
//             KorteBeschrijving = @event.KorteBeschrijving;
//             Startdatum = @event.Startdatum;
//             Doelgroep = @event.Doelgroep;
//             IsInPubliekeDatastroom = !@event.IsUitgeschrevenUitPubliekeDatastroom;
//             AssociationType = AssociationType.VerenigingZonderRechtspersoonlijkheid;
//
//             Contactgegevens.AddRange(@event.Contactgegevens);
//             Locaties.AddRange(@event.Locaties);
//             Vertegenwoordigers.AddRange(@event.Vertegenwoordigers);
//             HoofdactiviteitenVerenigingsloket = @event.HoofdactiviteitenVerenigingsloket;
//
//             UpdateIdCounters();
//         }
//
//         public void ApplyFeitelijkeVerenigingRegistration(FeitelijkeVerenigingWerdGeregistreerd @event)
//         {
//             Naam = @event.Naam;
//             KorteNaam = @event.KorteNaam;
//             KorteBeschrijving = @event.KorteBeschrijving;
//             Startdatum = @event.Startdatum;
//             Doelgroep = @event.Doelgroep;
//             IsInPubliekeDatastroom = !@event.IsUitgeschrevenUitPubliekeDatastroom;
//             AssociationType = AssociationType.FeitelijkeVereniging;
//
//             Contactgegevens.AddRange(@event.Contactgegevens);
//             Locaties.AddRange(@event.Locaties);
//             Vertegenwoordigers.AddRange(@event.Vertegenwoordigers);
//             HoofdactiviteitenVerenigingsloket = @event.HoofdactiviteitenVerenigingsloket;
//
//             UpdateIdCounters();
//         }
//
//         public void ApplyKboRegistration(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd @event)
//         {
//             Naam = @event.Naam;
//             KorteNaam = @event.KorteNaam;
//             Startdatum = @event.Startdatum;
//             KboNummer = @event.KboNummer;
//             Rechtsvorm = @event.Rechtsvorm;
//             AssociationType = AssociationType.VerenigingMetRechtspersoonlijkheid;
//             IsInPubliekeDatastroom = true;
//         }
//
//         private void UpdateIdCounters()
//         {
//             _nextContactgegevenId = Contactgegevens.Any() ? Contactgegevens.Max(c => c.ContactgegevenId) + 1 : 1;
//             _nextLocatieId = Locaties.Any() ? Locaties.Max(l => l.LocatieId) + 1 : 1;
//             _nextVertegenwoordigerId = Vertegenwoordigers.Any() ? Vertegenwoordigers.Max(v => v.VertegenwoordigerId) + 1 : 1;
//             _nextLidmaatschapId = Lidmaatschappen.Any() ? Lidmaatschappen.Max(l => l.LidmaatschapId) + 1 : 1;
//         }
//
//         public int GetNextContactgegevenId() => _nextContactgegevenId++;
//         public int GetNextLocatieId() => _nextLocatieId++;
//         public int GetNextVertegenwoordigerId() => _nextVertegenwoordigerId++;
//         public int GetNextLidmaatschapId() => _nextLidmaatschapId++;
//     }
//
//     #endregion
// }
