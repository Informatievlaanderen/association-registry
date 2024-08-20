﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe;
using AssociationRegistry.Test.Admin.Api.Framework;
using Moq;
using Wolverine;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Null_Request
{
    private readonly VoegVertegenwoordigerToeController _controller;

    public With_Null_Request()
    {
        var messageBus = Mock.Of<IMessageBus>();
        _controller = new VoegVertegenwoordigerToeController(messageBus, new VoegVertegenwoordigerToeValidator());
    }

    [Fact]
    public async Task Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Post(
                vCode: "V001001",
                null!,
                new CommandMetadataProviderStub { Initiator = "OVO000001" },
                ifMatch: "M/\"1\""));
    }
}
