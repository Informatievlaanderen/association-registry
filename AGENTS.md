# association-registry — project rules

## Database record classes (`*Document`)

- **No nullable primitives** on Marten `*Document` classes.
  This applies to: `bool?`, `int?`, `DateOnly?`, `DateTime?`, `decimal?`, and any other value-type nullable property.
- Instead of a nullable primitive, use a default value or model absence as a separate type.
- **Only exception:** `DateTimeOffset? DeletedAt` for soft-delete (Marten `ISoftDeleted`).
