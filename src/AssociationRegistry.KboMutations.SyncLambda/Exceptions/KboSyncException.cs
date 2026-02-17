namespace AssociationRegistry.KboMutations.SyncLambda.Exceptions;

[Serializable]
public class KboSyncException : ApplicationException
{
    private readonly string _vCode;
    private readonly string _kboNummer;

    public KboSyncException(string vCode, string kboNummer)
    {
        _vCode = vCode;
        _kboNummer = kboNummer;
    }
}
