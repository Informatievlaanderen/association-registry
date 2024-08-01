namespace AssociationRegistry.Test.When_WijzigContactgegeven;

using Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.Framework;
using AssociationRegistry.Test.Framework.Customizations;
using AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Contactgegeven_Validation
{
    private readonly Fixture _fixture;

    public Given_A_Contactgegeven_Validation()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    private void ValidateRequest(int lengthOfBeschrijving, bool expectValidationError)
    {
        var request = _fixture.Create<WijzigContactgegevenRequest>();

        request.Contactgegeven.Beschrijving = new string(Enumerable.Repeat('a', lengthOfBeschrijving).ToArray());

        var validator = new WijzigContactgegevenValidator();

        var validationResult = validator.Validate(request);

        validationResult.Errors
                        .Any(e => e.ErrorMessage == $"Beschrijving mag niet langer dan {Contactgegeven.MaxLengthBeschrijving} karakters zijn.")
                        .Should().Be(expectValidationError);
    }

    [Fact]
    public void Then_It_Has_Validation_Error_When_Length_GreaterThan_42() => ValidateRequest(43, true);

    [Fact]
    public void Then_It_Has_No_Validation_Error_When_Length_LowerThanOrEqual_42() => ValidateRequest(42, false);
}
