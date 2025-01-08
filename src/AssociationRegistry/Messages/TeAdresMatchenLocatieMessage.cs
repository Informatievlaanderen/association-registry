namespace AssociationRegistry.Messages;

using DecentraalBeheer.Administratie.ProbeerAdresTeMatchen;

public record TeAdresMatchenLocatieMessage(string VCode, int LocatieId)
{
    public ProbeerAdresTeMatchenCommand ToCommand()
        => new(VCode, LocatieId);
}
