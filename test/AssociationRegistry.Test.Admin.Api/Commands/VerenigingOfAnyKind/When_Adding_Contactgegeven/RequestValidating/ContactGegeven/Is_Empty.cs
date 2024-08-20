namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Contactgegeven.RequestValidating.ContactGegeven;

using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.RequestsModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegeven_is_verplicht()
    {
        var validator = new VoegContactgegevenToeValidator();
        var request = new VoegContactgegevenToeRequest();
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Contactgegeven)
              .WithErrorMessage("'Contactgegeven' is verplicht.");
    }
}
