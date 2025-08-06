namespace AssociationRegistry.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;

public record AanvaardDubbeleVerenigingMessage(string VCode, string VCodeDubbeleVereniging)
{
    public AanvaardDubbeleVerenigingCommand ToCommand()
        => new(Vereniging.VCode.Create(VCode), Vereniging.VCode.Create(VCodeDubbeleVereniging));
}
