using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
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
