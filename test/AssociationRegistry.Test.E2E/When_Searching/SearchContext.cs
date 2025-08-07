namespace AssociationRegistry.Test.E2E.When_Searching;

using Admin.Schema.Detail;
using Admin.Schema.Search;
using Framework.ApiSetup;
using Framework.TestClasses;
using Elastic.Clients.Elasticsearch;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(SearchCollection))]
public class SearchCollection : ICollectionFixture<SearchContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class SearchContext : TestContextBase<SearchScenario, NullRequest>
{
    protected override SearchScenario InitializeScenario()
        => new();

    public SearchContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(SearchScenario scenario)
    {
        CommandResult = CommandResult<NullRequest>.NullCommandResult();


        // var result = ApiSetup.ElasticClient.Search<VerenigingZoekDocument>(s => s
        //                                                                       .Query(q => q
        //                                                                                 .Term(t => t
        //                                                                                     .Field(f => f.VCode.Suffix("keyword"))
        //                                                                                     .Value(scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd3.VCode)
        //                                                                                  )
        //                                                                        ));
        // var counter = 0;
        //
        // while (!result.Documents.Any() && counter < 10)
        // {
        //     counter++;
        //     await Task.Delay(500);
        //
        //     result = ApiSetup.ElasticClient.Search<VerenigingZoekDocument>(s => s
        //                                                                       .Query(q => q
        //                                                                                 .Term(t => t
        //                                                                                             .Field(f => f.VCode.Suffix("keyword"))
        //                                                                                             .Value(scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd3.VCode)
        //                                                                                  )
        //                                                                        ));
        //
        // }
        // if(!result.Documents.Any())
        //     throw new InvalidOperationException("No documents found in ElasticSearch");
    }
}
