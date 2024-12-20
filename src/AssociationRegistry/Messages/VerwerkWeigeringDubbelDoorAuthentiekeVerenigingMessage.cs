namespace AssociationRegistry.Messages;

using Acties.VerwerkWeigeringDubbelDoorAuthentiekeVereniging;

public record VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage(string VCode)
{
    public VerwerkWeigeringDubbelDoorAuthentiekeVerenigingCommand ToCommand()
        => new(Vereniging.VCode.Create(VCode));
}
