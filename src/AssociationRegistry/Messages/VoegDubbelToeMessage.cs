namespace AssociationRegistry.Messages;

using AssociationRegistry.Acties.VoegDubbelToe;
using AssociationRegistry.Vereniging;

public record VoegDubbelToeMessage(VCode VCode, VCode VCodeDubbeleVereniging)
{
    public VoegDubbelToeCommand ToCommand()
        => new(VCode, VCodeDubbeleVereniging);
}
