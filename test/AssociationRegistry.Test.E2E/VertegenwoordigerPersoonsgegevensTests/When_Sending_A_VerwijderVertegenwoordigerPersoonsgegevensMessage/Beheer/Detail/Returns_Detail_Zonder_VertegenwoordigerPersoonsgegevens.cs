namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_Sending_A_VerwijderVertegenwoordigerPersoonsgegevensMessage.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Vereniging.Bronnen;
using Xunit;

[Collection(nameof(VerwijderVertegenwoordigerPersoonsgegevensCollection))]
public class Returns_Detail_Zonder_VertegenwoordigerPersoonsgegevens : End2EndTest<DetailVerenigingResponse>
{
    private readonly VerwijderVertegenwoordigerPersoonsgegevensTestContext _testContext;

    public Returns_Detail_Zonder_VertegenwoordigerPersoonsgegevens(VerwijderVertegenwoordigerPersoonsgegevensTestContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost.GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode,new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence));

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
    public async ValueTask WithFeitelijkeVereniging()
    {
        var verwijderdeVertegenwoordiger =
            _testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers[0];
        const string nietMeerGekend = "Niet meer gekend";

        Response.Vereniging.Vertegenwoordigers[0].Should().BeEquivalentTo(new Vertegenwoordiger()
        {
            id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(
                _testContext.VCode, verwijderdeVertegenwoordiger.VertegenwoordigerId.ToString()),
            type = JsonLdType.Vertegenwoordiger.Type,

            VertegenwoordigerId = verwijderdeVertegenwoordiger.VertegenwoordigerId,
            Voornaam = nietMeerGekend,
            Achternaam = nietMeerGekend,
            Roepnaam = nietMeerGekend,
            Rol = nietMeerGekend,
            Email = nietMeerGekend,
            Telefoon = nietMeerGekend,
            Insz = nietMeerGekend,
            Mobiel = nietMeerGekend,
            SocialMedia = nietMeerGekend,
            PrimairContactpersoon = verwijderdeVertegenwoordiger.IsPrimair,
            Bron = Bron.Initiator,
            VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
            {
                id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(
                    _testContext.VCode, verwijderdeVertegenwoordiger.VertegenwoordigerId.ToString()),
                type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                IsPrimair = verwijderdeVertegenwoordiger.IsPrimair,
                Email = nietMeerGekend,
                Telefoon = nietMeerGekend,
                Mobiel = nietMeerGekend,
                SocialMedia = nietMeerGekend,
            },
        });
    }
}
