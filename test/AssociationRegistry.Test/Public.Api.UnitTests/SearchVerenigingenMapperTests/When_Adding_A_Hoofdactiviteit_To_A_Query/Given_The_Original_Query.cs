namespace AssociationRegistry.Test.Public.Api.UnitTests.SearchVerenigingenMapperTests.When_Adding_A_Hoofdactiviteit_To_A_Query;

using AssociationRegistry.Public.Api;
using AssociationRegistry.Public.Api.SearchVerenigingen;
using FluentAssertions;
using Xunit;

public class Given_The_Original_Query
{
    private const string BaseUrl = "http://base/";

    public class Given_An_Empty_OriginalQuery
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenMapper.AddHoofdactiviteitToQuery(new AppSettings { BaseUrl = BaseUrl }, "BWRK", "");

            newQuery.Should().Be($"{BaseUrl}v1/verenigingen/zoeken?q=(hoofdactiviteiten.code:BWRK)");
        }
    }

    public class Given_An_OriginalQuery_That_Does_Not_Contain_Hoofdactiviteiten
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenMapper.AddHoofdactiviteitToQuery(new AppSettings { BaseUrl = BaseUrl }, "BWRK", "oudenaarde");

            newQuery.Should().Be($"{BaseUrl}v1/verenigingen/zoeken?q=(hoofdactiviteiten.code:BWRK) AND oudenaarde");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_Only_A_Star
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenMapper.AddHoofdactiviteitToQuery(new AppSettings { BaseUrl = BaseUrl }, "BWRK", "*");

            newQuery.Should().Be($"{BaseUrl}v1/verenigingen/zoeken?q=(hoofdactiviteiten.code:BWRK)");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_1_Hoofdactiviteit_And_Nothing_Else
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenMapper.AddHoofdactiviteitToQuery(new AppSettings { BaseUrl = BaseUrl }, "BWRK", "(hoofdactiviteiten.code:SPRT)");

            newQuery.Should().Be($"{BaseUrl}v1/verenigingen/zoeken?q=(hoofdactiviteiten.code:SPRT OR hoofdactiviteiten.code:BWRK)");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_2_Hoofdactiviteiten_And_Nothing_Else
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenMapper.AddHoofdactiviteitToQuery(new AppSettings { BaseUrl = BaseUrl }, "BWRK", "(hoofdactiviteiten.code:SPRT OR hoofdactiviteiten.code:DINT)");

            newQuery.Should().Be($"{BaseUrl}v1/verenigingen/zoeken?q=(hoofdactiviteiten.code:SPRT OR hoofdactiviteiten.code:DINT OR hoofdactiviteiten.code:BWRK)");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_1_Hoofdactiviteit_And_Something_Else
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenMapper.AddHoofdactiviteitToQuery(new AppSettings { BaseUrl = BaseUrl }, "BWRK", "(hoofdactiviteiten.code:SPRT) AND oudenaarde");

            newQuery.Should().Be($"{BaseUrl}v1/verenigingen/zoeken?q=(hoofdactiviteiten.code:SPRT OR hoofdactiviteiten.code:BWRK) AND oudenaarde");
        }
    }

    public class Given_An_OriginalQuery_That_Contains_2_Hoofdactiviteiten_And_Something_Else
    {
        [Fact]
        public void Then_it_adds_the_hoofdactiviteit_to_the_query()
        {
            var newQuery = SearchVerenigingenMapper.AddHoofdactiviteitToQuery(new AppSettings { BaseUrl = BaseUrl }, "BWRK", "(hoofdactiviteiten.code:SPRT OR hoofdactiviteiten.code:DINT) AND oudenaarde");

            newQuery.Should().Be($"{BaseUrl}v1/verenigingen/zoeken?q=(hoofdactiviteiten.code:SPRT OR hoofdactiviteiten.code:DINT OR hoofdactiviteiten.code:BWRK) AND oudenaarde");
        }
    }
}
