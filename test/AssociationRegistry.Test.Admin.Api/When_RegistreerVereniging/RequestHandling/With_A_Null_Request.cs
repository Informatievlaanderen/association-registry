namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestHandling;

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
        _controller = new RegistreerVerenigingController(messageBusMock, new RegistreerVerenigingRequestValidator(), new AppSettings());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Post(null));
    }
}
