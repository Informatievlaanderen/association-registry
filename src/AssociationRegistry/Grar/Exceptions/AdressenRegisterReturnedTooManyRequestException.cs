namespace AssociationRegistry.Grar.Exceptions;

using Clients;
using Resources;
using System.Net;

public class AdressenRegisterReturnedTooManyRequestException(string service, HttpStatusCode statusCode, ContextDescription contextDescription)
    : Exception(FormattedExceptionMessages.ServiceReturnedNonSuccesfulStatusCode(service, statusCode, contextDescription));
