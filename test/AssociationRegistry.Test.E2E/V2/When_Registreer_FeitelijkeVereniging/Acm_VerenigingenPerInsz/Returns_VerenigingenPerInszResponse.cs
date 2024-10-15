namespace AssociationRegistry.Test.E2E.V2.When_Registreer_FeitelijkeVereniging.Acm_VerenigingenPerInsz;

using Acm.Api.VerenigingenPerInsz;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Schema.Constants;
using FluentAssertions;
using Formats;
using JsonLdContext;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.TestClasses;
using Vereniging;
using Vereniging.Bronnen;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;
using Contactgegeven = Admin.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = Vereniging.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Api.Verenigingen.Detail.ResponseModels.Locatie;
using Verenigingstype = Vereniging.Verenigingstype;
using Vertegenwoordiger = Admin.Api.Verenigingen.Detail.ResponseModels.Vertegenwoordiger;
using Werkingsgebied = Vereniging.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VerenigingenPerInszResponse :
    End2EndTest<RegistreerFeitelijkeVerenigingTestContext, RegistreerFeitelijkeVerenigingRequest, VerenigingenPerInszResponse>
{
    private readonly string _inszToCompare;

    public Returns_VerenigingenPerInszResponse(RegistreerFeitelijkeVerenigingTestContext testContext) : base(testContext)
    {
        _inszToCompare = TestContext.Request.Vertegenwoordigers[0].Insz;
    }

    [Fact]
    public void With_Context()
    {
        Response.Insz.Should().Be(_inszToCompare);
    }

    public override Func<IApiSetup, VerenigingenPerInszResponse> GetResponse
        => setup => setup.AcmApiHost.GetVerenigingenPerInsz(_inszToCompare);
}
