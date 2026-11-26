namespace AssociationRegistry.Events;

/// <summary>
///     The vervaldatum of the relatieaanvraag passed while it was still open: the job of the scheduled host closed it.
///     Nothing changes on the verenigingen.
/// </summary>
/// <param name="VervalDatum">
///     The day the relatieaanvraag vervallen werd: the day the job ran, like the WeigeringsDatum of a weigering. Not the
///     vervaldatum from <see cref="RelatieaanvraagWerdIngediend" />, the last day it was open: when the job runs later, the
///     two are more than a day apart.
/// </param>
public record RelatieaanvraagWerdVervallen(string AanvraagId, DateOnly VervalDatum) : IRelatieaanvraagEvent;
