﻿namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection.Given_An_Extensive_DataSet.Seed;

using AssociationRegistry.Admin.Api.Adapters.DuplicateVerenigingDetectionService;
using Microsoft.Extensions.Logging;
using  Microsoft.Extensions.Logging.Abstractions;

public class OutputLogger : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => throw new NotImplementedException();

    public bool IsEnabled(LogLevel logLevel)
        => throw new NotImplementedException();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        throw new NotImplementedException();
    }
}
