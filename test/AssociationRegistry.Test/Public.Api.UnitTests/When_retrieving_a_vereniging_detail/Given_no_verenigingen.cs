﻿namespace AssociationRegistry.Test.Public.Api.UnitTests.When_retrieving_a_vereniging_detail;

using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.DetailVerenigingen;
using AssociationRegistry.Public.Api.ListVerenigingen;
using AutoFixture;
using FluentAssertions;
using Framework.Stubs;
using Microsoft.AspNetCore.Mvc;
using Xunit;

public class Given_no_verenigingen
{
    private readonly Fixture _fixture;

    public Given_no_verenigingen()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Then_404_is_returned()
    {
        var controller =
            new DetailVerenigingenController(new VerenigingenRepositoryStub(new List<VerenigingListItem>()));

        var appSettingsStub = new AppSettings();
        var response = await controller.Detail(appSettingsStub, _fixture.Create<string>());
        response.Should().BeOfType<NotFoundResult>();
    }
}
