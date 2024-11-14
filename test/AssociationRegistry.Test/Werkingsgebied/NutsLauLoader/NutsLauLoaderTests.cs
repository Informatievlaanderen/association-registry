namespace AssociationRegistry.Test.NutsLauLoader;

using CsvHelper.Configuration.Attributes;
using FluentAssertions;
using Vereniging;
using Xunit;

public class NutsLauLoaderTests
{
    [Fact]
    public async Task LoadNutsLauCodes()
    {
        var records = NutsLauReader.Read();

        records.First().Should().BeEquivalentTo(new NutsLauLine()
        {
            Nuts = "BE100",
            Lau = "21001",
            Gemeente = "Anderlecht",
        });
    }
}

