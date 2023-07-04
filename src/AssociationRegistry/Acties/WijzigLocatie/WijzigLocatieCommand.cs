namespace AssociationRegistry.Acties.WijzigLocatie;

using Vereniging;

public record WijzigLocatieCommand(VCode VCode, WijzigLocatieCommand.CommandLocatie Locatie)
{
    public record CommandLocatie(
        int LocatieId,
        Locatietype? Locatietype,
        bool? IsPrimair,
        string? Naam,
        Adres? Adres,
        AdresId? AdresId);
}
