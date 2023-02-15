namespace AssociationRegistry.Test.When_Creating_A_HoofdactiviteitenLijst;

using Activiteiten;
using Activiteiten.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_A_List_Of_Hoofdactiviteiten_With_Duplicates
{
    [Fact]
    public void Then_It_Throws_A_DuplicateHoofdactiviteitException()
    {
        var fixture = new Fixture();
        var listOfHoofdactiviteiten = Hoofdactiviteit.All()
            .OrderBy(_ => fixture.Create<int>())
            .Take(1)
            .Repeat(2)
            .ToList();

        var ctor = () => HoofdactiviteitenLijst.Create(listOfHoofdactiviteiten);

        ctor.Should().Throw<DuplicateHoofdactiviteit>();
    }
}

public static class EnumeralbeExtentions
{
    public static IEnumerable<T> Repeat<T>(this IEnumerable<T> collection, int times)
        => Enumerable.Repeat(collection.ToList(), times).SelectMany(_ => _);
}
