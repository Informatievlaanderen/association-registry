namespace AssociationRegistry.Admin.Api.Verenigingen.Stop;

using FluentValidation;
using RequestModels;
using System;

public class StopVerenigingRequestValidator:AbstractValidator<StopVerenigingRequest>
{
    public StopVerenigingRequestValidator()
    {
        RuleFor(r => r.Einddatum)
           .Must(NotBeDefault);
    }

    private static bool NotBeDefault(DateOnly e)
        => e != default;
}
