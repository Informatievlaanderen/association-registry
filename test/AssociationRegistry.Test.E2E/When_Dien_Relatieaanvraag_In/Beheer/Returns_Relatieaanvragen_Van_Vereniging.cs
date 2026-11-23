namespace AssociationRegistry.Test.E2E.When_Dien_Relatieaanvraag_In.Beheer;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

/// <summary>The relatieaanvraag is found for the koepel and for the vereniging, and can be filtered on status.</summary>
[Collection(nameof(DienRelatieaanvraagInCollection))]
public class Returns_Relatieaanvragen_Van_Vereniging
{
    private readonly DienRelatieaanvraagInContext _testContext;
    private readonly FullBlownApiSetup _setup;

    public Returns_Relatieaanvragen_Van_Vereniging(DienRelatieaanvraagInContext testContext)
    {
        _testContext = testContext;
        _setup = testContext.ApiSetup;
    }

    private RequestParameters WithExpectedSequence() =>
        new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence);

    [Fact]
    public async ValueTask For_The_Koepel()
    {
        var response = await _setup.AdminApiHost.GetRelatieaanvragen(
            _setup.AdminHttpClient,
            _testContext.Scenario.KoepelVCode,
            headers: WithExpectedSequence()
        );

        response.Relatieaanvragen.Should().ContainSingle().Which.AanvraagId.Should().Be(_testContext.AanvraagId);
    }

    [Fact]
    public async ValueTask For_The_Vereniging()
    {
        var response = await _setup.AdminApiHost.GetRelatieaanvragen(
            _setup.AdminHttpClient,
            _testContext.Scenario.VerenigingVCode,
            headers: WithExpectedSequence()
        );

        response.Relatieaanvragen.Should().ContainSingle().Which.AanvraagId.Should().Be(_testContext.AanvraagId);
    }

    [Fact]
    public async ValueTask With_Status_Aangevraagd()
    {
        var response = await _setup.AdminApiHost.GetRelatieaanvragen(
            _setup.AdminHttpClient,
            _testContext.Scenario.VerenigingVCode,
            status: "aangevraagd",
            headers: WithExpectedSequence()
        );

        response.Relatieaanvragen.Should().ContainSingle().Which.AanvraagId.Should().Be(_testContext.AanvraagId);
    }

    [Fact]
    public async ValueTask With_Another_Status()
    {
        var response = await _setup.AdminApiHost.GetRelatieaanvragen(
            _setup.AdminHttpClient,
            _testContext.Scenario.VerenigingVCode,
            status: "Bevestigd",
            headers: WithExpectedSequence()
        );

        response.Relatieaanvragen.Should().BeEmpty();
    }
}
