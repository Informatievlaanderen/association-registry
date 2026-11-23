# association-registry — project rules

## Database record classes (`*Document`)

- **No nullable primitives** on Marten `*Document` classes.
  This applies to: `bool?`, `int?`, `DateOnly?`, `DateTime?`, `decimal?`, and any other value-type nullable property.
- Instead of a nullable primitive, use a default value or model absence as a separate type.
- **Only exception:** `DateTimeOffset? DeletedAt` for soft-delete (Marten `ISoftDeleted`).

## Value objects

- **Model domain concepts as value objects** (`VCode`, `RelatieType`, `Koepelstatus`, …), not as loose strings or primitives. Use them often: in commands, the domain, and wherever input is converted.
- **A value object validates its own input.** `Create`/`Parse` throws a `DomainException` with a clear message, which the API turns into a `400`.
  - Requests convert to value objects in `ToCommand`.
  - Don't repeat the format or allowed-values check in the request validator. The validator keeps what the value object cannot know, like required fields.
- **Allowed values live on the value object**: a nested `public static class Labels` with consts, static instances built from them, and `All` (see `Contactgegeventype`, `RelatieType`).
  - Messages, examples and tests reference those. Never write a copy of the values in a string, e.g. build "Lidmaatschap of Subvereniging" from `RelatieType.All`.
- **No hardcoded messages** (`const string` in validators or controllers). Messages go in `ExceptionMessages.resx` (domain) or `ValidationMessages.resx` (API), with placeholders for values.

## Domain rules

- **Rules live in a `static Create` method on the type they belong to**, which throws `DomainException`s.
  - A variant with its own rules gets its own type, not `if (type)` branches in one long method. Example: `Lidmaatschapsaanvraag` and `Subverenigingsaanvraag` under `Relatieaanvraag`.
  - The same goes for a checked party, e.g. `BeoogdeKoepel.Create` for the koepel checks.
- **Check what the request alone can tell before loading anything**, e.g. the `Partijen` value object in `ToCommand`. The domain stays protected because it only accepts the checked type.
