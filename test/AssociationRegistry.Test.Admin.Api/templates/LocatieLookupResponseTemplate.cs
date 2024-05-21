namespace AssociationRegistry.Test.Admin.Api.templates;

using Scriban;
using System.Dynamic;
using Test.Framework;

public class LocatieLookupResponseTemplate
{
    private readonly dynamic _vereniging;

    public LocatieLookupResponseTemplate()
    {
        _vereniging = new ExpandoObject();
        _vereniging.locatielookups = new List<object>();
    }

    public LocatieLookupResponseTemplate WithVCode(string vCode)
    {
        _vereniging.vcode = vCode;

        return this;
    }

    public LocatieLookupResponseTemplate WithLocatieLookup(int locatieid, string adresId)
    {
        _vereniging.locatielookups.Add(new
        {
            locatieid = locatieid,
            adresid = adresId
        });

        return this;
    }

    public static implicit operator string(LocatieLookupResponseTemplate source)
        => source.Build();

    public string Build()
    {
        var json = GetType().Assembly.GetAssemblyResource(name: "templates.LocatieLookupResponse.json");

        var responseTemplate = Template.Parse(json);

        return responseTemplate.Render(new
        {
            vereniging = _vereniging,
        });
    }
}
