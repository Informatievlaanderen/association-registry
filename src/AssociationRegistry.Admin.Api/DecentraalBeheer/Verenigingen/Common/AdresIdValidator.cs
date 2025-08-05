namespace AssociationRegistry.Admin.Api.Verenigingen.Common;

using FluentValidation;
using Infrastructure.WebApi.Validation;

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
