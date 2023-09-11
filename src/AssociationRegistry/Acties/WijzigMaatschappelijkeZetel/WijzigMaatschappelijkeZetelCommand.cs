namespace AssociationRegistry.Acties.WijzigMaatschappelijkeZetel;

using Vereniging;

public record WijzigMaatschappelijkeZetelCommand(VCode VCode, int LocatieId, string? Naam, bool? IsPrimair);
