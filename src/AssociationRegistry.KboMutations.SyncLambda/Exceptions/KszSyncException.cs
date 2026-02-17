namespace AssociationRegistry.KboMutations.SyncLambda.Exceptions;

[Serializable]
public class KszSyncException : ApplicationException
{
    private readonly string _vCode;
    private readonly int _vertegenwoordigerId;

    public KszSyncException(string vCode, int vertegenwoordigerId)
    {
        _vCode = vCode;
        _vertegenwoordigerId = vertegenwoordigerId;
    }
}
