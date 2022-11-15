namespace AssociationRegistry.Admin.Api;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Verenigingen.Startdatums.Exceptions;

public class InvalidStartdatumFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is InvalidStartdatum)
        {
            context.Result = new BadRequestResult();
        }
    }
}
