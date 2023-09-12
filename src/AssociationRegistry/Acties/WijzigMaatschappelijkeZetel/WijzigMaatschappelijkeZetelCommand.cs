namespace AssociationRegistry.Acties.WijzigMaatschappelijkeZetel;

using Vereniging;

public record WijzigMaatschappelijkeZetelCommand(VCode VCode, WijzigMaatschappelijkeZetelCommand.Locatie TeWijzigenLocatie)
{
    public record Locatie(
        int LocatieId,
        bool? IsPrimair,
        string? Naam);
}
