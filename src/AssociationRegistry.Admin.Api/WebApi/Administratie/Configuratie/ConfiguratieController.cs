namespace AssociationRegistry.Admin.Api.WebApi.Administratie.Configuratie;

using Asp.Versioning;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Be.Vlaanderen.Basisregisters.Api;
using CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.Serialization;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("admin/config")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class ConfiguratieController : ApiController
{
    [HttpPost("minimumScoreDuplicateDetection")]
    public async Task<IActionResult> OverrideMinimumScoreDuplicateDetection(
        [FromBody] OverrideMinimumScoreDuplicateDetectionRequest? request,
        [FromServices] IDocumentSession session,
        [FromServices] IMemoryCache cache)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (request.Waarde is null)
        {
            session.Delete<SettingOverride>(SettingOverrideNames.ElasticSearch.MinimumScoreDuplicateDetection);
        }
        else
        {
            session.Store(new SettingOverride(SettingOverrideNames.ElasticSearch.MinimumScoreDuplicateDetection, request.Waarde.ToString()));
        }

        await session.SaveChangesAsync();

        cache.Remove(SettingOverrideNames.ElasticSearch.MinimumScoreDuplicateDetection);

        return Ok();
    }

    [HttpGet("minimumScoreDuplicateDetection")]
    public async Task<IActionResult> GetMinimumScoreDuplicateDetection(
        [FromServices] ElasticSearchOptionsSection elasticSearchOptionsSection,
        [FromServices] MinimumScore minimumScore,
        [FromServices] IDocumentSession session)
        => Ok(new MinimumScoreDuplicateDetectionOverrideResponse(
                  elasticSearchOptionsSection.MinimumScoreDuplicateDetection,
                  session.Query<SettingOverride>()
                         .FirstOrDefault(x => x.Key == SettingOverrideNames.ElasticSearch.MinimumScoreDuplicateDetection)?
                         .Value ?? string.Empty,
                  minimumScore.Value
              ));
}

[DataContract]
public record MinimumScoreDuplicateDetectionOverrideResponse(
    [property: DataMember]double DefaultMinimumScore,
    [property: DataMember]string MinimumScoreOverride,
    [property: DataMember]double ActualMinimumScore);
