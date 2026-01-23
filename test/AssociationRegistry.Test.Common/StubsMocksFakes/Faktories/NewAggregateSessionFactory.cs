namespace AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;

public class NewAggregateSessionFactory
{
    public NewAggregateSessionMock Mock() => new();

    public NewAggregateSessionMock Mock(
        VerenigingState returns,
        bool expectedLoadingDubbel = false,
        bool expectedLoadingVerwijderd = false
    ) => new(returns, expectedLoadingDubbel, expectedLoadingVerwijderd);
}
