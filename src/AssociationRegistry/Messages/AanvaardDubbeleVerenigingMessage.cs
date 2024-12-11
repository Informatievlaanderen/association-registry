namespace AssociationRegistry.Messages;

using Acties.AanvaardDubbel;
using AssociationRegistry.Vereniging;

public record AanvaardDubbeleVerenigingMessage(VCode VCode, VCode VCodeDubbeleVereniging)
{
    public AanvaardDubbeleVerenigingCommand ToCommand()
        => new(VCode, VCodeDubbeleVereniging);
}
