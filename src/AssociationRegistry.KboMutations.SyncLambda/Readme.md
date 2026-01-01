# Kbo Sync Lambda

Triggered by SQS items on the kbo sync queue. 

Performs the necessary logic to bring the association up to date with KBO.

## Main dependencies

- nuget package `Be.Vlaanderen.Basisregisters.AssociationRegistry`
- nuget package `Be.Vlaanderen.Basisregisters.AssociationRegistry.Magda`
- Kbo sync queue

## Message Format

The Lambda supports two message formats from SQS:

### CloudEvents Format (Recommended)

CloudEvents provide distributed tracing via W3C Trace Context and source file tracking:

```json
{
  "specversion": "1.0",
  "type": "com.verenigingen.kbo.sync.organisation.queued",
  "source": "urn:kbo:mutations:organisation",
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "time": "2024-12-31T10:30:00Z",
  "datacontenttype": "application/json",
  "data": {
    "KboNummer": "0123456789",
    "CorrelationId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
  },
  "traceparent": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
  "sourcefilename": "pub_mut_klanten-ondernemingen_20241231.xml"
}
```

**Key Features:**
- `traceparent`: W3C trace context for distributed tracing (trace ID flows to event store headers)
- `sourcefilename`: Source file name for traceability (persisted in event store headers)
- Standard event types: `com.verenigingen.kbo.sync.{organisation|function|person}.queued`
- Automatic source type tagging: Events are tagged with `Source: "KboSync"` or `Source: "KszSync"`

**Creating CloudEvents:**

```csharp
using AssociationRegistry.KboMutations.CloudEvents;

var cloudEvent = CloudEventBuilder.KboSyncOrganisationQueued()
    .WithData(new { KboNummer = "0123456789" })
    .FromFile("pub_mut_klanten-ondernemingen_20241231.xml")
    .Build();

var json = cloudEvent.ToJson();
```

### Plain JSON Format (Legacy)

Backward compatible format without tracing metadata:

```json
{
  "KboNummer": "0123456789",
  "CorrelationId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

Or for KSZ sync:

```json
{
  "Insz": "12345678901",
  "Overleden": false,
  "CorrelationId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890"
}
```

### Metadata Flow

CloudEvent metadata flows through to the event store with automatic source type tagging:

```
CloudEvent (traceparent, sourcefilename)
  → CommandMetadata.AdditionalMetadata
    → EventStore headers (TraceId, Source, SourceFileName)
      → Marten mt_events (PostgreSQL)
```

**Event Headers Persisted:**
- `TraceId`: W3C trace context ID from CloudEvent `traceparent`
- `Source`: Sync type (`"KboSync"` or `"KszSync"`) - automatically determined
- `SourceFileName`: Original mutation file name from CloudEvent `sourcefilename`

**Query Examples:**

Query events by trace ID:
```sql
SELECT * FROM admin.mt_events
WHERE mt_headers->>'TraceId' = '00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01';
```

Query events by source type:
```sql
-- All KBO sync events
SELECT * FROM admin.mt_events
WHERE mt_headers->>'Source' = 'KboSync';

-- All KSZ sync events
SELECT * FROM admin.mt_events
WHERE mt_headers->>'Source' = 'KszSync';
```

Query events by source file:
```sql
SELECT * FROM admin.mt_events
WHERE mt_headers->>'SourceFileName' = 'pub_mut_klanten-ondernemingen_20241231.xml';
```

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using
the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools)
from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.

```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.

```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests

```
    cd "AssociationRegistry.KboMutations.SyncLambda/test/AssociationRegistry.KboMutations.SyncLambda.Tests"
    dotnet test
```

Deploy function to AWS Lambda

```
    cd "AssociationRegistry.KboMutations.SyncLambda/src/AssociationRegistry.KboMutations.SyncLambda"
    dotnet lambda deploy-function
```
