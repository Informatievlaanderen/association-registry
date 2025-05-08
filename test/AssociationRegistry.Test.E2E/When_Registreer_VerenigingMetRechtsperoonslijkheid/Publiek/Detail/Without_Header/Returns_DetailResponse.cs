namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Publiek.Detail.Without_Header;

using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_DetailResponse :
    End2EndTest<RegistreerVerenigingMetRechtsperoonlijkheidTestContext, RegistreerVerenigingUitKboRequest, PubliekVerenigingDetailResponse>
{
    public Returns_DetailResponse(RegistreerVerenigingMetRechtsperoonlijkheidTestContext testContext) : base(testContext)
    {
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/publiek/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                               compareConfig: new ComparisonConfig
                                                                   { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Vereniging.Verenigingssubtype.Should().BeNull();

    public override Func<IApiSetup, PubliekVerenigingDetailResponse> GetResponse
        => setup => setup.PublicApiHost.GetPubliekDetail(TestContext.RequestResult.VCode);
}
