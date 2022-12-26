namespace AssociationRegistry.Test.Admin.Api.UnitTests.CommandHandlerTests;

using AssociationRegistry.Framework;
using Vereniging;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    public record Invocation(Vereniging Vereniging);

    public readonly List<Invocation> Invocations = new();

    public async Task<long> Save(Vereniging vereniging, CommandMetadata metadata)
    {
        Invocations.Add(new Invocation(vereniging));
        return await Task.FromResult(-1L);
    }
}
