namespace AssociationRegistry.Public.Api.ListVerenigingen;

using Constants;

public record Metadata(Pagination Pagination);

public record Pagination(int TotalCount, int Offset, int Limit);

public record PaginationQueryParams(int Offset = PagingConstants.DefaultOffset, int Limit = PagingConstants.DefaultLimit);
