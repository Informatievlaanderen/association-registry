namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Contactgegeven.RequestHandling;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using Framework;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly WijzigContactgegevenController _controller;

    public With_Null_Request()
    {
        var messageBus = Mock.Of<IMessageBus>();
        _controller = new WijzigContactgegevenController(messageBus, new WijzigContactgegevenValidator());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Patch(
                vCode: "V001001",
                contactgegevenId: 1,
                null!,
                new CommandMetadataProviderStub(),
                ifMatch: "M/\"1\""));
    }
}
