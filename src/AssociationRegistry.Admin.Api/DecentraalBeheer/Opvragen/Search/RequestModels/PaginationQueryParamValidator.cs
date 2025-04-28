namespace AssociationRegistry.Admin.Api.Verenigingen.Search.RequestModels;

using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using FluentValidation;

public class PaginationQueryParamsValidator : AbstractValidator<PaginationQueryParams>
{
    public PaginationQueryParamsValidator(AppSettings appSettings)
    {
        RuleFor(x => x)
           .Must(x => x.Limit + x.Offset <= appSettings.Search.MaxNumberOfSearchResults)
           .WithMessage($"'Limit' en 'Offset' mogen samen niet groter dan {appSettings.Search.MaxNumberOfSearchResults} zijn.");
    }
}
