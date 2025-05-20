namespace AssociationRegistry.Test.Common.Stubs.VCodeServices;

using AssociationRegistry.Vereniging;

public class StubVCodeService : IVCodeService
{
    public VCode VCode { get; }

    public StubVCodeService(VCode vCode)
    {
        VCode = vCode;
    }

    public async Task<VCode> GetNext()
        => VCode;
}
