namespace AssociationRegistry.Test.E2E.VertegenwoordigerPersoonsgegevensTests.When_VertegenwoordigerPersoonsgegevens_Are_Removed_On_KBO.Beheer.Detail;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using Contracts.JsonLdContext;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Persoonsgegevens;
using Vereniging.Bronnen;
using Xunit;

[Collection(nameof(VertegenwoordigerZonderPersoonsgegevensOnKBOTestCollection))]
public class Returns_Gewijzigde_Naam : End2EndTest<DetailVerenigingResponse>
{
    private readonly VertegenwoordigerZonderPersoonsgegevensOnKBOTestContext _testContext;

    public Returns_Gewijzigde_Naam(VertegenwoordigerZonderPersoonsgegevensOnKBOTestContext testContext) : base(testContext.ApiSetup)
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
    public async ValueTask With_FeitelijkeVereniging()
        => Response.Vereniging.Roepnaam.Should().Be(_testContext.CommandRequest.Roepnaam);

    [Fact]
    public async ValueTask With_Geanonimeseerde_Vertegenwoordigers()
    {
        var vertegenwoordigerId = _testContext.Scenario
                                              .VertegenwoordigerWerdToegevoegdVanuitKBOZonderPersoonsgegevensToKeep
                                              .VertegenwoordigerId;

        Response.Vereniging.Vertegenwoordigers.Should().BeEquivalentTo([
        new Vertegenwoordiger()
        {
               type = JsonLdType.Vertegenwoordiger.Type,
               id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(_testContext.VCode,
                                                                    vertegenwoordigerId.ToString()),
               VertegenwoordigerId = vertegenwoordigerId,
               Bron = Bron.KBO,
               Email = string.Empty,
               Insz = WellKnownAnonymousFields.Geanonimiseerd,
               Achternaam = WellKnownAnonymousFields.Geanonimiseerd,
               Mobiel = string.Empty,
               Roepnaam = string.Empty,
               Rol = string.Empty,
               SocialMedia = string.Empty,
               Telefoon = string.Empty,
               Voornaam = WellKnownAnonymousFields.Geanonimiseerd,
               PrimairContactpersoon = false,
               VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
               {
                   Email = string.Empty,
                   Mobiel = string.Empty,
                   SocialMedia = string.Empty,
                   Telefoon = string.Empty,
                   id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(
                       _testContext.VCode,
                       vertegenwoordigerId.ToString()),
                   type = JsonLdType.VertegenwoordigerContactgegeven.Type,
               },
        }]);
    }
}
