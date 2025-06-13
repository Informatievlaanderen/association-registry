namespace AssociationRegistry.Test.Public.Api.Mapping.When_Adding_A_Hoofdactiviteit_To_A_Query;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Api.Verenigingen.Search;
using FluentAssertions;
using Xunit;

public class Given_The_Original_Query
{
    private const string BaseUrl = "http://base";
    private const string EenHoofdActiviteitCode = "BWWC";

    public class Given_An_Empty_OriginalQuery
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenResponseMapper.AddHoofdactiviteitToQuery(
                new AppSettings { BaseUrl = BaseUrl }, EenHoofdActiviteitCode, originalQuery: "", Array.Empty<string>());

            newQuery.Should().Be($"{BaseUrl}/v1/verenigingen/zoeken?q=&facets.hoofdactiviteitenVerenigingsloket={EenHoofdActiviteitCode}");
        }
    }

    public class Given_An_OriginalQuery_That_Does_Not_Contain_Hoofdactiviteiten
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenResponseMapper.AddHoofdactiviteitToQuery(
                new AppSettings { BaseUrl = BaseUrl }, EenHoofdActiviteitCode, originalQuery: "oudenaarde", Array.Empty<string>());

            newQuery.Should()
                    .Be($"{BaseUrl}/v1/verenigingen/zoeken?q=oudenaarde&facets.hoofdactiviteitenVerenigingsloket={EenHoofdActiviteitCode}");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_Only_A_Star
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenResponseMapper.AddHoofdactiviteitToQuery(
                new AppSettings { BaseUrl = BaseUrl }, EenHoofdActiviteitCode, originalQuery: "*", Array.Empty<string>());

            newQuery.Should().Be($"{BaseUrl}/v1/verenigingen/zoeken?q=*&facets.hoofdactiviteitenVerenigingsloket={EenHoofdActiviteitCode}");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_1_Hoofdactiviteit_And_Nothing_Else
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery =
                SearchVerenigingenResponseMapper.AddHoofdactiviteitToQuery(new AppSettings { BaseUrl = BaseUrl }, EenHoofdActiviteitCode,
                                                                           originalQuery: "", new[] { "SPRT" });

            newQuery.Should()
                    .Be($"{BaseUrl}/v1/verenigingen/zoeken?q=&facets.hoofdactiviteitenVerenigingsloket=SPRT,{EenHoofdActiviteitCode}");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_2_Hoofdactiviteiten_And_Nothing_Else
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenResponseMapper.AddHoofdactiviteitToQuery(
                new AppSettings { BaseUrl = BaseUrl }, EenHoofdActiviteitCode, originalQuery: "mijn vereniging", new[] { "SPRT", "DINT" });

            newQuery.Should()
                    .Be(
                         $"{BaseUrl}/v1/verenigingen/zoeken?q=mijn vereniging&facets.hoofdactiviteitenVerenigingsloket=SPRT,DINT,{EenHoofdActiviteitCode}");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_1_Hoofdactiviteit_And_Something_Else
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenResponseMapper.AddHoofdactiviteitToQuery(
                new AppSettings { BaseUrl = BaseUrl }, EenHoofdActiviteitCode, originalQuery: "oudenaarde", new[] { "SPRT" });

            newQuery.Should()
                    .Be(
                         $"{BaseUrl}/v1/verenigingen/zoeken?q=oudenaarde&facets.hoofdactiviteitenVerenigingsloket=SPRT,{EenHoofdActiviteitCode}");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_2_Hoofdactiviteiten_And_Something_Else
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenResponseMapper.AddHoofdactiviteitToQuery(
                new AppSettings { BaseUrl = BaseUrl }, EenHoofdActiviteitCode, originalQuery: "oudenaarde", new[] { "SPRT", "DINT" });

            newQuery.Should()
                    .Be(
                         $"{BaseUrl}/v1/verenigingen/zoeken?q=oudenaarde&facets.hoofdactiviteitenVerenigingsloket=SPRT,DINT,{EenHoofdActiviteitCode}");
        }
    }
}
