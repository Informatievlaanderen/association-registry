namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Vereniging;

public class VerenigingsRepositoryFactory
{
    public VerenigingRepositoryMock Mock()
        => new();


    public VerenigingRepositoryMock Mock(VerenigingState returns, bool expectedLoadingDubbel = false, bool expectedLoadingVerwijderd = false)
        => new(returns, expectedLoadingDubbel, expectedLoadingVerwijderd);
}
