using AssociationRegistry.Notifications;

namespace AssociationRegistry.KboMutations.SyncLambda.Logging;

public readonly record struct KboSyncLambdaGefaald : IMessage
{
    private readonly string _kboSyncMessage;
    private readonly Exception _exception;

    public KboSyncLambdaGefaald(string kboSyncMessage, Exception exception)
    {
        _kboSyncMessage = kboSyncMessage;
        _exception = exception;
    }
    public string Value => $"KBO Sync lambda gefaald voor {_kboSyncMessage}. {_exception.Message}";
    public NotifyType Type => NotifyType.Failure;
}