namespace AssociationRegistry.Test.Admin.Api.Controllers.Given_A_RegistreerVerenigingController;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Xunit;

public class When_Patching_A_Null_Request
{
    private readonly RegistreerVerenigingController _controller;

    public When_Patching_A_Null_Request()
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
