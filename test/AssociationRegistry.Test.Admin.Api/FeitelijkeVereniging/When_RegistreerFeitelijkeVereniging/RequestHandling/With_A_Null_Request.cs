namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Fakes;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Null_Request
{
    private readonly RegistreerFeitelijkeVerenigingController _controller;

    public With_A_Null_Request()
    {
        var messageBusMock = new MessageBusMock();

        _controller = new RegistreerFeitelijkeVerenigingController(messageBusMock,
                                                                   new RegistreerFeitelijkeVerenigingRequestValidator(
                                                                       new ClockStub(DateOnly.MaxValue)), new AppSettings());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Post(request: null, new CommandMetadataProviderStub { Initiator = "OVO000001" }, string.Empty));
    }
}
