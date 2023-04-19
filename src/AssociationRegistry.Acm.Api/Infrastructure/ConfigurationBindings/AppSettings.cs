namespace AssociationRegistry.Acm.Api.Infrastructure.ConfigurationBindings;

public class AppSettings
{
    private string? _baseUrl;
    public string BaseUrl
    {
        get => _baseUrl?.TrimEnd(trimChar: '/') ?? string.Empty;
        set => _baseUrl = value;
    }

    public ApiDocsSettings ApiDocs { get; set; } = null!;

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
}
