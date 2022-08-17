namespace AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Microsoft.AspNetCore.Mvc;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("verenigingen")]
    [ApiExplorerSettings(GroupName = "Acm")]
    public class VerenigingenPerRijksregisternummerController : ApiController
    {
        private static readonly Dictionary<string, ImmutableArray<Vereniging>> Verenigingen =
            new Dictionary<string, ImmutableArray<Vereniging>>()
            {
                {
                    "7103", ImmutableArray.Create(
                        new Vereniging("V1234567", "FWA De vrolijke BA’s"),
                        new Vereniging("V7654321", "FWA De Bron"))
                },
                {
                    "9803", ImmutableArray.Create(
                        new Vereniging("V0000001", "De eenzame in de lijst"))
                }
            };


        /// <summary>
        /// Vraag de lijst van verenigingen voor een rijksregisternummer op.
        /// </summary>
        /// <param name="rijksregisternummer"></param>
        /// <response code="200">Als het rijksregisternummer gevonden is.</response>
        /// <response code="404">Als het rijksregisternummer niet gevonden kan worden.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet()]
        // [ProducesResponseType(typeof(AssociationResponse), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        // [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AssociationResponseExamples))]
        // [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(AssociationNotFoundResponseExamples))]
        // [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
        public async Task<IActionResult> Get(
            [FromQuery] string rijksregisternummer)
        {
            var minifiedRijksregisternummer = rijksregisternummer[..4];
            return Ok(Verenigingen[minifiedRijksregisternummer]);
        }
    }

    public record Vereniging(string Id, string Naam);
}
