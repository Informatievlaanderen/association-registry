namespace AssociationRegistry.Test.Acm.Api.templates;

using Scriban;
using Test.Framework;

public class VerenigingenPerInszResponseTemplate
{
    private string _insz;
    private readonly List<object> _verenigingen = new();

    public VerenigingenPerInszResponseTemplate WithInsz(string insz)
    {
        _insz = insz;

        return this;
    }

    public VerenigingenPerInszResponseTemplate WithVereniging(string vCode, string naam, string status = "Actief", string kboNummer = "")
    {
        _verenigingen.Add(new
        {
            vCode,
            naam,
            status,
            kboNummer,
        });

        return this;
    }

    public string Build()
    {
        var json = GetType().Assembly.GetAssemblyResource(name: "templates.VerenigingenPerInszResponse.json");

        var responseTemplate = Template.Parse(json);

        return responseTemplate.Render(new
        {
            Insz = _insz,
            Verenigingen = _verenigingen,
        });
    }

    public static implicit operator string(VerenigingenPerInszResponseTemplate source)
        => source.Build();
}
