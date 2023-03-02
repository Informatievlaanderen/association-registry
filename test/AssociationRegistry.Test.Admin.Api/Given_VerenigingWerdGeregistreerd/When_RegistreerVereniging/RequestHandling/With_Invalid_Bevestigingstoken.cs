namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_RegistreerVereniging.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AutoFixture;
using Fakes;
using Framework;
using VCodes;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Invalid_Bevestigingstoken
{
    private readonly RegistreerVerenigingController _controller;
    private readonly RegistreerVerenigingRequest _request;
    private readonly Fixture _fixture;

    public With_Invalid_Bevestigingstoken()
    {
        _fixture = new Fixture().CustomizeAll();
        _request = new RegistreerVerenigingRequest { Initiator = _fixture.Create<VCode>(), Naam = _fixture.Create<string>()};
        var messageBusMock = new MessageBusMock();
        _controller = new RegistreerVerenigingController(messageBusMock, new RegistreerVerenigingRequestValidator(),new AppSettings(){Salt = "RandomS@lt"});
    }

    [Fact]
    public async Task Then_it_throws_a_ValidationException()
    {
        await Assert.ThrowsAsync<InvalidBevestigingstokenProvided>(
            () => _controller.Post(_request, _fixture.Create<string>()));
    }
}
