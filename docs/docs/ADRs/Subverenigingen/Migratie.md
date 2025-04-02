# Migratie van FV naar VZERs

## Nieuw doel Vereniging class
Voor de migratie hadden we 2 types verenigingen, nl feitelijke vereniging en vereniging met rechtspersoonlijkheid.

Dit resulteerde in 3 classes, respectievelijk `Vereniging`, `VerenigingMetRechtspersoonlijkheid`, en een base class `VerenigingOfAnyKind`.

Omdat FV nu een subtype is geworden van VZER, en alle FVs standaard gemigreerd werden naar VZER - subtype NB, 
hebben we ervoor gekozen om de `Vereniging` class te hergebruiken voor VZERs. 

## Eenmalige migratie

We hebben bij het opstarten van de Beheer API een migratie taak toegevoegd.

Deze zoekt alle FV streams die nog geen migratie event hadden, en gebruiken hiervoor Marten rechtstreeks. 

We gaan hier niet via de standaard command handling pipeline, omdat we hier geen nood hebben aan meer domain validatie dan simpele concurrency checks.

Concurrency checks voorkomen echter wel dat meerdere instanties van de beheer API hier voor problemen kunnen zorgen.

Na de eenmalige migratie wordt deze code verwijderd.