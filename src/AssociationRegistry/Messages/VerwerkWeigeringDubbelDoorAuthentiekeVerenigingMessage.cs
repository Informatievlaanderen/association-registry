namespace AssociationRegistry.Messages;

using Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;

public record VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage(string VCode, string VCodeAuthentiekeVereniging)
{
    public VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand ToCommand()
        => new(Vereniging.VCode.Create(VCode), Vereniging.VCode.Create(VCodeAuthentiekeVereniging));
}
