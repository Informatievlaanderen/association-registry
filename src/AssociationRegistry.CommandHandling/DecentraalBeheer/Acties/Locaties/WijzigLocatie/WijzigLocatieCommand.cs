namespace AssociationRegistry.DecentraalBeheer.Acties.Locaties.WijzigLocatie;

using Vereniging;
using Vereniging.Adressen;

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
