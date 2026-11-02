namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Publiek.DetailAuthentiekeVereniging;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Xunit;

/// <summary>De authentieke vereniging neemt de basisgegevens over die ze zelf nog niet had.</summary>
[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<PubliekVerenigingDetailResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<PubliekVerenigingDetailResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.PublicApiHost.GetPubliekDetail(
            _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
            _testContext.DubbeleVerenigingWerdSamengevoegd!.Sequence
        );

    [Fact]
    public void With_The_Own_KorteNaam_Kept() =>
        Response
            .Vereniging.KorteNaam.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensAuthentiekeVereniging.KorteNaam);

    [Fact]
    public void With_The_KorteBeschrijving_Of_The_Dubbele_Vereniging() =>
        Response
            .Vereniging.KorteBeschrijving.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.KorteBeschrijving);

    [Fact]
    public void With_The_Startdatum_Of_The_Dubbele_Vereniging() =>
        Response
            .Vereniging.Startdatum.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.Startdatum);

    [Fact]
    public void With_The_Doelgroep_Of_The_Dubbele_Vereniging()
    {
        var doelgroep = MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.Doelgroep;

        Response.Vereniging.Doelgroep.Minimumleeftijd.Should().Be(doelgroep.Minimumleeftijd);
        Response.Vereniging.Doelgroep.Maximumleeftijd.Should().Be(doelgroep.Maximumleeftijd);
    }
}
