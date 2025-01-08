namespace AssociationRegistry.DecentraalBeheer.Locaties.WijzigMaatschappelijkeZetel;

using AssociationRegistry.Vereniging;

public record WijzigMaatschappelijkeZetelCommand(VCode VCode, WijzigMaatschappelijkeZetelCommand.Locatie TeWijzigenLocatie)
{
    public record Locatie(
        int LocatieId,
        bool? IsPrimair,
        string? Naam);
}
