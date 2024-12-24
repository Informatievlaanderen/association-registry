namespace AssociationRegistry.Test.Admin.Api.Framework.templates;

using EventStore;
using Microsoft.AspNetCore.Http;
using System.Dynamic;

public class ProblemDetailsResponseTemplate : ResponseTemplate
{
    private readonly dynamic _problemDetails;

    public ProblemDetailsResponseTemplate()
        : base("Framework.templates.ProblemDetailsResponse.json")
    {
        _problemDetails = new ExpandoObject();

        WithType("urn:associationregistry.admin.api:validation");
        WithTitle("Er heeft zich een fout voorgedaan!");
        WithDetail(string.Empty);
        WithStatus(StatusCodes.Status400BadRequest);
    }

    public ProblemDetailsResponseTemplate WithType(string type)
    {
        _problemDetails.type = type;

        return this;
    }

    public ProblemDetailsResponseTemplate WithTitle(string title)
    {
        _problemDetails.title = title;

        return this;
    }

    public ProblemDetailsResponseTemplate WithDetail(string detail)
    {
        _problemDetails.detail = detail;

        return this;
    }

    public ProblemDetailsResponseTemplate WithStatus(int status)
    {
        _problemDetails.status = status;

        return this;
    }

    public ProblemDetailsResponseTemplate FromException(UnexpectedAggregateVersionException ex)
        => WithStatus(StatusCodes.Status412PreconditionFailed)
           .WithDetail(ex.Message);

    protected override dynamic BuildModel()
        => new
        {
            type = _problemDetails.type,
            title = _problemDetails.title,
            detail = _problemDetails.detail,
            status = _problemDetails.status,
        };
}
