namespace AssociationRegistry.Test.Admin.Api.When_Removing_Contactgegeven.CommandHandling;

using AutoFixture;
using Fakes;
using Framework;
using Vereniging.VerwijderContactgegevens;
using Xunit;

public class Given_An_Unknown_VCode
{
    private readonly VerwijderContactgegevenCommandHandler _commandHandler;
    private readonly Fixture _fixture;

    public Given_An_Unknown_VCode()
    {
        var verenigingRepositoryMock = new VerenigingRepositoryMock();
        _fixture = new Fixture().CustomizeAll();

        _commandHandler = new VerwijderContactgegevenCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public void Then_XXX(){}
}
