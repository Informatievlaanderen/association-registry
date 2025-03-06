namespace AssociationRegistry.Test.Werkingsgebied.NutsLauLoader;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
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

