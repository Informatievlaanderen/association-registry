namespace AssociationRegistry.Hosts.Configuration.ConfigurationBindings;

using System;

public class AppSettings
{
    private string? _baseUrl;
    private string? _beheerProjectionHostBaseUrl;
    private string? _publicApiBaseUrl;
    private string? _publicProjectionHostBaseUrl;
    private string? _kboSyncQueueUrl;
    private string? _kboSyncQueueName;
    private string? _readdressQueueUrl;

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

    public string KboSyncQueueUrl
    {
        get => _kboSyncQueueUrl?.TrimEnd(trimChar: '/') ?? string.Empty;
        set => _kboSyncQueueUrl = value;
    }

    public string KboSyncQueueName
    {
        get => _kboSyncQueueName ?? string.Empty;
        set => _kboSyncQueueName = value;
    }

    public string ReaddressQueueUrl
    {
        get => _readdressQueueUrl?.TrimEnd(trimChar: '/') ?? string.Empty;
        set => _readdressQueueUrl = value;
    }

    public string[] SuperAdminClientIds { get; set; } = Array.Empty<string>();
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
