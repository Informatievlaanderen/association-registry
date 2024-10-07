namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging.Publiek_Zoeken;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;
using Contactgegeven = Public.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using DoelgroepResponse = Public.Api.Verenigingen.Detail.ResponseModels.DoelgroepResponse;
using GestructureerdeIdentificator = Public.Api.Verenigingen.Detail.ResponseModels.GestructureerdeIdentificator;
using HoofdactiviteitVerenigingsloket = Public.Api.Verenigingen.Detail.ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = Public.Api.Verenigingen.Detail.ResponseModels.Locatie;
using Relatie = Public.Api.Verenigingen.Detail.ResponseModels.Relatie;
using Sleutel = Public.Api.Verenigingen.Detail.ResponseModels.Sleutel;
using Vereniging = Public.Api.Verenigingen.Detail.ResponseModels.Vereniging;
using VerenigingsType = Public.Api.Verenigingen.Detail.ResponseModels.VerenigingsType;
using Werkingsgebied = Public.Api.Verenigingen.Detail.ResponseModels.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<RegistreerFeitelijkeVerenigingContext, RegistreerFeitelijkeVerenigingRequest, SearchVerenigingenResponse>
{
    private readonly RegistreerFeitelijkeVerenigingContext _context;

    public Returns_SearchVerenigingenResponse(RegistreerFeitelijkeVerenigingContext context) : base(context)
    {
        _context = context;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11004/v1/contexten/publiek/zoeken-vereniging-context.json");
    }

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekZoeken(_context.VCode);
}
