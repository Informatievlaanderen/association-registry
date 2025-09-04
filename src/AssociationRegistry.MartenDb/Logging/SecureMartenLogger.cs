namespace AssociationRegistry.MartenDb.Logging;

using Marten;
using Marten.Services;
using Microsoft.Extensions.Logging;
using Npgsql;

public class SecureMartenLogger : IMartenLogger, IMartenSessionLogger
{
    private readonly ILogger<SecureMartenLogger> _logger;
    private readonly bool _includeParameterNames;

    public SecureMartenLogger(ILogger<SecureMartenLogger> logger, bool includeParameterNames = false)
    {
        _logger = logger;
        _includeParameterNames = includeParameterNames;
    }

    public IMartenSessionLogger StartSession(IQuerySession session)
        => this;

    public void LogSuccess(NpgsqlCommand command)
    {
        using (_logger.BeginScope(new { SessionId = Guid.NewGuid() }))
        {
            _logger.LogDebug("Executing SQL: {CommandText}", command.CommandText);

            if (_includeParameterNames && command.Parameters.Count > 0)
            {
                var paramNames = command.Parameters
                    .OfType<NpgsqlParameter>()
                    .Select(p => p.ParameterName);
                _logger.LogDebug("With parameters: {ParameterNames}", string.Join(", ", paramNames));
            }
        }
    }

    public void LogFailure(NpgsqlCommand command, Exception ex)
    {
        _logger.LogError(ex, "SQL command failed: {CommandText}", command.CommandText);

        if (_includeParameterNames && command.Parameters.Count > 0)
        {
            var paramNames = command.Parameters
                .OfType<NpgsqlParameter>()
                .Select(p => p.ParameterName);
            _logger.LogError("Failed command had parameters: {ParameterNames}", string.Join(", ", paramNames));
        }
    }

    public void LogFailure(Exception ex, string message)
    {
        _logger.LogError(ex, "Encountered exception: {Message}", message);
    }

    public void RecordSavedChanges(IDocumentSession session, IChangeSet commit)
    {
        var insertCount = commit.Inserted.Count();
        var updateCount = commit.Updated.Count();
        var deleteCount = commit.Deleted.Count();

        if (insertCount > 0 || updateCount > 0 || deleteCount > 0)
        {
            _logger.LogInformation(
                "Marten changes saved - Inserts: {InsertCount}, Updates: {UpdateCount}, Deletes: {DeleteCount}",
                insertCount, updateCount, deleteCount);
        }
    }

    public void SchemaChange(string sql)
    {
        _logger.LogInformation("Schema change executed: {Sql}", sql);
    }

    public void LogSuccess(NpgsqlBatch batch)
    {
        _logger.LogDebug("Batch executed successfully with {CommandCount} commands", batch.BatchCommands.Count);
    }

    public void LogFailure(NpgsqlBatch batch, Exception ex)
    {
        _logger.LogError(ex, "Batch execution failed with {CommandCount} commands", batch.BatchCommands.Count);
    }

    public void LogFailure(Exception ex)
    {
        _logger.LogError(ex, "Marten operation failed");
    }

    public void RecordSavedChanges(IDocumentSession session, IChangeSet commit, NpgsqlBatch batch)
    {
        RecordSavedChanges(session, commit);
    }

    public void OnBeforeExecute(NpgsqlCommand command)
    {
        _logger.LogDebug("Preparing to execute command: {CommandText}",
            command.CommandText.Length > 100 ? command.CommandText.Substring(0, 100) + "..." : command.CommandText);
    }

    public void OnBeforeExecute(NpgsqlBatch batch)
    {
        _logger.LogDebug("Preparing to execute batch with {CommandCount} commands", batch.BatchCommands.Count);
    }
}
