// ReSharper disable once CheckNamespace
namespace Xunit.Categories
{
    using v3;

    // [TraitDiscoverer(IntegrationTestDiscoverer.DiscovererTypeName, DiscovererUtil.AssemblyName)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class IntegrationTestAttribute:Attribute//, ITraitAttribute
    {

    }
}

namespace Xunit.Categories
{
    // [TraitDiscoverer(CategoryDiscoverer.DiscovererTypeName, DiscovererUtil.AssemblyName)]
}
