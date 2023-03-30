namespace AssociationRegistry.Test.Admin.Api.When_Adding_Contactgegeven.RequestValidating.Contactgegeven.Type;

using AssociationRegistry.Admin.Api.Verenigingen.VoegContactGegevenToe;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new VoegContactgegevenToeValidator();
        var result = validator.TestValidate(
            new VoegContactgegevenToeRequest
            {
                Contactgegeven = new VoegContactgegevenToeRequest.RequestContactgegeven
                    {
                        Type = "email",
                    },
        });

        result.ShouldNotHaveValidationErrorFor(nameof(VoegContactgegevenToeRequest.Contactgegeven) + "." + nameof(VoegContactgegevenToeRequest.RequestContactgegeven.Type));
    }
}
