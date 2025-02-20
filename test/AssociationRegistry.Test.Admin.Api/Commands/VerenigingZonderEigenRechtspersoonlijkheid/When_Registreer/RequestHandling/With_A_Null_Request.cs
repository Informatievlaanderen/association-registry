namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingZonderEigenRechtspersoonlijkheid.When_Registreer.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequetsModels;
using Hosts.Configuration.ConfigurationBindings;
using Framework;
using Framework.Fakes;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Null_Request
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidController _controller;

    public With_A_Null_Request()
    {
        var messageBusMock = new MessageBusMock();

        _controller = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidController(messageBusMock,
                                                                   new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(
                                                                       new ClockStub(DateOnly.MaxValue)), new AppSettings());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Post(request: null, new CommandMetadataProviderStub { Initiator = "OVO000001" }, string.Empty));
    }
}
