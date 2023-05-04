namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestValidating.Contactgegeven;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegeven_is_verplicht()
    {
        var validator = new VoegContactgegevenToeValidator();
        var request = new VoegContactgegevenToeRequest()
        {
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Contactgegeven)
            .WithErrorMessage("'Contactgegeven' is verplicht.");
    }
}
