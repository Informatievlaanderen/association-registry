namespace AssociationRegistry.Messages;

using Acties.Dubbelbeheer.AanvaardCorrectieDubbel;

public record AanvaardCorrectieDubbeleVerenigingMessage(string VCode, string VCodeDubbeleVereniging)
{
    public AanvaardCorrectieDubbeleVerenigingCommand ToCommand()
        => new(Vereniging.VCode.Create(VCode), Vereniging.VCode.Create(VCodeDubbeleVereniging));
}
