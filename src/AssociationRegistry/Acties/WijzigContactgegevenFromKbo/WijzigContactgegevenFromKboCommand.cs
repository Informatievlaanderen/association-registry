﻿namespace AssociationRegistry.Acties.WijzigContactgegevenFromKbo;

using Vereniging;

public record WijzigContactgegevenFromKboCommand(VCode VCode, WijzigContactgegevenFromKboCommand.CommandContactgegeven Contactgegeven)
{
    public record CommandContactgegeven(int ContacgegevenId, string? Beschrijving, bool? IsPrimair);
}
