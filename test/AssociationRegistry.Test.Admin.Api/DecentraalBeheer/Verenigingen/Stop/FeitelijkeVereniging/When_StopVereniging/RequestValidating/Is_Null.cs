namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Stop.FeitelijkeVereniging.When_StopVereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Einddatum_Is_Null : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors_for_startdatum()
    {
        var validator = new StopVerenigingRequestValidator();
        var result = validator.TestValidate(new StopVerenigingRequest { Einddatum = null });

        result.ShouldHaveValidationErrorFor(nameof(StopVerenigingRequest.Einddatum))
              .WithErrorMessage("'Einddatum' is verplicht.");
    }
}
