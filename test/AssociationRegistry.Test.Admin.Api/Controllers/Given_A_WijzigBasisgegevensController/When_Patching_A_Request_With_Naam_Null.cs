namespace AssociationRegistry.Test.Admin.Api.Controllers.Given_A_WijzigBasisgegevensController;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using FluentValidation;
using Xunit;

public class When_Patching_A_Request_With_Naam_Null
{
    private readonly WijzigBasisgegevensController _controller;
    private readonly WijzigBasisgegevensRequest _requestWithNaamNull = new() {Initiator = "OVO000001", Naam = null};
    private const string VCode = "V0001001";

    public When_Patching_A_Request_With_Naam_Null()
    {
        var messageBusMock = new MessageBusMock();
        _controller = new WijzigBasisgegevensController(messageBusMock, new AppSettings());
    }

    [Fact]
    public async Task Then_it_throws_a_ValidationException()
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _controller.Patch(
                new WijzigBasisgegevensRequestValidator(),
                _requestWithNaamNull,
                VCode,
                "M/\"1\""));
    }
}
