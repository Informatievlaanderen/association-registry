using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AssociationRegistry.Public.ProjectionHost;

class DefaultApiJsonContractResolver : CamelCasePropertyNamesContractResolver
{
    private static readonly NamingStrategy DefaultNamingStrategy =
        new CamelCaseNamingStrategy
        {
            OverrideSpecifiedNames = true,
            ProcessDictionaryKeys = true,
            ProcessExtensionDataNames = true,
        };

    private DefaultApiJsonContractResolver(NamingStrategy namingStrategy)
        => NamingStrategy = namingStrategy;

    public static DefaultApiJsonContractResolver UsingDefaultNamingStrategy()
        => new(DefaultNamingStrategy);

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        return ApplyDataMemberParameters(member, property);
    }

    private static JsonProperty ApplyDataMemberParameters(MemberInfo member, JsonProperty property)
    {
        var dataMemberAttribute = member.GetCustomAttributes<DataMemberAttribute>().SingleOrDefault();
        if (dataMemberAttribute == null)
            return property;

        if (!property.Order.HasValue)
            property.Order = dataMemberAttribute.Order;

        if (!property.DefaultValueHandling.HasValue)
        {
            property.DefaultValueHandling = dataMemberAttribute.EmitDefaultValue
                ? DefaultValueHandling.Include
                : DefaultValueHandling.Ignore;
        }

        return property;
    }
}
