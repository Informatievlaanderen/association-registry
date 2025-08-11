namespace AssociationRegistry.CommandHandling.KboSyncLambda.SyncKbo.Messages;

using AssociationRegistry.Integrations.Slack;

public class KboSynchronisatieMisluktNotification:INotification
{
    private readonly string _kboNummer;

    public KboSynchronisatieMisluktNotification(string kboNummer)
    {
        _kboNummer = kboNummer;
    }

    public string Value => $"Vereniging met kbonummer: {_kboNummer}, kon niet gesynchroniseerd worden.";
    public NotifyType Type => NotifyType.Failure;
}
