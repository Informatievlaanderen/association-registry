namespace AssociationRegistry.Test.Acm.Api.templates;

using AssociationRegistry.Acm.Schema.Constants;
using Common.Extensions;
using Scriban;
using System.Collections.Generic;
using Vereniging;

using VerenigingStatus = AssociationRegistry.Acm.Schema.Constants.VerenigingStatus;

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
        Verenigingstype verenigingstype,
        string status = VerenigingStatus.Actief,
        string[] corresponderendeVCodes = null,
        string kboNummer = "",
        bool isHoofdvertegenwoordigerVan = true)
    {
        _verenigingen.Add(new
        {
            vcode = vCode,
            corresponderendevcodes = corresponderendeVCodes ?? new string[0],
            vertegenwoordigerid = vertegenwoordigerId,
            naam = naam,
            status = status,
            kbonummer = kboNummer,
            verenigingstype = new
            {
                naam = verenigingstype.Naam,
                code = verenigingstype.Code,
            },
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
