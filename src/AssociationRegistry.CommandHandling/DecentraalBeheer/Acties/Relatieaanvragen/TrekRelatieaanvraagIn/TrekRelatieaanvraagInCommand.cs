namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.TrekRelatieaanvraagIn;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Vereniging;

/// <param name="IngetrokkenDoor">The vCode from vr-namens: must be the aanvrager of the relatieaanvraag.</param>
/// <param name="Reden">Optional: empty when none was given.</param>
public record TrekRelatieaanvraagInCommand(RelatieaanvraagId AanvraagId, VCode IngetrokkenDoor, string Reden);
