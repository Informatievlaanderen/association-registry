namespace AssociationRegistry.Magda.Extensions;

using System.Xml.Serialization;

public static class SerializationExtensions
{
    public static string SerializeObject<T>(this T @object)
    {
        var serializer = new XmlSerializer(@object!.GetType());

        var ns = new XmlSerializerNamespaces();
        ns.Add(prefix: "s", ns: "http://schemas.xmlsoap.org/soap/envelope/");
        ns.Add(prefix: "geefondernemingvkbo", ns: "http://webservice.geefondernemingvkbodienst-02_00.onderneming-02_00.vip.vlaanderen.be");

        using var textWriter = new Utf8StringWriter();
        serializer.Serialize(textWriter, @object, ns);

        return textWriter.ToString();
    }

    public static string SerializeObject<T>(this T @object, IDictionary<string, string> namespaces)
    {
        var serializer = new XmlSerializer(@object!.GetType());

        var ns = new XmlSerializerNamespaces();
        ns.Add(prefix: "s", ns: "http://schemas.xmlsoap.org/soap/envelope/");
        ns.Add(prefix: "geefonderneming", ns: "http://webservice.geefondernemingdienst-02_00.onderneming-02_00.vip.vlaanderen.be");

        using var textWriter = new Utf8StringWriter();
        serializer.Serialize(textWriter, @object, ns);

        return textWriter.ToString();
    }
}
