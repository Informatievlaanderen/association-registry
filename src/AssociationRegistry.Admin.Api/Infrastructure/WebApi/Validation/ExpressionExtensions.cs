namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

public static class ExpressionExtensions
{
    public static string GetSerializedName<T>(this Expression<Func<T, string?>> expression)
    {
        if (expression.Body is not MemberExpression member)
            throw new InvalidOperationException("Expression must be a member expression.");

        var property = member.Member as PropertyInfo
                    ?? throw new InvalidOperationException("Member is not a property.");

        var dataMember = property.GetCustomAttribute<DataMemberAttribute>();

        return !string.IsNullOrWhiteSpace(dataMember?.Name)
            ? dataMember!.Name
            : property.Name;
    }
}
