namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerwijderVertegenwoordiger;

using FluentValidation;
using Infrastructure.Validation;

public class VerwijderVertegenwoordigerRequestValidator: AbstractValidator<VerwijderVertegenwoordigerRequest>
{
    public VerwijderVertegenwoordigerRequestValidator()
    {
        this.RequireNotNullOrEmpty(request => request.Initiator);
    }
}
