namespace AssociationRegistry.Test.Admin.Api.Framework.templates;

using Common.Extensions;
using Scriban;

public abstract class ResponseTemplate
{
    private readonly string _template;

    public ResponseTemplate(string template)
    {
        _template = template;
    }

    protected abstract dynamic BuildModel();

    public static implicit operator string(ResponseTemplate source)
        => source.Build();

    public string Build()
    {
        var json = GetType().Assembly.GetAssemblyResource(name: _template);

        var responseTemplate = Template.Parse(json);

        return responseTemplate.Render(BuildModel());
    }
}
