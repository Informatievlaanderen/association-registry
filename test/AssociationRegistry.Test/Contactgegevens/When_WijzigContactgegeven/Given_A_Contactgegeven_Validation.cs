﻿namespace AssociationRegistry.Test.Contactgegevens.When_WijzigContactgegeven;

using Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;
using Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_A_Contactgegeven_Validation
{
    private readonly Fixture _fixture;

    public Given_A_Contactgegeven_Validation()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    private void ValidateRequest(int lengthOfBeschrijving, bool expectValidationError)
    {
        var request = _fixture.Create<WijzigContactgegevenRequest>();

        request.Contactgegeven.Beschrijving = new string(Enumerable.Repeat(element: 'a', lengthOfBeschrijving).ToArray());

        var validator = new WijzigContactgegevenValidator();

        var validationResult = validator.Validate(request);

        validationResult.Errors
                        .Any(e => e.ErrorMessage ==
                                  $"Beschrijving mag niet langer dan {Contactgegeven.MaxLengthBeschrijving} karakters zijn.")
                        .Should().Be(expectValidationError);
    }

    [Fact]
    public void Then_It_Has_Validation_Error_When_Length_GreaterThan_42()
        => ValidateRequest(lengthOfBeschrijving: 129, expectValidationError: true);

    [Fact]
    public void Then_It_Has_No_Validation_Error_When_Length_LowerThanOrEqual_42()
        => ValidateRequest(lengthOfBeschrijving: 128, expectValidationError: false);
}
