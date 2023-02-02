namespace AssociationRegistry.Test.When_Creating_A_Vertegenwoordiger.From_The_VertegenwoordigersService;

using ContactInfo;
using FluentAssertions;
using Framework;
using INSZ;
using Magda;
using Magda.Exceptions;
using Moq;
using Vertegenwoordigers;
using Vertegenwoordigers.Exceptions;
using Xunit;

public class Given_Magda_Throws_Exception
{
    [Fact]
    public async Task Then_it_throw_an_UnknownInszException()
    {
        var insz = Insz.Create(InszTestSet.Insz1);
        var contactLijst = ContactLijst.Create(new[] { ContactInfo.CreateInstance(null, "loki@trikster.be", "123456798", "www.mischief.loki", "#LocoLoki") });

        var magdaMock = new Mock<IMagdaFacade>();
        magdaMock.Setup(m => m.GetByInsz(insz, It.IsAny<CancellationToken>())).ThrowsAsync(new MagdaException());

        var service = new VertegenwoordigerService(magdaMock.Object);

        var createFunc = () => service.CreateVertegenwoordiger(
            insz,
            false,
            "god of mischief",
            "Trikstergod",
            contactLijst);

        await createFunc.Should().ThrowAsync<UnknownInsz>();
    }
}
