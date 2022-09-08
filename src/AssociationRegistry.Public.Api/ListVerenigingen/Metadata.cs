namespace AssociationRegistry.Public.Api.ListVerenigingen;

public record Metadata(Pagination Pagination);

public record Pagination(int TotalCount, int Offset, int Limit);

public record PaginationQueryParams(int Offset = Constants.DefaultOffset, int Limit = Constants.DefaultLimit);
