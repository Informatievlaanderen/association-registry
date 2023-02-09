namespace AssociationRegistry.Test.Admin.Api.Given_A_Vereniging_Does_Not_Exist.When_registreer_a_new_vereniging.Handling_The_Request;

using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AutoFixture;
using Fakes;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Empty_KboNumber
{
    private readonly RegistreerVerenigingController _controller;
    private readonly RegistreerVerenigingRequest _request;

    public With_Empty_KboNumber()
    {
        var messageBusMock = new MessageBusMock();
        var autoFixture = new Fixture();
        _controller = new RegistreerVerenigingController(messageBusMock, new AppSettings());
        _request = new RegistreerVerenigingRequest
        {
            Initiator = "V0001001",
            Naam = autoFixture.Create<string>(),
            KboNummer = string.Empty,
        };
    }

    [Fact]
    public async Task Then_it_is_valid()
    {
        await Assert.ThrowsAsync<NotImplementedException>(
            async () => await _controller.Post(
                new RegistreerVerenigingRequestValidator(),
                _request));
    }

}
