namespace AssociationRegistry.KboMutations.SyncLambda.Exceptions;

[Serializable]
public class KszSyncException : Exception
{
    public readonly string VCode;
    public readonly int VertegenwoordigerId;

    public KszSyncException(string vCode, int vertegenwoordigerId, Exception inner)
        : base($"KSZ sync failed for VCode '{vCode}' and VertegenwoordigerId '{vertegenwoordigerId}'", inner)
    {
        VCode = vCode;
        VertegenwoordigerId = vertegenwoordigerId;
    }
}
