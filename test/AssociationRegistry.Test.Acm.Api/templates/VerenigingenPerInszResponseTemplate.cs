namespace AssociationRegistry.Test.Acm.Api.templates;

using AssociationRegistry.Acm.Schema.Constants;
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

    public VerenigingenPerInszResponseTemplate WithVereniging(
        string vCode,
        int vertegenwoordigerId,
        string naam,
        string status = VerenigingStatus.Actief,
        string kboNummer = "",
        bool isHoofdvertegenwoordigerVan = true)
    {
        _verenigingen.Add(new
        {
            vcode = vCode,
            vertegenwoordigerid = vertegenwoordigerId,
            naam = naam,
            status = status,
            kbonummer = kboNummer,
            ishoofdvertegenwoordigervan = isHoofdvertegenwoordigerVan,
        });

        return this;
    }

    public string Build()
    {
        var json = GetType().Assembly.GetAssemblyResource(name: EmbeddedResources.templates_VerenigingenPerInszResponse_json);

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
