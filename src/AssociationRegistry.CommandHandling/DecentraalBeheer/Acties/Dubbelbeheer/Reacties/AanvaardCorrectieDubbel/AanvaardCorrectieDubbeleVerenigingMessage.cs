namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardCorrectieDubbel;

public record AanvaardCorrectieDubbeleVerenigingMessage(string VCode, string VCodeDubbeleVereniging)
{
    public AanvaardCorrectieDubbeleVerenigingCommand ToCommand()
        => new(AssociationRegistry.DecentraalBeheer.Vereniging.VCode.Create(VCode), AssociationRegistry.DecentraalBeheer.Vereniging.VCode.Create(VCodeDubbeleVereniging));
}
