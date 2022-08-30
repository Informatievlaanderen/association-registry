using System.Collections.Immutable;
using AssociationRegistry.Public.Api.SearchVerenigingen.Examples;

namespace AssociationRegistry.Public.Api.VerenigingenPerRijksregisternummer;

using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class SearchVerenigingenController : ApiController
{
    /// <summary>
    /// Vraag de lijst van verenigingen voor een rijksregisternummer op.
    /// </summary>
    /// <response code="200">Er kwam geen fout voor.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Get() =>
        await Task.FromResult<IActionResult>(Ok(new SearchVerenigingenResponse(ImmutableArray.Create(
                new Vereniging(
                    "V1234567",
                    "FWA De vrolijke BA’s",
                    "DVB",
                    ImmutableArray.Create(
                        new Locatie(
                            "Correspondentieadres",
                            "https://data.vlaanderen.be/id/adres/2272122",
                            "Bombardonstraat 245, 1770 Liedekerke"),
                        new Locatie(
                            "Plaats van de activiteiten",
                            "https://data.vlaanderen.be/id/adres/4855889",
                            "Kerkstraat 1, 1770 Liedekerke"),
                        new Locatie(
                            "Plaats van de activiteiten",
                            "https://data.vlaanderen.be/id/adres/1115009",
                            "Kerkstraat 1, 9473 Denderleeuw")
                    ),
                    ImmutableArray.Create(
                        new Activiteit(123, "Badminton"),
                        new Activiteit(456, "Tennis"))),
                new Vereniging(
                    "V7654321",
                    "FWA De Bron",
                    string.Empty,
                    ImmutableArray.Create(
                        new Locatie(
                            "Plaats van de activiteiten",
                            "https://data.vlaanderen.be/id/gemeente/44021",
                            "Gent")),
                    ImmutableArray.Create(
                        new Activiteit(456, "Tennis"))
                ),
                new Vereniging(
                    "V0000001",
                    "De eenzame in de lijst",
                    "Me, myself and I",
                    ImmutableArray<Locatie>.Empty,
                    ImmutableArray.Create(
                        new Activiteit(123, "Badminton"))
                    )
            )
        )));
}
