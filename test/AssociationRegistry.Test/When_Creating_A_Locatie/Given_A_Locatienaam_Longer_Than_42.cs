namespace AssociationRegistry.Test.When_Creating_A_Locatie;

using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Locatienaam_Validation
{
    private readonly Fixture _fixture;

    public Given_A_Locatienaam_Validation()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    private void ValidateRequest(int lengthOfLocatienaam, bool expectValidationError)
    {
        var request = _fixture.Create<RegistreerFeitelijkeVerenigingRequest>();

        request.Locaties = new ToeTeVoegenLocatie[]
        {
            new ToeTeVoegenLocatie()
            {
                Naam = new string(Enumerable.Repeat('a', lengthOfLocatienaam).ToArray()),
            },
        };

        var clock = new ClockStub(_fixture.Create<DateTime>());
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(clock);

        var validationResult = validator.Validate(request);

        validationResult.Errors
                        .Any(e => e.ErrorMessage == $"Locatienaam mag niet langer dan 42 karakters zijn.")
                        .Should().Be(expectValidationError);
    }

    [Fact]
    public void Then_It_Has_Validation_Error_When_Length_GreaterThan_42() => ValidateRequest(43, true);

    [Fact]
    public void Then_It_Has_No_Validation_Error_When_Length_LowerThanOrEqual_42() => ValidateRequest(42, false);

}
