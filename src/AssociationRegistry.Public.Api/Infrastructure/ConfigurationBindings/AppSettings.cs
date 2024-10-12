namespace AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;

public class AppSettings
{
    private string? _baseUrl;
    public string OrganisationRegistryUri { get; set; } = null!;

    public string BaseUrl
    {
        get => _baseUrl?.TrimEnd(trimChar: '/') ?? string.Empty;
        set => _baseUrl = value;
    }

    public string NewsletterUrl { get; set; } = null!;
    public string ApiKeyRequestFormUrl { get; set; } = null!;
    public ApiDocsSettings ApiDocs { get; set; } = null!;
    public SearchSettings Search { get; set; } = new();
    public PubliqSettings Publiq { get; set; } = null!;

    public class ApiDocsSettings
    {
        public string Title { get; set; } = null!;
        public LicenseSettings License { get; set; } = null!;
        public ContactSettings Contact { get; set; } = null!;

        public class LicenseSettings
        {
            public string Name { get; set; } = null!;
            public string Url { get; set; } = null!;
        }

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

    public class PubliqSettings
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public bool UseLocalstack { get; set; }
    }
}
