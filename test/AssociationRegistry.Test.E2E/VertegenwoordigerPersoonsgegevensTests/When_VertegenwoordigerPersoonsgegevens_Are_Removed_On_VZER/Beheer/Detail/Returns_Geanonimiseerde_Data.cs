namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_VertegenwoordigerPersoonsgegevens_Are_Removed_On_VZER.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Formats;
using AssociationRegistry.Persoonsgegevens;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging.Bronnen;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Xunit;

[Collection(nameof(VertegenwoordigerZonderPersoonsgegevensTestCollection))]
public class Returns_Geanonimiseerde_Data : End2EndTest<DetailVerenigingResponse>
{
    private readonly VertegenwoordigerZonderPersoonsgegevensOnVZERScenarioTestContext _testContext;

    public Returns_Geanonimiseerde_Data(VertegenwoordigerZonderPersoonsgegevensOnVZERScenarioTestContext testContext) : base(testContext.ApiSetup)
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
    public async ValueTask WithNaamIsGewijzigd()
        => Response.Vereniging.Naam.Should().Be(_testContext.CommandRequest.Naam);

    [Fact]
    public async ValueTask WithGeanonimiseerdeVertegenwoordigers()
    {
        var vertegenwoordigers = _testContext.Scenario
                                              .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens
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
