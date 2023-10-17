﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;
using Fakes;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly WijzigBasisgegevensController _controller;

    public With_Null_Request()
    {
        var messageBusMock = new MessageBusMock();
        _controller = new WijzigBasisgegevensController(messageBusMock, new AppSettings());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Patch(
                new WijzigBasisgegevensRequestValidator(),
                null,
                "V0001001",
                new CommandMetadataProviderStub { Initiator = "OVO0001001"},
                "M/\"1\""));
    }
}
