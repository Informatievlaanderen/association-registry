using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AssociationRegistry.Public.Api.SearchVerenigingen.Examples;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Public.Api.ListVerenigingen;

using System;
using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
using Newtonsoft.Json;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class ListVerenigingenController : ApiController
{
    private readonly IVerenigingenRepository _verenigingenRepository;
    private ImmutableArray<Vereniging> verenigingen;

    public ListVerenigingenController(IVerenigingenRepository verenigingenRepository)
    {
        _verenigingenRepository = verenigingenRepository;
    }
/// <summary>
/// Vraag de lijst van verenigingen op.
/// </summary>
/// <param name="paginationQueryParams"></param>
/// <response code="200">Een lijst met de bevraagde verenigingen</response>
/// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ListVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ListVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]

    public async Task<IActionResult> List(
        [FromServices] ListVerenigingContext context,
        [FromQuery] PaginationQueryParams paginationQueryParams)
    {
        var paginationRequest = ExtractPaginationRequest(paginationQueryParams);
        var totalCount = await _verenigingenRepository.TotalCount();
        verenigingen = await _verenigingenRepository.List();
        return Ok(
            new ListVerenigingenResponse(
                context,
                verenigingen.Skip(paginationRequest.Offset).Take(paginationRequest.Limit).ToImmutableArray(),
                new Metadata(
                    new Pagination(
                        totalCount,
                        paginationRequest.Offset,
                        paginationRequest.Limit
                    )
                )
            )
        );
    }

    private static PaginationRequest ExtractPaginationRequest(PaginationQueryParams paginationQueryParams) =>
        new(paginationQueryParams.Offset,
            Math.Min(paginationQueryParams.Limit, Constants.DefaultLimit));
}
