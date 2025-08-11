namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.WijzigMaatschappelijkeZetel;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record WijzigMaatschappelijkeZetelCommand(VCode VCode, WijzigMaatschappelijkeZetelCommand.Locatie TeWijzigenLocatie)
{
    public record Locatie(
        int LocatieId,
        bool? IsPrimair,
        string? Naam);
}
