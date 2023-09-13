namespace AssociationRegistry.Admin.Api.Infrastructure.Swagger.Annotations;

using Microsoft.AspNetCore.Mvc;

public class ConsumesJsonAttribute : ConsumesAttribute
{
    public ConsumesJsonAttribute() : base("application/json")
    {
    }
}
