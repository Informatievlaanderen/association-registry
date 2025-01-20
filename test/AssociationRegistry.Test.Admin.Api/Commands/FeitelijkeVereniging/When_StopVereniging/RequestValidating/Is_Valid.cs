namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_StopVereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Stop;
using AssociationRegistry.Admin.Api.Verenigingen.Stop.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Einddatum_Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_validation_errors_for_einddatum()
    {
        var validator = new StopVerenigingRequestValidator();

        var result = validator.TestValidate(new StopVerenigingRequest
                                                { Einddatum = DateOnly.ParseExact(s: "2022-12-31", Datum.Format) });

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.Einddatum);
    }
}
