namespace AssociationRegistry.Test.E2E.When_OverridingMinimumScoreDuplicateDetection.Beheer.Configuratie;

using Admin.Api.Administratie.Configuratie;
using AutoFixture;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using System.Net;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Without_Super_Admin_Authentication {
    private readonly FullBlownApiSetup _apiSetup;

    public Without_Super_Admin_Authentication(FullBlownApiSetup apiSetup)
    {
        _apiSetup = apiSetup;
    }

    [Fact]
    public async Task Returns_403()
    {
       var request = new OverrideMinimumScoreDuplicateDetectionRequest()
        {
            Waarde = new Fixture().Create<double>(),
        };

        var response = await _apiSetup.AdminApiHost.PostMinimumScoreDuplicateDetectionOverride(request, _apiSetup.AdminHttpClient);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
