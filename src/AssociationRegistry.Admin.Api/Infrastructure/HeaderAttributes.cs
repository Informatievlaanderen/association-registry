namespace AssociationRegistry.Admin.Api.Infrastructure;

using Microsoft.AspNetCore.Mvc;

public class IfMatchHeaderAttribute : FromHeaderAttribute
{
    public IfMatchHeaderAttribute()
    {
        Name = WellknownHeaderNames.IfMatch;
    }
}

public class InitiatorHeaderAttribute : FromHeaderAttribute
{
    public InitiatorHeaderAttribute()
    {
        Name = WellknownHeaderNames.Initiator;
    }
}
