namespace AssociationRegistry.Events;

/// <summary>
///     The vervaldatum of the relatieaanvraag passed while it was still open: the job of the scheduled host closed it.
///     Nothing changes on the verenigingen. The day the job ran is not in it: that is in the metadata of the event.
/// </summary>
public record RelatieaanvraagWerdVervallen(string AanvraagId, DateOnly VervalDatum) : IRelatieaanvraagEvent;
