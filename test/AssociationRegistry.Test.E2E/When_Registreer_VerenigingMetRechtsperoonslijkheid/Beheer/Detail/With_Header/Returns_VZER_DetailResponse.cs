namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Beheer.Detail.With_Header;

using Admin.Api;
using Admin.Api.Verenigingen.Detail.ResponseModels;
using Admin.Api.Verenigingen.Registreer.MetRechtspersoonlijkheid.RequestModels;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_VZER_DetailResponse :
    End2EndTest<RegistreerVerenigingMetRechtsperoonlijkheidTestContext, RegistreerVerenigingUitKboRequest, DetailVerenigingResponse>
{
    public Returns_VZER_DetailResponse(RegistreerVerenigingMetRechtsperoonlijkheidTestContext testContext) : base(testContext)
    {
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                               compareConfig: new ComparisonConfig
                                                                   { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public async ValueTask WithKboVereniging()
        => Response.Vereniging.Verenigingssubtype.Should().BeNull();

    public override Func<IApiSetup, DetailVerenigingResponse> GetResponse
    {
        get { return setup =>
        {
            var logger = setup.AdminApiHost.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("EXECUTING GET REQUEST");

            return setup.AdminApiHost.GetBeheerDetailWithHeader(setup.SuperAdminHttpClient, TestContext.RequestResult.VCode,
                                                                TestContext.RequestResult.Sequence)
                        .GetAwaiter().GetResult();
        }; }
    }
}
