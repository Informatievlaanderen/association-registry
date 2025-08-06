namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;

using Microsoft.AspNetCore.Mvc;

public class ConsumesJsonAttribute : ConsumesAttribute
{
    public ConsumesJsonAttribute() : base("application/json")
    {
    }
}
