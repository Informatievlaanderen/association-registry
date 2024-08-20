namespace AssociationRegistry.Test.When_Creating_A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
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
            new()
            {
                Naam = new string(Enumerable.Repeat(element: 'a', lengthOfLocatienaam).ToArray()),
            },
        };

        var clock = new ClockStub(_fixture.Create<DateTime>());
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(clock);

        var validationResult = validator.Validate(request);

        validationResult.Errors
                        .Any(e => e.ErrorMessage == $"Locatienaam mag niet langer dan {Locatie.MaxLengthLocatienaam} karakters zijn.")
                        .Should().Be(expectValidationError);
    }

    [Fact]
    public void Then_It_Has_Validation_Error_When_Length_GreaterThan_42()
        => ValidateRequest(lengthOfLocatienaam: 43, expectValidationError: true);

    [Fact]
    public void Then_It_Has_No_Validation_Error_When_Length_LowerThanOrEqual_42()
        => ValidateRequest(lengthOfLocatienaam: 42, expectValidationError: false);
}
