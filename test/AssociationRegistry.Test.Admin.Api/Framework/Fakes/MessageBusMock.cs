#pragma warning disable CS1998
namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using Wolverine;

public class MessageBusMock : IMessageBus
{
    public async Task InvokeAsync(object message, CancellationToken cancellation = new(), TimeSpan? timeout = null)
        => throw new NotImplementedException();

    public async Task<T> InvokeAsync<T>(object message, CancellationToken cancellation = new(), TimeSpan? timeout = null)
        => throw new NotImplementedException();

    public async Task InvokeForTenantAsync(
        string tenantId,
        object message,
        CancellationToken cancellation = new(),
        TimeSpan? timeout = null)
        => throw new NotImplementedException();

    public async Task<T> InvokeForTenantAsync<T>(
        string tenantId,
        object message,
        CancellationToken cancellation = new(),
        TimeSpan? timeout = null)
        => throw new NotImplementedException();

    public IDestinationEndpoint EndpointFor(string endpointName)
        => throw new NotImplementedException();

    public IDestinationEndpoint EndpointFor(Uri uri)
        => throw new NotImplementedException();

    public IReadOnlyList<Envelope> PreviewSubscriptions(object message)
        => throw new NotImplementedException();

    public async ValueTask SendAsync<T>(T message, DeliveryOptions? options = null)
        => throw new NotImplementedException();

    public async ValueTask PublishAsync<T>(T message, DeliveryOptions? options = null)
        => throw new NotImplementedException();

    public async ValueTask BroadcastToTopicAsync(string topicName, object message, DeliveryOptions? options = null)
        => throw new NotImplementedException();

    public string? TenantId { get; set; }
}
