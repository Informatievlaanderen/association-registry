namespace AssociationRegistry.Test.Admin.Api.UnitTests;

using System.Threading.Tasks;
using AssociationRegistry.Admin.Api.Verenigingen.VCodes;

public class InMemorySequentialVCodeService : IVCodeService
{
    private static int vCode = 0;

    public Task<VCode> GetNext()
        => Task.FromResult(new VCode(++vCode));

    /// <summary>
    /// only for testing
    /// </summary>
    public static string GetLast()
        => new VCode(vCode);
}
