namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Beheer.Detail.With_Header;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;

[Collection(nameof(RegistreerVerenigingMetRechtsperoonlijkheidCollection))]
public class Returns_Vereniging : End2EndTest<DetailVerenigingResponse>
{
    private readonly RegistreerVerenigingMetRechtsperoonlijkheidContext _testContext;
    public Returns_Vereniging(RegistreerVerenigingMetRechtsperoonlijkheidContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override DetailVerenigingResponse GetResponse(FullBlownApiSetup setup)
        => setup.AdminApiHost.GetBeheerDetailWithHeader(setup.SuperAdminHttpClient, _testContext.CommandResult.VCode,
                                                        _testContext.CommandResult.Sequence)
                .GetAwaiter().GetResult();

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
}
