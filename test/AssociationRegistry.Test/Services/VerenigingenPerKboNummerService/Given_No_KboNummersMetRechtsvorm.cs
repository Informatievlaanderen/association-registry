namespace AssociationRegistry.Test.Services.VerenigingenPerKboNummerService;

using AssociationRegistry.Services;
using FluentAssertions;
using Xunit;

public class Given_No_KboNummersMetRechtsvorm
{
    [Fact]
    public async Task Returns_No_VerenigingenPerKbo()
    {
        var service = new AssociationRegistry.Services.VerenigingenPerKboNummerService();
        var result = await service.GetKboNummerInfo([], CancellationToken.None);

        result.Should().BeEmpty();
    }
}

public class Given_KboNummersMetOngeldigeRechtsvorm
{
    [Fact]
    public async Task Returns_VerenigingenPerKbo_With_NVT()
    {
        var service = new AssociationRegistry.Services.VerenigingenPerKboNummerService();
        var result = await service.GetKboNummerInfo(new []
        {
            new KboNummerMetRechtsvorm("12345", "FV")
        }, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Single().VCode.Should().Be("NVT");
        result.Single().IsHoofdvertegenwoordiger.Should().BeFalse();
    }
}

