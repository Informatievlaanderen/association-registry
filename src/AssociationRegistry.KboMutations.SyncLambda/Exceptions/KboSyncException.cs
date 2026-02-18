namespace AssociationRegistry.KboMutations.SyncLambda.Exceptions;

[Serializable]
public class KboSyncException : Exception
{
    public readonly string VCode;
    public readonly string KboNummer;

    public KboSyncException(string vCode, string kboNummer, Exception inner)
        : base($"KBO sync failed for VCode '{vCode}' and KBO '{kboNummer}'", inner)
    {
        VCode = vCode;
        KboNummer = kboNummer;
    }
}
