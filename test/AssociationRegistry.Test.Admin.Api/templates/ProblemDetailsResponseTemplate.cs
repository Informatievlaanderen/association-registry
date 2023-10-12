namespace AssociationRegistry.Test.Admin.Api.templates;

using EventStore;
using Microsoft.AspNetCore.Http;
using Scriban;
using System.Dynamic;
using Test.Framework;

public class ProblemDetailsResponseTemplate
{
    private dynamic _problemDetails;

    public ProblemDetailsResponseTemplate()
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
        => this
          .WithStatus(StatusCodes.Status412PreconditionFailed)
          .WithDetail(ex.Message);

    public static implicit operator string(ProblemDetailsResponseTemplate source)
        => source.Build();

    public string Build()
    {
        var json = GetType().Assembly.GetAssemblyResource(name: "templates.ProblemDetailsResponse.json");

        var responseTemplate = Template.Parse(json);

        return responseTemplate.Render(new
        {
            type = _problemDetails.type,
            title = _problemDetails.title,
            detail = _problemDetails.detail,
            status = _problemDetails.status
        });
    }
}
