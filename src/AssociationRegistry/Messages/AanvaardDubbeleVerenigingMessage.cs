namespace AssociationRegistry.Messages;

using Acties.AanvaardDubbel;
using AssociationRegistry.Vereniging;

public record AanvaardDubbeleVerenigingMessage(string VCode, string VCodeDubbeleVereniging)
{
    public AanvaardDubbeleVerenigingCommand ToCommand()
        => new(AssociationRegistry.Vereniging.VCode.Create(VCode), AssociationRegistry.Vereniging.VCode.Create(VCodeDubbeleVereniging));
}
