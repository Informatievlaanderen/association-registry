namespace AssociationRegistry.Test.Admin.Api.templates.kboSyncHistoriek;

using Common.Extensions;
using Scriban;
using Test.Framework;

public class KboSyncHistoriekTemplate
{
    public KboSyncHistoriekGebeurtenis[] Gebeurtenissen { get; }

    public KboSyncHistoriekTemplate(params KboSyncHistoriekGebeurtenis[] gebeurtenissen)
    {
        Gebeurtenissen = gebeurtenissen;
    }

    public string Build()
    {
        var json = GetType().Assembly.GetAssemblyResource(name: "templates.kboSyncHistoriek.KboSyncHistoriek.json");

        var responseTemplate = Template.Parse(json);

        return responseTemplate.Render(this);
    }
}
