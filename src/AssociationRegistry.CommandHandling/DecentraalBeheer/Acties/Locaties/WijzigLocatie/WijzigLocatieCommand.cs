namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.WijzigLocatie;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;

public record WijzigLocatieCommand(VCode VCode, WijzigLocatieCommand.Locatie TeWijzigenLocatie)
{
    public record Locatie(
        int LocatieId,
        Locatietype? Locatietype,
        bool? IsPrimair,
        string? Naam,
        Adres? Adres,
        AdresId? AdresId);
}
