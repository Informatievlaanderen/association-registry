namespace AssociationRegistry.Notifications.Messages;

public class KboSynchronisatieMisluktMessage:IMessage
{
    private readonly string _kboNummer;

    public KboSynchronisatieMisluktMessage(string kboNummer)
    {
        _kboNummer = kboNummer;
    }

    public string Value => $"Vereniging met kbonummer: {_kboNummer}, kon niet gesynchroniseerd worden.";
    public NotifyType Type => NotifyType.Failure;
}
