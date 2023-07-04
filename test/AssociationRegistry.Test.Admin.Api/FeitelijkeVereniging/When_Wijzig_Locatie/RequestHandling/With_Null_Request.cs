﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly WijzigLocatieController _controller;

    public With_Null_Request()
    {
        var messageBus = Mock.Of<IMessageBus>();
        _controller = new WijzigLocatieController(messageBus, new WijzigLocatieValidator());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Patch(
                "V001001",
                1,
                null!,
                new InitiatorProvider { Value = "OVO0001000"},
                "M/\"1\""));
    }
}
