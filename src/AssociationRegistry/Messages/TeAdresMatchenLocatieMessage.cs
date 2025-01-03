﻿namespace AssociationRegistry.Messages;

using Acties.ProbeerAdresTeMatchen;

public record TeAdresMatchenLocatieMessage(string VCode, int LocatieId)
{
    public ProbeerAdresTeMatchenCommand ToCommand()
        => new(VCode, LocatieId);
}
