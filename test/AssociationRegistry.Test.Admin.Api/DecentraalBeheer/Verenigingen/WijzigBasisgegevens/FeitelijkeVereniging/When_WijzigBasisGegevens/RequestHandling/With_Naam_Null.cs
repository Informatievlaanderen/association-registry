namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestHandling;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using FluentValidation;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Naam_Null
{
    private readonly WijzigBasisgegevensController _controller;
    private readonly WijzigBasisgegevensRequest _requestWithNaamNull = new() { Naam = null };
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
                new CommandMetadataProviderStub { Initiator = "OVO0001001" },
                ifMatch: "M/\"1\""));
    }
}
