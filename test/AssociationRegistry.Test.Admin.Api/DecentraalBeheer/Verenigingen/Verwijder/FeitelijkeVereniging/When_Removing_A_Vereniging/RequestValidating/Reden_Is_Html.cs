namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Verwijder.FeitelijkeVereniging.When_Removing_A_Vereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Verwijder;
using AssociationRegistry.Admin.Api.Verenigingen.Verwijder.RequestModels;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Reden_Is_Html : ValidatorTest
{
    [Fact]
    public void Has_validation_error_for_reden()
    {
        var validator = new VerwijderVerenigingRequestValidator();
        var result = validator.TestValidate(new VerwijderVerenigingRequest { Reden = "<p>Injecteer html</p>" });

        result.ShouldHaveValidationErrorFor(nameof(VerwijderVerenigingRequest.Reden))
              .WithErrorMessage(ExceptionMessages.UnsupportedContent);
    }
}
