namespace AssociationRegistry.Messages;

using DecentraalBeheer.Administratie.ProbeerAdresTeMatchen;
using DecentraalBeheer.Locaties.ProbeerAdresTeMatchen;

public record TeAdresMatchenLocatieMessage(string VCode, int LocatieId)
{
    public ProbeerAdresTeMatchenCommand ToCommand()
        => new(VCode, LocatieId);
}
