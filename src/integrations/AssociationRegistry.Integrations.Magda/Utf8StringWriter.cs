namespace AssociationRegistry.Integrations.Magda;

using System.Text;

public class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}
