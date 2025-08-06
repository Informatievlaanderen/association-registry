namespace AssociationRegistry.Notifications.Messages;

using DecentraalBeheer.Vereniging;
using Notifications;
using Vereniging;

public class VerwerkWeigeringDubbelDoorAuthentiekeVerenigingGefaald : IMessage
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
