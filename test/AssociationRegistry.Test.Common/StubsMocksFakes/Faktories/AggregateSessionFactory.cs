namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Vereniging;

public class AggregateSessionFactory
{
    public AggregateSessionMock Mock() => new();

    public AggregateSessionMock Mock(
        VerenigingState returns,
        bool expectedLoadingDubbel = false,
        bool expectedLoadingVerwijderd = false
    ) => new(returns, expectedLoadingDubbel, expectedLoadingVerwijderd);
}
