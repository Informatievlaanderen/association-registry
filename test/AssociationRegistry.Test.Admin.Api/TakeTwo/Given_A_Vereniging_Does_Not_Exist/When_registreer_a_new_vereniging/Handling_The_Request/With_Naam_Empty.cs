namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging.Handling_The_Request;

using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Fakes;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Naam_Empty
{
    private readonly RegistreerVerenigingController _controller;
    private readonly RegistreerVerenigingRequest _request;

    public With_Naam_Empty()
    {
        var messageBusMock = new MessageBusMock();
        _controller = new RegistreerVerenigingController(messageBusMock, new AppSettings());
        _request = new RegistreerVerenigingRequest
        {
            Initiator = "V0001001",
            Naam = string.Empty,
        };
    }

    [Fact]
    public async Task Then_it_throws_a_ValidationException()
    {
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            async () => await _controller.Post(
                new RegistreerVerenigingRequestValidator(),
                _request));
    }
}
