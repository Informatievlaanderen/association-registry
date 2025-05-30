namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using Vereniging;

public class VerenigingsRepositoryFactory
{
    public VerenigingRepositoryMock Mock()
        => new();


    public VerenigingRepositoryMock Mock(VerenigingState returns)
        => new(returns);
}
