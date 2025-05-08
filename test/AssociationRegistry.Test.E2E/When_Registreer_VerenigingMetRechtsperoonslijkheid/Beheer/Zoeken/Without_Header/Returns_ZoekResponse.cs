namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Beheer.Zoeken.Without_Header;

using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.Mappers;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;
using Vereniging = Admin.Api.Verenigingen.Search.ResponseModels.Vereniging;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Verenigingstype = Admin.Api.Verenigingen.Search.ResponseModels.Verenigingstype;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse : End2EndTest<RegistreerVerenigingMetRechtsperoonlijkheidTestContext, RegistreerVerenigingUitKboRequest, SearchVerenigingenResponse>
{
    private readonly RegistreerVerenigingMetRechtsperoonlijkheidTestContext _testContext;

    public Returns_SearchVerenigingenResponse(RegistreerVerenigingMetRechtsperoonlijkheidTestContext testContext) : base(testContext)
    {
        _testContext = testContext;
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/zoek-verenigingen-context.json");
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen.Single().Verenigingssubtype.Should().BeNull();

    public override Func<IApiSetup, SearchVerenigingenResponse> GetResponse
        => setup => setup.AdminApiHost.GetBeheerZoeken($"vCode:{_testContext.RequestResult.VCode}");
}
