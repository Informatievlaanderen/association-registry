namespace AssociationRegistry.Test.E2E.When_OverridingMinimumScoreDuplicateDetection.Beheer.Detail;

using Admin.Api.Administratie.Configuratie;
using Alba;
using AssociationRegistry.Admin.Api;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.Comparison;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Bronnen;
using AutoFixture;
using Azure;
using Azure.Core;
using DuplicateVerenigingDetection;
using FluentAssertions;
using JasperFx.Core;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NodaTime;
using System.Net;
using When_Registreer_FeitelijkeVereniging;
using Xunit;
using Contactgegeven = Admin.Api.Verenigingen.Detail.ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = Vereniging.HoofdactiviteitVerenigingsloket;
using Locatie = Admin.Api.Verenigingen.Detail.ResponseModels.Locatie;
using VerenigingStatus = Admin.Schema.Constants.VerenigingStatus;
using Vertegenwoordiger = Admin.Api.Verenigingen.Detail.ResponseModels.Vertegenwoordiger;
using Werkingsgebied = Vereniging.Werkingsgebied;

[Collection(FullBlownApiCollection.Name)]
public class Returns_The_Updated_Value_On_Next_Get {
    private readonly FullBlownApiSetup _apiSetup;

    public Returns_The_Updated_Value_On_Next_Get(FullBlownApiSetup apiSetup)
    {
        _apiSetup = apiSetup;
    }

    [Fact]
    public async Task Execute()
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
