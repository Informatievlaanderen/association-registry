namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.WeigerRelatieaanvraag;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Vereniging;

/// <param name="GeweigerdDoor">The vCode from vr-namens: must be the ontvanger of the relatieaanvraag.</param>
public record WeigerRelatieaanvraagCommand(RelatieaanvraagId AanvraagId, VCode GeweigerdDoor);
