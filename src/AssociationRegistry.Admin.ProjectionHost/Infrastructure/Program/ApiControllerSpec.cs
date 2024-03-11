namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;

public class ApiControllerSpec : IApiControllerSpecification
{
    private readonly Type _apiControllerType = typeof(ApiController).GetTypeInfo();

    public bool IsSatisfiedBy(ControllerModel controller) =>
        _apiControllerType.IsAssignableFrom(controller.ControllerType);
}

public abstract class ApiController : ControllerBase
{
}
