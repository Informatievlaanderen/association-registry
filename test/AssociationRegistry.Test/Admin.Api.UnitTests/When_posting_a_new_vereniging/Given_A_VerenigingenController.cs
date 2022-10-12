namespace AssociationRegistry.Test.Admin.Api.UnitTests.When_posting_a_new_vereniging;

using AssociationRegistry.Admin.Api.Verenigingen;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class Given_A_VerenigingenController
{
    [Fact]
    public async Task Then_it_return_AcceptedResult()
    {

        var controller = new VerenigingenController(Mock.Of<ISender>());
        var response = await controller.Post(null!);
        response.Should().BeOfType<AcceptedResult>();
    }
}
