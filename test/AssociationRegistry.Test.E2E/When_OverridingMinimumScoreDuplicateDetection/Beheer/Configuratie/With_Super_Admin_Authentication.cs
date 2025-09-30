namespace AssociationRegistry.Test.E2E.When_OverridingMinimumScoreDuplicateDetection.Beheer.Configuratie;

using Admin.Api.WebApi.Administratie.Configuratie;
using Framework.AlbaHost;
using Framework.ApiSetup;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using DecentraalBeheer.Vereniging.DubbelDetectie;
using FluentAssertions;
using System.Net;
using Xunit;

[Collection(nameof(OverridingMinimumScoreDuplicateDetectionCollection))]
public class With_Super_Admin_Authentication {
    private readonly FullBlownApiSetup _apiSetup;

    public With_Super_Admin_Authentication(FullBlownApiSetup apiSetup)
    {
        _apiSetup = apiSetup;
    }

    [Fact]
    public async ValueTask Returns_The_Updated_Value_On_Next_Get()
    {
        var fixture = new Fixture();
        var _waarde = fixture.Create<double>();

        await CheckValuesBeforeChange();

        var request = new OverrideMinimumScoreDuplicateDetectionRequest()
        {
            Waarde = _waarde,
        };

        var response = await _apiSetup.AdminApiHost.PostMinimumScoreDuplicateDetectionOverride(request, _apiSetup.SuperAdminHttpClient);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var overrideMinimumScore = await _apiSetup.AdminApiHost.GetMinimumScoreDuplicateDetectionOverride(_apiSetup.SuperAdminHttpClient);

        overrideMinimumScore.ActualMinimumScore.Should().Be(_waarde);
        overrideMinimumScore.MinimumScoreOverride.Should().Be(_waarde.ToString());
        overrideMinimumScore.DefaultMinimumScore.Should().Be(MinimumScore.Default.Value);
    }

    private async Task CheckValuesBeforeChange()
    {
        var overrideMinimumScore = await _apiSetup.AdminApiHost.GetMinimumScoreDuplicateDetectionOverride(_apiSetup.SuperAdminHttpClient);

        overrideMinimumScore.ActualMinimumScore.Should().Be(MinimumScore.Default.Value);
        overrideMinimumScore.MinimumScoreOverride.Should().Be(string.Empty);
        overrideMinimumScore.DefaultMinimumScore.Should().Be(MinimumScore.Default.Value);
    }
}
