namespace AssociationRegistry.Messages;


public record CorrigeerAanvaardingDubbeleVerenigingMessage(string VCode, string VCodeDubbeleVereniging)
{
    public CorrigeerAanvaardingDubbeleVerenigingMessage ToCommand()
        => new(Vereniging.VCode.Create(VCode), Vereniging.VCode.Create(VCodeDubbeleVereniging));
}
