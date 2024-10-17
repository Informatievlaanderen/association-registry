namespace AssociationRegistry.Test.Services.VerenigingenPerKboNummerService;

using AssociationRegistry.Magda.Services;
using FluentAssertions;
using Xunit;

public class Given_No_KboNummersMetRechtsvorm
{
    [Fact]
    public async Task Returns_No_VerenigingenPerKbo()
    {
        var service = new AssociationRegistry.Services.VerenigingenPerKboNummerService(new RechtsvormCodeService());
        var result = await service.GetKboNummerInfo([], CancellationToken.None);

        result.Should().BeEmpty();
    }
}
