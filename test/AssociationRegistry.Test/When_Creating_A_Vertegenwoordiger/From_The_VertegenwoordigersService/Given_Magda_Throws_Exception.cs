namespace AssociationRegistry.Test.When_Creating_A_Vertegenwoordiger.From_The_VertegenwoordigersService;

using AutoFixture;
using Contactgegevens;
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
using Xunit.Categories;

[UnitTest]
public class Given_Magda_Throws_Exception
{
    [Fact]
    public async Task Then_it_throw_an_UnknownInszException()
    {
        var fixture = new Fixture().CustomizeAll();
        var vertegenwoordiger = fixture.Create<Vertegenwoordiger>();

        var magdaMock = new Mock<IMagdaFacade>();
        magdaMock.Setup(m => m.GetByInsz(vertegenwoordiger.Insz, It.IsAny<CancellationToken>())).ThrowsAsync(new MagdaException());

        var service = new VertegenwoordigerService(magdaMock.Object);

        var createFunc = () => service.GetVertegenwoordigersLijst(new[] { vertegenwoordiger });

        await createFunc.Should().ThrowAsync<UnknownInsz>();
    }
}
