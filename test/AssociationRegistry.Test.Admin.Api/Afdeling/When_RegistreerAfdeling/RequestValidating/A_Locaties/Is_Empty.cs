namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Locaties;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new RegistreerAfdelingRequestValidator();
        var request = new RegistreerAfdelingRequest
        {
            Locaties = Array.Empty<ToeTeVoegenLocatie>(),
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.Locaties);
    }
}
