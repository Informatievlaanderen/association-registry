namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AutoFixture;
using Fakes;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Invalid_Bevestigingstoken
{
    private readonly RegistreerFeitelijkeVerenigingController _controller;
    private readonly Fixture _fixture;
    private readonly RegistreerFeitelijkeVerenigingRequest _request;

    public With_Invalid_Bevestigingstoken()
    {
        _fixture = new Fixture().CustomizeAll();
        _request = new RegistreerFeitelijkeVerenigingRequest { Naam = _fixture.Create<string>() };
        var messageBusMock = new MessageBusMock();
        _controller = new RegistreerFeitelijkeVerenigingController(messageBusMock, new RegistreerFeitelijkeVerenigingRequestValidator(), new AppSettings { Salt = "RandomS@lt" });
    }

    [Fact]
    public async Task Then_it_throws_a_ValidationException()
    {
        await Assert.ThrowsAsync<InvalidBevestigingstokenProvided>(
            () => _controller.Post(_request, _fixture.Create<string>(), _fixture.Create<string>()));
    }
}
