namespace AssociationRegistry.Admin.Api.Infrastructure.Swagger.Annotations;

using Microsoft.AspNetCore.Mvc;

public class ProducesJsonAttribute : ProducesAttribute
{
    public ProducesJsonAttribute() : base("application/json")
    {
    }
}
