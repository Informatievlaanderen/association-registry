using System.Collections.Immutable;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Mvc;

namespace AssociationRegistry.Public.Api.ListVerenigingen;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class ListVerenigingenController : ApiController
{
    private readonly IVerenigingenRepository _verenigingenRepository;

    public ListVerenigingenController(IVerenigingenRepository verenigingenRepository)
    {
        _verenigingenRepository = verenigingenRepository;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        return Ok(new ListVerenigingenResponse(await _verenigingenRepository.List()));
    }
}

public interface IVerenigingenRepository
{
    Task<ImmutableArray<Vereniging>> List();
}

public record ListVerenigingenResponse(ImmutableArray<Vereniging> Verenigingen);

public record Vereniging(
    string VCode,
    string Naam,
    string KorteNaam,
    ImmutableArray<Locatie> Locaties,
    ImmutableArray<string> Activiteiten);

public record Locatie(
    string Postcode,
    string Gemeentenaam);
