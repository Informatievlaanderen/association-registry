namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_VertegenwoordigerPersoonsgegevens_Are_Removed_On_FV.Beheer.Detail;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Contracts.JsonLdContext;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Persoonsgegevens;
using Vereniging.Bronnen;
using Xunit;

[Collection(nameof(VertegenwoordigerZonderPersoonsgegevensOnFVTestCollection))]
public class Returns_Geanonimiseerde_Data : End2EndTest<DetailVerenigingResponse>
{
    private readonly VertegenwoordigerZonderPersoonsgegevensOnFVScenarioTestContext _testContext;

    public Returns_Geanonimiseerde_Data(VertegenwoordigerZonderPersoonsgegevensOnFVScenarioTestContext testContext) : base(testContext.ApiSetup)
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
    public async ValueTask With_Naam_Is_Gewijzigd()
        => Response.Vereniging.Naam.Should().Be(_testContext.CommandRequest.Naam);

    [Fact]
    public async ValueTask With_Geanonimiseerde_Vertegenwoordigers()
    {
        var vertegenwoordigers = _testContext.Scenario
        .FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens

        .Vertegenwoordigers.Select(x=> new Vertegenwoordiger()
        {
                                                   type = JsonLdType.Vertegenwoordiger.Type,
                                                   id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(_testContext.VCode,
                                                       x.VertegenwoordigerId.ToString()),
                                                   VertegenwoordigerId = x.VertegenwoordigerId,
                                                   Bron = Bron.Initiator,
                                                   Email = WellKnownAnonymousFields.Geanonimiseerd,
                                                   Insz = WellKnownAnonymousFields.Geanonimiseerd,
                                                   Achternaam = WellKnownAnonymousFields.Geanonimiseerd,
                                                   Mobiel = WellKnownAnonymousFields.Geanonimiseerd,
                                                   Roepnaam = WellKnownAnonymousFields.Geanonimiseerd,
                                                   Rol = WellKnownAnonymousFields.Geanonimiseerd,
                                                   SocialMedia = WellKnownAnonymousFields.Geanonimiseerd,
                                                   Telefoon = WellKnownAnonymousFields.Geanonimiseerd,
                                                   Voornaam = WellKnownAnonymousFields.Geanonimiseerd,
                                                   PrimairContactpersoon = x.IsPrimair,
                                                   VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
                                                   {
                                                       Email = WellKnownAnonymousFields.Geanonimiseerd,
                                                       Mobiel = WellKnownAnonymousFields.Geanonimiseerd,
                                                       SocialMedia = WellKnownAnonymousFields.Geanonimiseerd,
                                                       Telefoon = WellKnownAnonymousFields.Geanonimiseerd,
                                                       id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(
                                                           _testContext.VCode,
                                                           x.VertegenwoordigerId.ToString()),
                                                       type = JsonLdType.VertegenwoordigerContactgegeven.Type,
                                                   },
                                               });

        Response.Vereniging.Vertegenwoordigers.Should().BeEquivalentTo(vertegenwoordigers);
    }
}
