namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Contactgegeven.RequestValidating.ContactGegeven;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegeven_is_verplicht()
    {
        var validator = new WijzigContactgegevenValidator();
        var request = new WijzigContactgegevenRequest();
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Contactgegeven)
              .WithErrorMessage("'Contactgegeven' is verplicht.");
    }
}
