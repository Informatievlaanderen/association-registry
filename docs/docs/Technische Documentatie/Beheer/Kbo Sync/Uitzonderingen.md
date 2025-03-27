# Kbo Sync: Uitzonderingen

Dit schema geeft de Flow Chart weer over wanneer al dan niet een Kbo Sync command naar de DLQ te sturen bij fouten.

Zie ook:
- [ADR: Foutbehandeling Bij KBO Sync](/docs/ADRs/Foutbehandeling%20Bij%20KBO%20Sync.md)
- [ADR: RegistreerInschrijving voor KBO verenigingen](/docs/ADRs/RegistreerInschrijving%20voor%20KBO%20verenigingen.md)

## Flow Chart

```mermaid
---
config:
  theme: mc
---
flowchart
    SQS -->|Triggert| KboSyncLambda
    KboSyncLambda --> Exists{Bestaat in VR?}
    Exists -->|Ja| KanGeefOnderneming{Geef onderneming werkt?}
    Exists -->|Nee| Negeren
    KanGeefOnderneming -->|Ja| LoadVereniging{Load Vereniging}
    KanGeefOnderneming -->|Nee| DLQ
    LoadVereniging -->|Ja| RegistreerInschrijving{Registreer Inschrijving}
    LoadVereniging -->|Nee| DLQ
    RegistreerInschrijving -->|Ja| SyncWithMagdaGegevens
    RegistreerInschrijving -->|Nee| DLQ
    SyncWithMagdaGegevens --> Events

```