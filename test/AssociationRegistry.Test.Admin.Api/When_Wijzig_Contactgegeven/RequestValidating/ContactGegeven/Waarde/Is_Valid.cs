namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.RequestValidating.ContactGegeven.Waarde;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new WijzigContactgegevenValidator();
        var result = validator.TestValidate(
            new WijzigContactgegevenRequest
            {
                Contactgegeven = new WijzigContactgegevenRequest.RequestContactgegeven
                    {
                        Waarde = "test waarde",
                    },
            });

        result.ShouldNotHaveValidationErrorFor(nameof(WijzigContactgegevenRequest.Contactgegeven) + "." + nameof(WijzigContactgegevenRequest.RequestContactgegeven.Waarde));
    }
}
