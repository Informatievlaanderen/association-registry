namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestValidating.Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.VoegContactGegevenToe;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__initiator_is_verplicht()
    {
        var validator = new VoegContactgegevenToeValidator();
        var result = validator.TestValidate(new VoegContactgegevenToeRequest());

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
            .WithErrorMessage("'Initiator' is verplicht.");
    }
}
