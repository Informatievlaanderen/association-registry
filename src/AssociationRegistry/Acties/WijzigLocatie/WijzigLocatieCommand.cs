﻿namespace AssociationRegistry.Acties.WijzigLocatie;

using Vereniging;

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
