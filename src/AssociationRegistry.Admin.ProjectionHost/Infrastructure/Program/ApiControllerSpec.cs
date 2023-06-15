namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program;

using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

public class ApiControllerSpec : IApiControllerSpecification
{
    private readonly Type _apiControllerType = typeof(ApiController).GetTypeInfo();

    public bool IsSatisfiedBy(ControllerModel controller) =>
        _apiControllerType.IsAssignableFrom(controller.ControllerType);
}

public abstract class ApiController : ControllerBase { }
