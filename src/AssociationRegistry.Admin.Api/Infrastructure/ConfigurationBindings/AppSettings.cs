namespace AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;

public class AppSettings
{
    private string? _baseUrl;
    private string? _beheerApiBaseUrl;
    private string? _beheerProjectionHostBaseUrl;
    private string? _publicApiBaseUrl;
    private string? _publicProjectionHostBaseUrl;

    public string BeheerApiBaseUrl
    {
        get => _beheerApiBaseUrl?.TrimEnd(trimChar: '/') ?? string.Empty;
        set => _beheerApiBaseUrl = value;
    }

    public string BeheerProjectionHostBaseUrl
    {
        get => _beheerProjectionHostBaseUrl?.TrimEnd(trimChar: '/') ?? string.Empty;
        set => _beheerProjectionHostBaseUrl = value;
    }

    public string PublicApiBaseUrl
    {
        get => _publicApiBaseUrl?.TrimEnd(trimChar: '/') ?? string.Empty;
        set => _publicApiBaseUrl = value;
    }

    public string PublicProjectionHostBaseUrl
    {
        get => _publicProjectionHostBaseUrl?.TrimEnd(trimChar: '/') ?? string.Empty;
        set => _publicProjectionHostBaseUrl = value;
    }

    public string BaseUrl
    {
        get => _baseUrl?.TrimEnd(trimChar: '/') ?? string.Empty;
        set => _baseUrl = value;
    }

    public string Salt { get; set; } = null!;
    public ApiDocsSettings ApiDocs { get; set; } = new();
    public SearchSettings Search { get; set; } = new();

    public class ApiDocsSettings
    {
        public string Title { get; set; } = null!;
        public ContactSettings Contact { get; set; } = null!;

        public class ContactSettings
        {
            public string Name { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Url { get; set; } = null!;
        }
    }

    public class SearchSettings
    {
        public int MaxNumberOfSearchResults { get; set; } = 1000;
    }
}
