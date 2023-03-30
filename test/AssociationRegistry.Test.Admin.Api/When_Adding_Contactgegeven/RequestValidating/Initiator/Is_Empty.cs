namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestValidating.Initiator;

using AssociationRegistry.Admin.Api.Verenigingen.VoegContactGegevenToe;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__initiator_mag_niet_leeg_zijn()
    {
        var validator = new VoegContactgegevenToeValidator();
        var result = validator.TestValidate(new VoegContactgegevenToeRequest { Initiator = "" });

        result.ShouldHaveValidationErrorFor(vereniging => vereniging.Initiator)
            .WithErrorMessage("'Initiator' mag niet leeg zijn.")
            .Only();
    }
}
