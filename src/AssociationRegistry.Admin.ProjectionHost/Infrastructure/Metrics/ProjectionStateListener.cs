namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Metrics;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

public class AllFieldsContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var props = type
                   .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                   .Select(p => base.CreateProperty(p, memberSerialization))
                   .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                              .Select(f => base.CreateProperty(f, memberSerialization)))
                   .ToList();

        props.ForEach(p =>
        {
            p.Writable = true;
            p.Readable = true;
        });

        return props;
    }
}
