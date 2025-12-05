namespace AssociationRegistry.Test.Magda.GeefPersoonService.Validator;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Integrations.Magda.Persoon.GeefPersoon;
using Integrations.Magda.Persoon.Validation;
using Integrations.Magda.Shared.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Middleware;
using Resources;
using Xunit;

public class GeefPersoonValidatorTests
{
    private readonly Fixture _fixture;

    public GeefPersoonValidatorTests()
    {
        _fixture = new Fixture().CustomizeAdminApi();
    }

    [Theory]
    [InlineData("30001")]
    [InlineData("30002")]
    [InlineData("30003")]
    [InlineData("30004")]
    public void With_UitzonderingTypeFout_And_GekendeGebruikersFoutCode_ThrowsDomainExceptionWhenUitzonderingDoorGebruiker(
        string foutCode)
    {
        var sut = new MagdaGeefPersoonValidator(NullLogger<MagdaGeefPersoonValidator>.Instance);

        var exception = Assert.Throws<EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz>(() =>
        {
            sut.ValidateOrThrow(MagdaTestResponseFactory.GeefPersoonResponses.Fout(foutCode, UitzonderingTypeType.FOUT));
        });

        exception.Message.Should().Be(ExceptionMessages.EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz);
    }

    [Fact]
    public void With_UitzonderingTypeFout_And_MagdaFoutCode_ThrowsMagdaException()
    {
        var foutCode = _fixture.Create<string>();

        var sut = new MagdaGeefPersoonValidator(NullLogger<MagdaGeefPersoonValidator>.Instance);

        var exception = Assert.Throws<MagdaException>(() =>
        {
            sut.ValidateOrThrow(MagdaTestResponseFactory.GeefPersoonResponses.Fout(foutCode, UitzonderingTypeType.FOUT));
        });

        exception.Message.Should().Be(ExceptionMessages.MagdaException);
    }

    [Theory]
    [InlineData(UitzonderingTypeType.INFORMATIE)]
    [InlineData(UitzonderingTypeType.WAARSCHUWING)]
    public void With_UitzonderingType_Then_Nothing(UitzonderingTypeType uitzonderingTypeType)
    {
        var foutCode = _fixture.Create<string>();

        var sut = new MagdaGeefPersoonValidator(NullLogger<MagdaGeefPersoonValidator>.Instance);

        sut.ValidateOrThrow(MagdaTestResponseFactory.GeefPersoonResponses.Fout(foutCode, uitzonderingTypeType));
    }
}
