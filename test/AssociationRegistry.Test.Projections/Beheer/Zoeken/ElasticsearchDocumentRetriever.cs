namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Elastic.Clients.Elasticsearch;
using Polly;
using System;
using System.Threading.Tasks;

public class ElasticsearchDocumentRetriever<TDocument> where TDocument : class
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly RetryConfiguration _retryConfiguration;

    public ElasticsearchDocumentRetriever(
        ElasticsearchClient elasticClient,
        RetryConfiguration? retryConfiguration = null)
    {
        _elasticClient = elasticClient;
        _retryConfiguration = retryConfiguration ?? RetryConfiguration.Default;
    }

    public async Task<TDocument> GetDocumentAsync(string documentId)
    {
        var retryContext = new RetryContext();

        var retryPolicy = CreateRetryPolicy(retryContext, documentId);

        var getResponse = await retryPolicy.ExecuteAsync(async () =>
        {
            if (retryContext.ShouldRefreshBeforeRetry)
            {
                await RefreshIndicesAsync();
            }

            return await _elasticClient.GetAsync<TDocument>(documentId);
        });

        return ValidateAndExtractDocument(getResponse, documentId, retryContext);
    }

    private IAsyncPolicy<GetResponse<TDocument>> CreateRetryPolicy(RetryContext retryContext, string documentId)
    {
        return Policy
            .HandleResult<GetResponse<TDocument>>(result =>
                ShouldRetry(result, retryContext))
            .WaitAndRetryAsync(
                _retryConfiguration.MaxRetries,
                _retryConfiguration.GetRetryDelay,
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    OnRetry(retryContext, timespan, retryCount);
                });
    }

    private bool ShouldRetry(GetResponse<TDocument> response, RetryContext retryContext)
    {
        retryContext.IncrementAttempts();

        if (response == null)
        {
            retryContext.SetLastError("Response was null");
            return true;
        }

        if (!response.Found)
        {
            retryContext.SetLastError($"Document not found");
            return true;
        }

        if (!response.IsValidResponse)
        {
            retryContext.SetLastError($"Invalid response: {response.DebugInformation}");
            return true;
        }

        return false;
    }

    private void OnRetry(RetryContext retryContext, TimeSpan delay, int retryCount)
    {
        Console.WriteLine($"Retry {retryCount}: Waiting {delay.TotalMilliseconds}ms. {retryContext.LastError}");
    }

    private async Task RefreshIndicesAsync()
    {
        await _elasticClient.Indices.RefreshAsync(Indices.All);
        await Task.Delay(_retryConfiguration.RefreshDelay);
    }

    private TDocument ValidateAndExtractDocument(
        GetResponse<TDocument> response,
        string documentId,
        RetryContext retryContext)
    {
        if (response?.Source == null)
        {
            throw new DocumentRetrievalException(
                documentId,
                retryContext.Attempts,
                retryContext.LastError);
        }

        return response.Source;
    }

    private class RetryContext
    {
        public int Attempts { get; private set; }
        public string LastError { get; private set; } = string.Empty;
        public bool ShouldRefreshBeforeRetry => Attempts > 0;

        public void IncrementAttempts() => Attempts++;
        public void SetLastError(string error) => LastError = error;
    }
}

public class RetryConfiguration
{
    public int MaxRetries { get; init; }
    public TimeSpan RefreshDelay { get; init; }
    public Func<int, TimeSpan> GetRetryDelay { get; init; }

    public static RetryConfiguration Default => new()
    {
        MaxRetries = 10,
        RefreshDelay = TimeSpan.FromMilliseconds(100),
        GetRetryDelay = retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt - 3))
    };
}

public class DocumentRetrievalException : Exception
{
    public string DocumentId { get; }
    public int Attempts { get; }
    public string LastError { get; }

    public DocumentRetrievalException(string documentId, int attempts, string lastError)
        : base($"Failed to retrieve document with ID: {documentId} after {attempts} attempts. " +
               $"Last error: {lastError}. " +
               $"This may indicate an Elasticsearch mapping issue or projection delay.")
    {
        DocumentId = documentId;
        Attempts = attempts;
        LastError = lastError;
    }
}
