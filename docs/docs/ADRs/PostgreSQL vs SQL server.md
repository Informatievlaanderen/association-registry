---
title: PostgreSQL vs SQL server
date: 2022-10-10
---

# PostgreSQL vs SQL server

# Besproken met

- AWS team rond ondersteuning en kostprijs
- Development team

# Motivatie

We kiezen PostgreSQL omdat:
- document db is beter geschikt voor event sourced systems dan relationele db
  - relaties hebben we niet nodig
- events en projecties zijn heterogene objecten
  - dit past beter in een document db 
- PostgreSQL is goedkoper dan MSSQL 
- er bestaan beter gedocumenteerde libraries voor PostgreSQL event sourcing dan voor MSSQL event sourcing

We verkiezen Marten over SQLStreamstore omdat:

- Marten een uitgebreidere set van features aanbiedt die we nodig hebben 
- Marten ontwikkeld is voor postgresql 
- Marten actiever ontwikkeld wordt

We gebruiken Marten als ORM. 

Deze bevat functionaliteiten voor Linq queries die vertaald worden naar pgsql queries, support voor raw sql queries, en plain text queries.

Zie ook: [OR-1202: PoC  - Event sourcing architectuur, klaar voor aanmaken van een vereniging](https://vlaamseoverheid.atlassian.net/browse/OR-1202).


Postgres db Verenigingsregister voor test-omgeving en staging-omgeving:

- db.t3.medium 
- 30GB storage 
- 300GB backup storage
- Single AZ