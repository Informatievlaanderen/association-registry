namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;

using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using FluentValidation;

public class AdresIdValidator : AbstractValidator<AdresId>
{
    public AdresIdValidator()
    {
        this.RequireNotNullOrEmpty(adresId => adresId.Broncode);
        this.RequireNotNullOrEmpty(adresId => adresId.Bronwaarde);

        RuleFor(m => m.Broncode).MustNotContainHtml();
        RuleFor(m => m.Bronwaarde).MustNotContainHtml();
    }
}
