namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_WijzigBasisGegevens.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;
using AssociationRegistry.Test.Admin.Api.Fakes;
using FluentValidation;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Naam_Null
{
    private readonly WijzigBasisgegevensController _controller;
    private readonly WijzigBasisgegevensRequest _requestWithNaamNull = new() {Initiator = "OVO000001", Naam = null};
    private const string VCode = "V0009001";

    public With_Naam_Null()
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
