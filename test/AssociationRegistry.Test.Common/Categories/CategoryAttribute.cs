// ReSharper disable once CheckNamespace
namespace Xunit.Categories;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CategoryAttribute:Attribute//, ITraitAttribute
{
    public CategoryAttribute(string categoryName)
    {
        this.Name = categoryName;
    }

    public string Name { get; private set; }


}
