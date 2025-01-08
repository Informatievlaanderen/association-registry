namespace AssociationRegistry.Messages;

using Acties.Administratie.ProbeerAdresTeMatchen;

public record TeAdresMatchenLocatieMessage(string VCode, int LocatieId)
{
    public ProbeerAdresTeMatchenCommand ToCommand()
        => new(VCode, LocatieId);
}
