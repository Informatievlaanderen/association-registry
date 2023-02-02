namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging.Handling_The_Request;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Fakes;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Null_Request
{
    private readonly RegistreerVerenigingController _controller;

    public With_A_Null_Request()
    {
        var messageBusMock = new MessageBusMock();
        _controller = new RegistreerVerenigingController(messageBusMock, new AppSettings());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Post(
                new RegistreerVerenigingRequestValidator(),
                null));
    }
}
