namespace AssociationRegistry.Test.Admin.AddressSync.When_Fetching_Te_Synchroniseren_Locaties;

using AssociationRegistry.Admin.AddressSync.Fetchers;
using FluentAssertions;
using Integrations.Grar.Clients;
using Integrations.Grar.Integration.Messages;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class Given_No_LocatieLookupDocumenten
{
    private readonly TeSynchroniserenLocatieAdresMessage[] _locaties;

    public Given_No_LocatieLookupDocumenten()
    {
        var store = TestDocumentStoreFactory.CreateAsync(nameof(Given_No_LocatieLookupDocumenten))
                                            .GetAwaiter().GetResult();

        using var session = store.LightweightSession();

        var sut = new TeSynchroniserenLocatiesFetcher(
            Mock.Of<IGrarClient>(),
            NullLogger<TeSynchroniserenLocatiesFetcher>.Instance
        );

        _locaties = sut.GetTeSynchroniserenLocaties(session, CancellationToken.None)
                       .GetAwaiter().GetResult().ToArray();
    }

    [Fact]
    public void Then_Locaties_Should_Be_Empty()
    {
        _locaties.Should().BeEmpty();
    }
}
