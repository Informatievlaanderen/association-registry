namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingZonderEigenRechtspersoonlijkheid.Publiek.Zoeken.With_Geotags;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Xunit;

[Collection(nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCollection))]
public class Returns_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext _testContext;

    public Returns_ZoekResponse(RegistreerVerenigingZonderEigenRechtspersoonlijkheidContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.PublicApiHost.GetPubliekZoekenWithHeader(setup.SuperAdminHttpClient, $"vCode:{_testContext.VCode} AND" +
                                                                                      $" geotags.identificatie:BE24123027 AND geotags.identificatie:BE24 AND geotags.identificatie:{_testContext.CommandRequest.Locaties.First().Adres.Postcode}", _testContext.CommandResult.Sequence);

    [Fact]
    public async ValueTask WithFeitelijkeVereniging()
        => Response.Verenigingen
                   .SingleOrDefault(x => x.VCode == _testContext.VCode).Should().NotBeNull();
}
