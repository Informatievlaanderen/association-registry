namespace AssociationRegistry.Messages;

using Acties.AanvaardDubbel;

public record AanvaardDubbeleVerenigingMessage(string VCode, string VCodeDubbeleVereniging)
{
    public AanvaardDubbeleVerenigingCommand ToCommand()
        => new(Vereniging.VCode.Create(VCode), Vereniging.VCode.Create(VCodeDubbeleVereniging));
}
