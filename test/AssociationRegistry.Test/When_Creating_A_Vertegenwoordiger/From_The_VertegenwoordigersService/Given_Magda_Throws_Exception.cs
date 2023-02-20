﻿namespace AssociationRegistry.Test.When_Creating_A_Vertegenwoordiger.From_The_VertegenwoordigersService;

using AutoFixture;
using FluentAssertions;
using Framework;
using INSZ;
using Magda;
using Magda.Exceptions;
using Moq;
using Vereniging.RegistreerVereniging;
using Vertegenwoordigers;
using Vertegenwoordigers.Exceptions;
using Xunit;

public class Given_Magda_Throws_Exception
{
    [Fact]
    public async Task Then_it_throw_an_UnknownInszException()
    {
        var fixture = new Fixture();
        var insz = Insz.Create(InszTestSet.Insz1);

        var magdaMock = new Mock<IMagdaFacade>();
        magdaMock.Setup(m => m.GetByInsz(insz, It.IsAny<CancellationToken>())).ThrowsAsync(new MagdaException());

        var service = new VertegenwoordigerService(magdaMock.Object);

        var vertegenwoordiger = new RegistreerVerenigingCommand.Vertegenwoordiger(
            insz,
            fixture.Create<bool>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            new RegistreerVerenigingCommand.ContactInfo[]
            {
                new(fixture.Create<string>(), $"{ fixture.Create<string?>() }@{ fixture.Create<string?>() }.com", fixture.Create<int>().ToString(), $"http://{fixture.Create<string?>()}.com", $"http://{fixture.Create<string?>()}.com", false),
            });
        var createFunc = () => service.GetVertegenwoordigersLijst(new[] { vertegenwoordiger });

        await createFunc.Should().ThrowAsync<UnknownInsz>();
    }
}
