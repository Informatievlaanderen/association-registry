﻿namespace AssociationRegistry.Resources;

using Grar.Clients;
using System.Net;

public static class FormattedExceptionMessages
{
    public static string ServiceReturnedNonSuccesfulStatusCode(
        string service,
        HttpStatusCode statusCode,
        ContextDescription contextDescription)
        => string.Format(ExceptionMessages.ServiceReturnedNonSuccesfulStatusCode, service, statusCode, contextDescription);
}
