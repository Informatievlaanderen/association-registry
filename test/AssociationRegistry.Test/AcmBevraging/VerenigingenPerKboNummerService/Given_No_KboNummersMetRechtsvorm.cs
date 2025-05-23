﻿namespace AssociationRegistry.Test.AcmBevraging.VerenigingenPerKboNummerService;

using AssociationRegistry.Magda.Services;
using Common.Framework;
using FluentAssertions;
using Xunit;

public class Given_No_KboNummersMetRechtsvorm
{
    [Fact]
    public async ValueTask Returns_No_VerenigingenPerKbo()
    {
        var store = await TestDocumentStoreFactory.CreateAsync(nameof(Given_No_KboNummersMetRechtsvorm));

        var service = new AssociationRegistry.AcmBevraging.VerenigingenPerKboNummerService(new RechtsvormCodeService(), store);
        var result = await service.GetVerenigingenPerKbo([], CancellationToken.None);

        result.Should().BeEmpty();
    }
}
