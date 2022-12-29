namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using Constants;

public record Metadata(Pagination Pagination);

public record Pagination(long TotalCount, int Offset, int Limit);

public record PaginationQueryParams(int Offset = PagingConstants.DefaultOffset, int Limit = PagingConstants.DefaultLimit);
