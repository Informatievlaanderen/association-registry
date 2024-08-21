namespace AssociationRegistry.Test.Admin.Api.Framework.templates.kboSyncHistoriek;

using Common.Extensions;
using Scriban;

public class KboSyncHistoriekTemplate
{
    public KboSyncHistoriekGebeurtenis[] Gebeurtenissen { get; }

    public KboSyncHistoriekTemplate(params KboSyncHistoriekGebeurtenis[] gebeurtenissen)
    {
        Gebeurtenissen = gebeurtenissen;
    }

    public string Build()
    {
        var json = GetType().Assembly.GetAssemblyResource(name: "Framework.templates.kboSyncHistoriek.KboSyncHistoriek.json");

        var responseTemplate = Template.Parse(json);

        return responseTemplate.Render(this);
    }
}
