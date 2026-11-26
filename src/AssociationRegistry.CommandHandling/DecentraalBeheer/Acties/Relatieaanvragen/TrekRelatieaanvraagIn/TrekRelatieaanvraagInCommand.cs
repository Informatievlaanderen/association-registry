namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.TrekRelatieaanvraagIn;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Vereniging;

/// <param name="IngetrokkenDoor">The vCode from vr-namens: must be the aanvrager of the relatieaanvraag.</param>
public record TrekRelatieaanvraagInCommand(RelatieaanvraagId AanvraagId, VCode IngetrokkenDoor);
