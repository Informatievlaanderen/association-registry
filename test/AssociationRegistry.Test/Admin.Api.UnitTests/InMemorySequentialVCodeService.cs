namespace AssociationRegistry.Test.Admin.Api.UnitTests;

using System.Threading.Tasks;
using VCodes;

public class InMemorySequentialVCodeService : IVCodeService
{
    private int _vCode = 0;

    public Task<VCode> GetNext()
        => Task.FromResult(new VCode(++_vCode));

    /// <summary>
    /// only for testing
    /// </summary>
    public string GetLast()
        => new VCode(_vCode);
}
