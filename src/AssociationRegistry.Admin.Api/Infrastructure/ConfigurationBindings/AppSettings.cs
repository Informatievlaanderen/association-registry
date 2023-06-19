namespace AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;

public class AppSettings
{
    private string? _baseUrl;

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
