namespace AssociationRegistry.Test.Common.templates.adminApi.kboSyncHistoriek;

using Extensions;
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
        var json = GetType().Assembly.GetAssemblyResource(name: "templates.kboSyncHistoriek.KboSyncHistoriek.json");

        var responseTemplate = Template.Parse(json);

        return responseTemplate.Render(this);
    }
}
