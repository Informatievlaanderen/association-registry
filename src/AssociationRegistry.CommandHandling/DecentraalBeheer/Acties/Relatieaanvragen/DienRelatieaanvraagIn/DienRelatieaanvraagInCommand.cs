namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.DienRelatieaanvraagIn;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

/// <param name="Partijen">The koepel, the vereniging and the aanvrager from vr-namens, checked before anything is loaded.</param>
public record DienRelatieaanvraagInCommand(Partijen Partijen, RelatieType RelatieType, string Beschrijving);
