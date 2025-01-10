namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Duplicates_With_Gemeentenaam_In_Verenigingsnaam.Beheer.Detail;

using Acm.Api.Infrastructure.Options;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Bronnen;
using Azure;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;
using Contactgegeven = Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = Vereniging.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels.Locatie;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Vertegenwoordiger = Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels.Vertegenwoordiger;
using Werkingsgebied = Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_DetailResponse : IClassFixture<RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext>, IAsyncLifetime
{
    private readonly RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext _context;


    public Returns_DetailResponse(RegistreerFeitelijkeVerenigingenWithGemeentenaamInVerenigingsnaamContext context)
    {
        _context = context;
    }

    [Fact]
    public void With_Context()
    {
        foreach (var response in _context.Responses)
        {
            response.actual.Should().BeEquivalentTo(response.expected);
        }
    }


    public async Task InitializeAsync()
        => await Task.CompletedTask;

    public async Task DisposeAsync()
        => await Task.CompletedTask;
}
