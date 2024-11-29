namespace AssociationRegistry.Test.GrarUpdates;

using Common.Framework;
using Vereniging;
using Xunit;

public class Given_One_Location
{
    [Fact]
    public void Then_One_AdresWerdOntkoppeldVanAdressenregister_Is_Saved()
    {
        var repository = new VerenigingRepositoryMock();
        var sut = new TeOntkoppelenAdresHandler(repository);
    }
}

public class TeOntkoppelenAdresHandler
{
    private readonly IVerenigingsRepository _repository;

    public TeOntkoppelenAdresHandler(IVerenigingsRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(TeOnkoppelenAdresMessage message, CancellationToken cancellationToken)
    {

    }
}

public record TeOnkoppelenAdresMessage
{
}
