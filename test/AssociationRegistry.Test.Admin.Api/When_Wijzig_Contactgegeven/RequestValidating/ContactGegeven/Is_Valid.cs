namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven.RequestValidating.ContactGegeven;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Theory]
    [InlineData("waarde1", null, null)]
    [InlineData(null, "omschrijving2", null)]
    [InlineData(null, null, true)]
    [InlineData("waarde4", "omschrijving4", true)]
    public void Has_not_validation_error(string? waarde, string? omschrijving, bool? isPrimair)
    {
        var validator = new WijzigContactgegevenValidator();
        var request = new WijzigContactgegevenRequest
        {
            Contactgegeven = new WijzigContactgegevenRequest.RequestContactgegeven
            {
                Waarde = waarde,
                Omschrijving = omschrijving,
                IsPrimair = isPrimair,
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(toeRequest => toeRequest.Contactgegeven);
    }
}
