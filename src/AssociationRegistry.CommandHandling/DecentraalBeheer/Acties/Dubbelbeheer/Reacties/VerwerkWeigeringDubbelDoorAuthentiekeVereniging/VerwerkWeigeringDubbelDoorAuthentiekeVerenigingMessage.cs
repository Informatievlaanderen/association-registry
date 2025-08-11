namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;

public record VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage(string VCode, string VCodeAuthentiekeVereniging)
{
    public VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand ToCommand()
        => new(AssociationRegistry.DecentraalBeheer.Vereniging.VCode.Create(VCode), AssociationRegistry.DecentraalBeheer.Vereniging.VCode.Create(VCodeAuthentiekeVereniging));
}
