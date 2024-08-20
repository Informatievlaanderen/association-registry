namespace AssociationRegistry.Test.Admin.Api.Framework.templates;

using System.Dynamic;

public class LocatieLookupResponseTemplate : ResponseTemplate
{
    private readonly dynamic _vereniging;

    public LocatieLookupResponseTemplate()
        : base("templates.LocatieLookupResponse.json")
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
            adresid = adresId,
        });

        return this;
    }

    protected override dynamic BuildModel()
        => new
        {
            vereniging = _vereniging,
        };
}
