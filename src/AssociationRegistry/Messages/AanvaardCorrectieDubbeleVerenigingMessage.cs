namespace AssociationRegistry.Messages;

using Acties.AanvaardCorrectieDubbel;

public record AanvaardCorrectieDubbeleVerenigingMessage(string VCode, string VCodeDubbeleVereniging)
{
    public AanvaardCorrectieDubbeleVerenigingCommand ToCommand()
        => new(Vereniging.VCode.Create(VCode), Vereniging.VCode.Create(VCodeDubbeleVereniging));
}
