namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.Vereniging;

public class InMemorySequentialVCodeService : IVCodeService
{
    private int _vCode = 1000;

    public Task<VCode> GetNext()
        => Task.FromResult(VCode.Create(++_vCode));

    /// <summary>
    /// only for testing
    /// </summary>
    public string GetLast()
        => VCode.Create(_vCode);
}
