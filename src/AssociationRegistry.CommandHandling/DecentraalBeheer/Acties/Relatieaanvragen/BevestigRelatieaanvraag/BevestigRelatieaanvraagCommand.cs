namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.BevestigRelatieaanvraag;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Vereniging;

/// <param name="BevestigdDoor">The vCode from vr-namens: must be the ontvanger of the relatieaanvraag.</param>
/// <param name="Relatiegegevens">The optional identificatie and beschrijving of the relatie that is made.</param>
public record BevestigRelatieaanvraagCommand(
    RelatieaanvraagId AanvraagId,
    VCode BevestigdDoor,
    Relatiegegevens Relatiegegevens
);
