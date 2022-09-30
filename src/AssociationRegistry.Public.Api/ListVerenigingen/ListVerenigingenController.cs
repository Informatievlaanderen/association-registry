using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;

namespace AssociationRegistry.Public.Api.ListVerenigingen;

using Constants;

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

    /// <summary>
    /// Vraag de lijst van verenigingen op.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="paginationQueryParams"></param>
    /// <response code="200">Een lijst met de bevraagde verenigingen</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ListVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ListVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.JsonLd)]
    public async Task<IActionResult> List(
        [FromServices] AppSettings appsettings,
        [FromQuery] PaginationQueryParams paginationQueryParams)
    {
        var paginationRequest = ExtractPaginationRequest(paginationQueryParams);
        var totalCount = await _verenigingenRepository.TotalCount();
        var listVerenigingenResponse = new ListVerenigingenResponse(
            $"{appsettings.AssociationRegistryUri}v1/contexten/list-verenigingen-context.json",
            (await _verenigingenRepository.List())
            .Skip(paginationRequest.Offset)
            .Take(paginationRequest.Limit)
            .Select(ListVerenigingenResponseItem.FromVereniging)
            .ToImmutableArray(),
            new Metadata(
                new Pagination(
                    totalCount,
                    paginationRequest.Offset,
                    paginationRequest.Limit
                )
            )
        );
        return Ok(
            listVerenigingenResponse
        );
    }

    private static PaginationRequest ExtractPaginationRequest(PaginationQueryParams paginationQueryParams) =>
        new(paginationQueryParams.Offset,
            Math.Min(paginationQueryParams.Limit, PagingConstants.DefaultLimit));
}
