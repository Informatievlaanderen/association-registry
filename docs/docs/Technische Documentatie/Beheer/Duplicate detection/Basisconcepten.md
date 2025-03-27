---
title: Basisconcepten
---

# Duplicate Detection Basisconcepten

## Minimumscore

De minimumscore is een waarde die gebruikt wordt om te bepalen of een mogelijke duplicaat zal voorgelegd worden aan de gebruiker.

ElasticSearch scoort de gevonden resultaten, waarbij een hogere score staat voor een grotere gelijkenis tussen de gevonden vereniging en de te registreren vereniging.

De score kan echter wijzigen naargelang de dataset. ElasticSearch houdt bijv rekening met hoe uniek bepaalde termen zijn. Daarom laten we het toe de minimumScore te wijzigen.
