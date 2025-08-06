namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Test.Common.Stubs.VCodeServices;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;

public class VCodeServiceFactory
{
    public StubVCodeService Stub(VCode vCode)
        => new StubVCodeService(vCode);
}
