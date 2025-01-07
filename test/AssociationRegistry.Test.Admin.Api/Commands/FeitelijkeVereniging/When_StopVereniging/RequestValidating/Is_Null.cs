namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_StopVereniging.RequestValidating;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Stop.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
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
