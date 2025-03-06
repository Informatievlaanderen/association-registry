﻿namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.Detail;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Schema.Constants;
using FluentAssertions;
using Framework.AlbaHost;
using Xunit;

[Collection(WellKnownCollections.MarkeerAlsDubbelVan)]
public class Returns_Detail_With_Dubbel_Van : IAsyncLifetime
{
    private readonly MarkeerAlsDubbelVanContext _context;

    public Returns_Detail_With_Dubbel_Van(MarkeerAlsDubbelVanContext context)
    {
        _context = context;
    }

    [Fact]
    public void With_IsDubbelVan_VCode_Of_AndereFeitelijkeVerenigingWerdGeregistreerd()
    {
        Response.Vereniging.IsDubbelVan.Should().Be(_context.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    [Fact]
    public void With_Status_Is_Dubbel()
    {
        Response.Vereniging.Status.Should().Be(VerenigingStatus.Dubbel);
    }

    public DetailVerenigingResponse Response { get; set; }

    public async ValueTask InitializeAsync()
    {
        Response = _context.ApiSetup.AdminApiHost.GetBeheerDetail(_context.VCode);
    }

    public async ValueTask DisposeAsync()
    {
    }
}
