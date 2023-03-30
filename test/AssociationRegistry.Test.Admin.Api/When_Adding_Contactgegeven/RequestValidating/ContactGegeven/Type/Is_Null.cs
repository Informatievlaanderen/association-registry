namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestValidating.Contactgegeven.Type;

using AssociationRegistry.Admin.Api.Verenigingen.VoegContactGegevenToe;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegevenType_mag_niet_leeg_zijn()
    {
        var validator = new VoegContactgegevenToeValidator();
        var result = validator.TestValidate(
            new VoegContactgegevenToeRequest
            {
                Contactgegeven = new VoegContactgegevenToeRequest.RequestContactgegeven
                    {
                        Type = null!,
                    },
            });

        result.ShouldHaveValidationErrorFor(nameof(VoegContactgegevenToeRequest.Contactgegeven)+"."+nameof(VoegContactgegevenToeRequest.RequestContactgegeven.Type))
            .WithErrorMessage("'Type' is verplicht.")
            .Only();
    }
}
