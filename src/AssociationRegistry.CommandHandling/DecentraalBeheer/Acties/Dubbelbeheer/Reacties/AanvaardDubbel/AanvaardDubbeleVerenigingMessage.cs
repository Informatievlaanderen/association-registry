namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;

public record AanvaardDubbeleVerenigingMessage(string VCode, string VCodeDubbeleVereniging)
{
    public AanvaardDubbeleVerenigingCommand ToCommand()
        => new(AssociationRegistry.DecentraalBeheer.Vereniging.VCode.Create(VCode), AssociationRegistry.DecentraalBeheer.Vereniging.VCode.Create(VCodeDubbeleVereniging));
}
