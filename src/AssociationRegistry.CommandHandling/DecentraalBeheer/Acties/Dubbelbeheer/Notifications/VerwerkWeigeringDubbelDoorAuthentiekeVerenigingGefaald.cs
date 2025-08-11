namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Notifications;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Integrations.Slack;
using System;

public class VerwerkWeigeringDubbelDoorAuthentiekeVerenigingGefaald : INotification
{
    private readonly Exception _exception;
    private readonly VCode _vCode;
    private readonly VCode _authentiekeVerenging;

    public VerwerkWeigeringDubbelDoorAuthentiekeVerenigingGefaald(Exception exception, VCode vCode, VCode authentiekeVerenging)
    {
        _exception = exception;
        _vCode = vCode;
        _authentiekeVerenging = authentiekeVerenging;
    }

    public string Value => $"Dubbel ({_vCode}) werd geweigerd door authentieke vereniging({_authentiekeVerenging}): {_exception.Message}.";
    public NotifyType Type => NotifyType.Failure;
}
