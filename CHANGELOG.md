# [6.6.0](https://github.com/informatievlaanderen/association-registry/compare/v6.5.0...v6.6.0) (2023-07-10)


### Features

* or-1813 make capitalization uniform among request headers ([d205b2c](https://github.com/informatievlaanderen/association-registry/commit/d205b2cfa5f93452babf8ca34c8a53a4253d1b84))

# [6.5.0](https://github.com/informatievlaanderen/association-registry/compare/v6.4.1...v6.5.0) (2023-07-07)


### Features

* or-1813 update swagger docs ([6c3a92b](https://github.com/informatievlaanderen/association-registry/commit/6c3a92ba9d5fe242b3882f88a500f6e36f5bbd37))

## [6.4.1](https://github.com/informatievlaanderen/association-registry/compare/v6.4.0...v6.4.1) (2023-07-06)


### Bug Fixes

* or-1763 fix ignoring the doelgroep when it is not provided for a wijzig basisgegevens ([f65c1d9](https://github.com/informatievlaanderen/association-registry/commit/f65c1d9e0d9853f78fd4254b1a02af6a92e6e1e8))

# [6.4.0](https://github.com/informatievlaanderen/association-registry/compare/v6.3.0...v6.4.0) (2023-07-06)


### Bug Fixes

* or-1813 add missing possible 200 status code to edit endopints ([8e1d41b](https://github.com/informatievlaanderen/association-registry/commit/8e1d41ba7e70d2cd6d972c9a09ae76621a8cda4e))
* or-1813 correct documentation on the VR-sequence in edit endpoints ([a4b6ca2](https://github.com/informatievlaanderen/association-registry/commit/a4b6ca2ea3768d2de8abac782518d92aed7ee3c1))
* or-1813 correct the summary of wijzig basisgegevens for vereniging met rechtspersoonlijkheid ([1a9b3f0](https://github.com/informatievlaanderen/association-registry/commit/1a9b3f0636f20f2fd39793f03041490123ca716b))


### Features

* or-1763 allow wijzig doelgroep as part of the wijzig basisgegevens endpoint ([5523f51](https://github.com/informatievlaanderen/association-registry/commit/5523f51eccc2254b97a099f98aa69c44316a7fef))

# [6.3.0](https://github.com/informatievlaanderen/association-registry/compare/v6.2.1...v6.3.0) (2023-07-06)


### Features

* or-1763 allow doelgroep when registreer vereniging ([963ebeb](https://github.com/informatievlaanderen/association-registry/commit/963ebeb5beb0362816c00757b4c2062784c9635b))

## [6.2.1](https://github.com/informatievlaanderen/association-registry/compare/v6.2.0...v6.2.1) (2023-07-06)


### Bug Fixes

* or-1772 add json-ld context to historiek response examples ([a240f7c](https://github.com/informatievlaanderen/association-registry/commit/a240f7c16da79e9c3cfb8af3b99ed6c1d90db916))

# [6.2.0](https://github.com/informatievlaanderen/association-registry/compare/v6.1.1...v6.2.0) (2023-07-06)


### Features

* or-1772 add json-ld context for beheer historiek ([f8c1756](https://github.com/informatievlaanderen/association-registry/commit/f8c1756b9656f170ea249c973ce26bb99bb1aa59))
* or-1815 add contexten route to the swagger documentation for beheer and publieke API ([e4cf721](https://github.com/informatievlaanderen/association-registry/commit/e4cf721198465129877d76244b0c757b2b23b82e))

## [6.1.1](https://github.com/informatievlaanderen/association-registry/compare/v6.1.0...v6.1.1) (2023-07-06)


### Bug Fixes

* or-1779 correct swagger documentation and return 200 when no changes were detected ([9134ed8](https://github.com/informatievlaanderen/association-registry/commit/9134ed8c1707fe9d16b726f28ec9b1222daac2d3))
* OR-1779 fix typo ([62f87e1](https://github.com/informatievlaanderen/association-registry/commit/62f87e19301f84bcee96b5c6366be07cb881ea5b))

# [6.1.0](https://github.com/informatievlaanderen/association-registry/compare/v6.0.0...v6.1.0) (2023-07-05)


### Bug Fixes

* or-1779 introduce IsEquivalentTo in locatie for wijzig locatie ([af73787](https://github.com/informatievlaanderen/association-registry/commit/af737877550fabb3d40329160175d168e2f0696c))


### Features

* or-1709 ensure locaties, contactgegevens and vertegenwoordigers are sorted by their Id in all projections ([2311cc1](https://github.com/informatievlaanderen/association-registry/commit/2311cc1f24b9370c68e52ef0c45537ee0f7134f6))
* or-1779 allow wijzig locatie ([621ba50](https://github.com/informatievlaanderen/association-registry/commit/621ba50fecf10664193ae8513f7da126353b6767))

# [6.0.0](https://github.com/informatievlaanderen/association-registry/compare/v5.0.1...v6.0.0) (2023-07-04)


### Features

* OR-1794 remove doelgroep and activiteiten ([8800f00](https://github.com/informatievlaanderen/association-registry/commit/8800f00dc42b6489d72de4db2e056dbb7a39f883))


### BREAKING CHANGES

* doelgroep and activiteiten are removed from the search and potential duplicates results

## [5.0.1](https://github.com/informatievlaanderen/association-registry/compare/v5.0.0...v5.0.1) (2023-07-04)


### Bug Fixes

* OR-1778 add locatieId to publiekDetailProjectie and add locaties to afdelingWerdGeregistreerd event apply ([b13ea4e](https://github.com/informatievlaanderen/association-registry/commit/b13ea4e1658411c96521fc3aa5f5fe00e7eaf778))

# [5.0.0](https://github.com/informatievlaanderen/association-registry/compare/v4.1.0...v5.0.0) (2023-07-03)


### Bug Fixes

* OR-1796 remove duplicate initiator header from swagger documentation ([72aa62c](https://github.com/informatievlaanderen/association-registry/commit/72aa62ca4f1d1930d75d7a5fd830adbe53fa28e5))


### Features

* or-1796 add initiator to all calls ([034863c](https://github.com/informatievlaanderen/association-registry/commit/034863c4e836902f424cbdcaa451549248fac2cb))


### BREAKING CHANGES

* Initiator header is now required on all requests

# [4.1.0](https://github.com/informatievlaanderen/association-registry/compare/v4.0.0...v4.1.0) (2023-07-03)


### Features

* or-1777 fix documentation + add example ([7822c89](https://github.com/informatievlaanderen/association-registry/commit/7822c89babf6a7c7e4f324f5e40fb07b283e661b))
* or-1778 update publiek detail projection ([9423266](https://github.com/informatievlaanderen/association-registry/commit/9423266f06fadfb81472d6956edf868ab3fde4d2))

# [4.0.0](https://github.com/informatievlaanderen/association-registry/compare/v3.32.0...v4.0.0) (2023-07-03)


### Features

* or-1765 add correlationId ([4217c09](https://github.com/informatievlaanderen/association-registry/commit/4217c09b67f42b0efa4da51e8dd9927189d4d7ee))
* or-1765 add correlationId to documentation ([fb57447](https://github.com/informatievlaanderen/association-registry/commit/fb57447dade9c009ee45499927504f27ae9d4125))


### BREAKING CHANGES

* all request now require x-correlation-id header

# [3.32.0](https://github.com/informatievlaanderen/association-registry/compare/v3.31.0...v3.32.0) (2023-06-30)


### Features

* or-1778 allow remove locatie (no projections) ([3f33403](https://github.com/informatievlaanderen/association-registry/commit/3f33403c43801e9dd3d6e89c0ae064ba3cab3ee1))
* or-1778 verwijderLocatie in projections ([492789c](https://github.com/informatievlaanderen/association-registry/commit/492789caeb05eead938d7dcd89a12ad1d57eccc1))

# [3.31.0](https://github.com/informatievlaanderen/association-registry/compare/v3.30.1...v3.31.0) (2023-06-30)


### Features

* OR-1806 add maatschappelijke zetel as locatietype to response models ([916cd25](https://github.com/informatievlaanderen/association-registry/commit/916cd2576f14b256e2b7160dd54a69512f84bac5))

## [3.30.1](https://github.com/informatievlaanderen/association-registry/compare/v3.30.0...v3.30.1) (2023-06-29)


### Bug Fixes

* OR-1777 fix locatieWerdToegevoegd Historiek ([b5bc769](https://github.com/informatievlaanderen/association-registry/commit/b5bc769e1484690aa2628d82edcd1f84befface9))
* or-1777 fix unit test for historiek ([b84921f](https://github.com/informatievlaanderen/association-registry/commit/b84921f26d118de8aa79d9cbc3130b497d7abc69))

# [3.30.0](https://github.com/informatievlaanderen/association-registry/compare/v3.29.1...v3.30.0) (2023-06-27)


### Features

* OR-1777 allow toevoegen locatie ([bc96195](https://github.com/informatievlaanderen/association-registry/commit/bc96195ae0f841df6ad9e96449c8192801ca2cde))

## [3.29.1](https://github.com/informatievlaanderen/association-registry/compare/v3.29.0...v3.29.1) (2023-06-23)


### Bug Fixes

* or-1776 fix null reference exception, replace adres by adresvoorstelling in publiek en beheer zoek ([6fd7cfb](https://github.com/informatievlaanderen/association-registry/commit/6fd7cfbc039e262e57cbccd5f8977abae6af6504))

# [3.29.0](https://github.com/informatievlaanderen/association-registry/compare/v3.28.3...v3.29.0) (2023-06-22)


### Bug Fixes

* or-1776 don't compare adressen and adresIds when they are null; rename adresvoorstelling ([9e7bfc8](https://github.com/informatievlaanderen/association-registry/commit/9e7bfc8239acb7e36d1712e70175fb9894e3c0f4))
* or-1776 fix reference to AdresId ([f870e34](https://github.com/informatievlaanderen/association-registry/commit/f870e343a093a6c7b4a037d18ca61e6c1da80bb2))


### Features

* or-1776 add  adresid to publiek detail ([fdb2662](https://github.com/informatievlaanderen/association-registry/commit/fdb2662f42b01bd3b570bb1d92b68ac111021ced))
* or-1776 add adresId to beheer detail ([a0cfd6f](https://github.com/informatievlaanderen/association-registry/commit/a0cfd6f119d7b30602d038a20ca1144120f4b186))
* or-1776 add AdresId to Locatie and replace Hoofdlocatie by IsPrimair ([dfa40d9](https://github.com/informatievlaanderen/association-registry/commit/dfa40d95f49ee7d8728e61627e398e88ef4e298e))
* or-1776 add AdresId to registreer FV request [WIP ([5cb4dc7](https://github.com/informatievlaanderen/association-registry/commit/5cb4dc7c454405d4589a67b8653e5af709cac53f))
* or-1776 add bron to responses ([7dc5842](https://github.com/informatievlaanderen/association-registry/commit/7dc5842ab8904684253556399610b7acb5dffbfd))
* or-1776 merge branches back together ([42b1de7](https://github.com/informatievlaanderen/association-registry/commit/42b1de7782af432c08074db217e2db5c66506a7d))
* or-1776 merge with projections ([b931ff1](https://github.com/informatievlaanderen/association-registry/commit/b931ff154c01c0655809e097e1350265937e5a97))
* or-1776 setup adres event contract ([407ca8b](https://github.com/informatievlaanderen/association-registry/commit/407ca8b00d5bf08785704246cff6fb82bfbef962))

## [3.28.3](https://github.com/informatievlaanderen/association-registry/compare/v3.28.2...v3.28.3) (2023-06-21)


### Bug Fixes

* or-1775 remove locatieId from publiek detail ([72d09d1](https://github.com/informatievlaanderen/association-registry/commit/72d09d10a2c826ebd14c7e32009b8ceb20d7acbf))

## [3.28.2](https://github.com/informatievlaanderen/association-registry/compare/v3.28.1...v3.28.2) (2023-06-20)


### Bug Fixes

* or-1775 remove locatieId from beheer zoek, publiek zoek en detail projections ([a62175b](https://github.com/informatievlaanderen/association-registry/commit/a62175b2cbed7dc4ba2491e0976c8eb6d9ded423))

## [3.28.1](https://github.com/informatievlaanderen/association-registry/compare/v3.28.0...v3.28.1) (2023-06-20)


### Bug Fixes

* or-1701 apply isUitgeschrevenUitPubliekeDatastroom to verenigingState ([de656f9](https://github.com/informatievlaanderen/association-registry/commit/de656f9bce40f69dd4f237ab25e590924d25bbd6))

# [3.28.0](https://github.com/informatievlaanderen/association-registry/compare/v3.27.0...v3.28.0) (2023-06-19)


### Features

* or-1780 remove InPubliekeStroom for Afdeling ([8b66279](https://github.com/informatievlaanderen/association-registry/commit/8b662798c0971bbd21d62ce2d635d2d40df05f10))

# [3.27.0](https://github.com/informatievlaanderen/association-registry/compare/v3.26.0...v3.27.0) (2023-06-19)


### Features

* or-1775 add locatieId ([ee0b38b](https://github.com/informatievlaanderen/association-registry/commit/ee0b38b7c54f49203ee6284cdeed24c166b388f9))

# [3.26.0](https://github.com/informatievlaanderen/association-registry/compare/v3.25.0...v3.26.0) (2023-06-19)


### Bug Fixes

* or-1717 adjust swagger collections ([6188989](https://github.com/informatievlaanderen/association-registry/commit/6188989edbaa89ce4713a987347ba2020291fc17))


### Features

* or-1717 add validator for pagination query params (admin) ([1431d7b](https://github.com/informatievlaanderen/association-registry/commit/1431d7b358e3b8ef37d9dd98cd08a6f9bc012e57))
* or-1717 add validator for pagination query params (public) ([298dbb0](https://github.com/informatievlaanderen/association-registry/commit/298dbb0a8250c5e36e6726951bf4834933627590))

# [3.25.0](https://github.com/informatievlaanderen/association-registry/compare/v3.24.0...v3.25.0) (2023-06-19)


### Features

* or-1701 apply IsUitgeschrevenUitPubliekeStroom to beheer projecties ([50f941e](https://github.com/informatievlaanderen/association-registry/commit/50f941eafef71ec31581e3527e27cdeaf682fd27))

# [3.24.0](https://github.com/informatievlaanderen/association-registry/compare/v3.23.0...v3.24.0) (2023-06-19)


### Bug Fixes

* or-1747 modify detail link to gerelateerde vereniging to string and empty string when no vCode is known ([9069aaa](https://github.com/informatievlaanderen/association-registry/commit/9069aaa7d2ef5d1ceffe0109b80759077943cc69))


### Features

* or-1701 only show public association in publiek detail ([572842e](https://github.com/informatievlaanderen/association-registry/commit/572842e585099d3b11ca063d8fceddbe0779e933))

# [3.23.0](https://github.com/informatievlaanderen/association-registry/compare/v3.22.0...v3.23.0) (2023-06-16)


### Features

* or-1701 allow patch to be executed with only IsUitgeschrevenUitPubliekeDatastroom ([5cb5d01](https://github.com/informatievlaanderen/association-registry/commit/5cb5d01632dbf098e8a159795fad7e59314aaa62))
* or-1701 project IsUitgeschrevenUitPubliekStroom from registratie events ([5b2ce2d](https://github.com/informatievlaanderen/association-registry/commit/5b2ce2dbb34a65fb97aa8e7c3752c9b86c19a7bb))

# [3.22.0](https://github.com/informatievlaanderen/association-registry/compare/v3.21.1...v3.22.0) (2023-06-16)


### Features

* or-1422 build admin projectionhost ([5b83c89](https://github.com/informatievlaanderen/association-registry/commit/5b83c89e57a8e7482610085c83fb83ec7f8783e4))
* or-1422 fix assembly location ([a1381ca](https://github.com/informatievlaanderen/association-registry/commit/a1381caf6e7200c875b07b5ccb05a17992a0b5f3))
* or-1422 split projectionhost from admin api ([9a48f11](https://github.com/informatievlaanderen/association-registry/commit/9a48f11ef5287868dacdbda0d3941d68580da213))
* or-1422 unify projection hosts; fix wolverine namespace issue ([330e846](https://github.com/informatievlaanderen/association-registry/commit/330e84666612e34c520366feefd1b3207f334849))

## [3.21.1](https://github.com/informatievlaanderen/association-registry/compare/v3.21.0...v3.21.1) (2023-06-15)


### Bug Fixes

* or-1747 do add detail link when there is no vCode for the gerelateerde vereniging ([8c48688](https://github.com/informatievlaanderen/association-registry/commit/8c486883cf5c9eb58387ec441aecc58402880384))

# [3.21.0](https://github.com/informatievlaanderen/association-registry/compare/v3.20.4...v3.21.0) (2023-06-15)


### Bug Fixes

* or-1717 fix workflow ([76681ca](https://github.com/informatievlaanderen/association-registry/commit/76681ca6752f7492bc481efc8815c4029faf457e))


### Features

* or-1717 copy publiek zoeken naar beheer zoeken ([8a9aacf](https://github.com/informatievlaanderen/association-registry/commit/8a9aacfece690f1dea239a488c5854be46cecc30))
* or-1717 remove facets from beheer zoeken ([ea4d1ec](https://github.com/informatievlaanderen/association-registry/commit/ea4d1ece88dbff63b5cc055fabe79445f62245cc))
* or-1717 update workflow ([587bfb3](https://github.com/informatievlaanderen/association-registry/commit/587bfb32f1d7d789c6b44ba08f5ab8832a8026c8))

## [3.20.4](https://github.com/informatievlaanderen/association-registry/compare/v3.20.3...v3.20.4) (2023-06-15)


### Bug Fixes

* or-1747 add detail link to gerelateerde vereniging ([b278f94](https://github.com/informatievlaanderen/association-registry/commit/b278f9426140b8f05fd5e741b13a4cef1383f706))

## [3.20.3](https://github.com/informatievlaanderen/association-registry/compare/v3.20.2...v3.20.3) (2023-06-14)


### Bug Fixes

* or-1762 add status 200 and location header in case of 200 to swagger documentation ([4218073](https://github.com/informatievlaanderen/association-registry/commit/42180730187369a10c398ef2fd4ae688926a6019))

## [3.20.2](https://github.com/informatievlaanderen/association-registry/compare/v3.20.1...v3.20.2) (2023-06-13)


### Bug Fixes

* or-1762 correct location header of duplicate KBO response ([747b865](https://github.com/informatievlaanderen/association-registry/commit/747b865497d467d56d78580b32ee08ae81d904c1))

## [3.20.1](https://github.com/informatievlaanderen/association-registry/compare/v3.20.0...v3.20.1) (2023-06-13)


### Bug Fixes

* or-1757 add json ld context to responseExample of zoek in the public API ([d783348](https://github.com/informatievlaanderen/association-registry/commit/d783348a79a10680a2e54eda972a385c2a61077b))

# [3.20.0](https://github.com/informatievlaanderen/association-registry/compare/v3.19.0...v3.20.0) (2023-06-13)


### Features

* or-1701 add opt-out of publieke datastroom to basisgegevens of a vereniging ([b86bb97](https://github.com/informatievlaanderen/association-registry/commit/b86bb9721a681da13bbfa4e7c3bed13ec21e3d4c))

# [3.19.0](https://github.com/informatievlaanderen/association-registry/compare/v3.18.0...v3.19.0) (2023-06-12)


### Bug Fixes

* rename json ld context for zoeken in beheer API ([88a6ceb](https://github.com/informatievlaanderen/association-registry/commit/88a6ceb1faee429b1e5618433f6b2a73e1c54594))


### Features

* or-1757 add json ld context to beheer detail ([f479f1a](https://github.com/informatievlaanderen/association-registry/commit/f479f1aec02ac80031f266c14dd64d2e464846eb))
* or-1757 add Json-LD context to zoeken in public API ([8980d1b](https://github.com/informatievlaanderen/association-registry/commit/8980d1b0b4fd8b1bef1fdee82f30ca62084e9b8e))

# [3.18.0](https://github.com/informatievlaanderen/association-registry/compare/v3.17.1...v3.18.0) (2023-06-12)


### Bug Fixes

* or-1762 fix rebase errors ([4d9a2eb](https://github.com/informatievlaanderen/association-registry/commit/4d9a2eb5bd9a22dcd3385bac4c810d2a96067b07))


### Features

* or-1762 detect duplicates from kbo ([c02612d](https://github.com/informatievlaanderen/association-registry/commit/c02612d83a13434cf106fe91b0183b14fba6f555))

## [3.17.1](https://github.com/informatievlaanderen/association-registry/compare/v3.17.0...v3.17.1) (2023-06-12)


### Bug Fixes

* or-1747 add afdeling relaties to publiek detail projection ([03f27f2](https://github.com/informatievlaanderen/association-registry/commit/03f27f284ef7b71d9f93f76d4fb9dd93f1717eec))
* or-1747 add historiekGebeurtenis for moeder of AfdelingWerdGeregstreerd ([7c60e77](https://github.com/informatievlaanderen/association-registry/commit/7c60e774828ec2dd946d8717905bb943403ab50b))
* or-1747 correct BasisBeheerUri for verenigingen met rechtspersoonlijkheid ([6b3a855](https://github.com/informatievlaanderen/association-registry/commit/6b3a855e26af6ea001773e6ad0b369d3204fbedf))

# [3.17.0](https://github.com/informatievlaanderen/association-registry/compare/v3.16.0...v3.17.0) (2023-06-08)


### Features

* or-1747 add relatie to moeder when afdelingWerdGeregistreerd ([9bc6cf8](https://github.com/informatievlaanderen/association-registry/commit/9bc6cf8a2a8a6994d61a1bfd3c65b2b810f29099))
* or-1747 use batchSize of 1 to prevent missing updates ([77c7aba](https://github.com/informatievlaanderen/association-registry/commit/77c7abaf15aed4f27beb545ccedf61f97dfdc7ea))

# [3.16.0](https://github.com/informatievlaanderen/association-registry/compare/v3.15.1...v3.16.0) (2023-06-07)


### Features

* or-1715 add duplicate detection to RegistreerAfdeling ([fe55a2b](https://github.com/informatievlaanderen/association-registry/commit/fe55a2bc0ef250df5d4fb3b35a519c5078ca90ee))

## [3.15.1](https://github.com/informatievlaanderen/association-registry/compare/v3.15.0...v3.15.1) (2023-06-06)


### Bug Fixes

* or-1715 add ACM projection for AfdelingWerdGeregistreerd event ([14002c4](https://github.com/informatievlaanderen/association-registry/commit/14002c4a7986583ea6793515652a65211917d1ce))

# [3.15.0](https://github.com/informatievlaanderen/association-registry/compare/v3.14.0...v3.15.0) (2023-06-06)


### Features

* or-1715 add relaties to publiek ([24e3005](https://github.com/informatievlaanderen/association-registry/commit/24e30056cd075ab6d8dbd275cc2e46d2ea69b232))
* or-1715 expand relation with more info on AndereVereniging ([7f0956c](https://github.com/informatievlaanderen/association-registry/commit/7f0956c11cbb4b278f84eef2d3446b3f755dc018))
* or-1715 registreer afdeling ([0fbc544](https://github.com/informatievlaanderen/association-registry/commit/0fbc54458351a0efdefe95ada48c92e17f249649))

# [3.14.0](https://github.com/informatievlaanderen/association-registry/compare/v3.13.0...v3.14.0) (2023-06-02)


### Features

* or-1696 remove insz from event data when VertegenwoordigerWerdVerwijderd ([e9cb944](https://github.com/informatievlaanderen/association-registry/commit/e9cb944bcac6e98baf67ef2b71674e01e2513c92))


### Reverts

* Revert "build(deps): bump slackapi/slack-github-action from 1.23.0 to 1.24.0" ([57a50e4](https://github.com/informatievlaanderen/association-registry/commit/57a50e420b1fc6df27954c0d1f88bf54be2c77cd))

# [3.13.0](https://github.com/informatievlaanderen/association-registry/compare/v3.12.0...v3.13.0) (2023-06-01)


### Features

* or-1696 remove insz from historiek ([521b140](https://github.com/informatievlaanderen/association-registry/commit/521b14020c27ea6c1cdae4f3c784e6f5e5cd0ed8))

# [3.12.0](https://github.com/informatievlaanderen/association-registry/compare/v3.11.0...v3.12.0) (2023-05-31)


### Features

* or-1760 allow voornaam en achternaam when creating a vertegenwoordiger ([570cb8f](https://github.com/informatievlaanderen/association-registry/commit/570cb8fb24a4d0cdaa0084e2900b2771168bc223))

# [3.11.0](https://github.com/informatievlaanderen/association-registry/compare/v3.10.0...v3.11.0) (2023-05-31)


### Features

* or-1725 add exception message when action is started on wrong verenigingstype ([68b954b](https://github.com/informatievlaanderen/association-registry/commit/68b954b74465df8651e45e9d5d4d7f2c3a90c4c1))

# [3.10.0](https://github.com/informatievlaanderen/association-registry/compare/v3.9.0...v3.10.0) (2023-05-30)


### Features

* or-1696 remove insz from beheerd detail ([0907a67](https://github.com/informatievlaanderen/association-registry/commit/0907a67ba62053132c2cdcdc4d6fde9172824022))

# [3.9.0](https://github.com/informatievlaanderen/association-registry/compare/v3.8.0...v3.9.0) (2023-05-30)


### Features

* or-1725 add tests for new code ([cfcd67e](https://github.com/informatievlaanderen/association-registry/commit/cfcd67e1589098cdb0e6cec894a04fade3e7d38e))
* or-1725 WIP splits feitelijke ven rechtspersoonlijkheid ([c5068a0](https://github.com/informatievlaanderen/association-registry/commit/c5068a08fbd5f73501f0164f914067970612443e))

# [3.8.0](https://github.com/informatievlaanderen/association-registry/compare/v3.7.0...v3.8.0) (2023-05-30)


### Features

* or-1758 remove license from admin api ([1343327](https://github.com/informatievlaanderen/association-registry/commit/13433272d883bdffe1ee93f044490238a88e951f))

# [3.7.0](https://github.com/informatievlaanderen/association-registry/compare/v3.6.0...v3.7.0) (2023-05-23)


### Features

* or-1702 add KBO basisgegevens fields to the WerdGeregistreerd event with hardcoded or empty values ([4428fb0](https://github.com/informatievlaanderen/association-registry/commit/4428fb0e34c29e55ff145ceddcf8755f4e6749b4))

# [3.6.0](https://github.com/informatievlaanderen/association-registry/compare/v3.5.0...v3.6.0) (2023-05-22)


### Bug Fixes

* or-1702 revert experimental code ([d36f102](https://github.com/informatievlaanderen/association-registry/commit/d36f102c8d9f2affb755d1af7668592ddd487d17))


### Features

* or-1702 registreer from kbo ([effaf43](https://github.com/informatievlaanderen/association-registry/commit/effaf4372ab9aee56e7aa313a163169d91aeb1ee))


### Reverts

* Revert "Revert "feat: or-1702 registreer from kbo"" ([ec3f2b0](https://github.com/informatievlaanderen/association-registry/commit/ec3f2b0176aff0ee54fd9350c280dba3bbc5f266))
* Revert "feat: or-1702 registreer from kbo" ([7700531](https://github.com/informatievlaanderen/association-registry/commit/77005316ef678beb997f5b2c68b1b7a27b89bcf2))

# [3.5.0](https://github.com/informatievlaanderen/association-registry/compare/v3.4.5...v3.5.0) (2023-05-16)


### Bug Fixes

* or-1704 add detail examples ([2b08860](https://github.com/informatievlaanderen/association-registry/commit/2b088601768abf3d8bb34126db3e12bcddeb2e32))
* or-1704 fix typo ([dfafb86](https://github.com/informatievlaanderen/association-registry/commit/dfafb86f1c140e809ffb9efcc5b19123bd5e953c))


### Features

* or-1704 add verenigingstype to projections ([737450b](https://github.com/informatievlaanderen/association-registry/commit/737450b7f2bab642c68376636bf51329f733ca73))
* or-1704 remove kbo number from feitelijke vereniging ([a89e9e8](https://github.com/informatievlaanderen/association-registry/commit/a89e9e8427fc2e8515bdd0bccee61bae0ea6bdb2))
* or-1704 rename event to FeitelijkeVerenigingWerdGeregistreerd ([455e957](https://github.com/informatievlaanderen/association-registry/commit/455e9574a45e6a462fba35a85a19dadb10bb1e84))
* or-1704 rename everything to FeitelijkeVerenigingWerdGeregistreerd ([4b9ce86](https://github.com/informatievlaanderen/association-registry/commit/4b9ce869df9b336f842c32a7a886e1a15fd33f33))

## [3.4.5](https://github.com/informatievlaanderen/association-registry/compare/v3.4.4...v3.4.5) (2023-05-09)


### Bug Fixes

* or-1699 make historiek message uniform ([461e5de](https://github.com/informatievlaanderen/association-registry/commit/461e5dee09cb35f1cbde1ef183fd6b1b14dd27ba))

## [3.4.4](https://github.com/informatievlaanderen/association-registry/compare/v3.4.3...v3.4.4) (2023-05-08)


### Bug Fixes

* or-1699 show naam and voornaam in historiek projection ([9477b76](https://github.com/informatievlaanderen/association-registry/commit/9477b7676174366e4b296f22de6445693b92964d))

## [3.4.3](https://github.com/informatievlaanderen/association-registry/compare/v3.4.2...v3.4.3) (2023-05-08)


### Bug Fixes

* or-1713 add middleware for UnexpectedAggregateVersion ([ba41527](https://github.com/informatievlaanderen/association-registry/commit/ba41527b3e2dc7be122b59c4564b78c91034c70c))

## [3.4.2](https://github.com/informatievlaanderen/association-registry/compare/v3.4.1...v3.4.2) (2023-05-08)


### Bug Fixes

* or-1710 add 412 response code to api documentation for contactgegevens and vertegenwoordigers ([0435472](https://github.com/informatievlaanderen/association-registry/commit/04354722c23efe79cce817318874c884d4c27125))

## [3.4.1](https://github.com/informatievlaanderen/association-registry/compare/v3.4.0...v3.4.1) (2023-05-04)


### Bug Fixes

* or-1699 add missing api documentation ([fbbcae3](https://github.com/informatievlaanderen/association-registry/commit/fbbcae32b63f3644b135695cd3fe1a2f68532dcc))

# [3.4.0](https://github.com/informatievlaanderen/association-registry/compare/v3.3.0...v3.4.0) (2023-05-04)


### Features

* or-1678 add the initiator in the header for http verbs post, put and patch ([f4a6248](https://github.com/informatievlaanderen/association-registry/commit/f4a624820a266b35feced0fb0e1279f2386f4009))

# [3.3.0](https://github.com/informatievlaanderen/association-registry/compare/v3.2.0...v3.3.0) (2023-05-04)


### Features

* or-1699 Patch endpoint vertegenwoordigers ([0d22cf2](https://github.com/informatievlaanderen/association-registry/commit/0d22cf25f2b3201058df8084d2ebea4380e6d785))

# [3.2.0](https://github.com/informatievlaanderen/association-registry/compare/v3.1.0...v3.2.0) (2023-05-04)


### Features

* or-1706 add pre-build step to workflow ([9953dbe](https://github.com/informatievlaanderen/association-registry/commit/9953dbe7402f9ae44452fd92a78ec5cf6bfc1886))

# [3.1.0](https://github.com/informatievlaanderen/association-registry/compare/v3.0.1...v3.1.0) (2023-05-03)


### Features

* or-1706 use auto type load mode, but don't optimize ([ce2e6b1](https://github.com/informatievlaanderen/association-registry/commit/ce2e6b1aa2004e2ebcf454d9816f297f1597beac))

## [3.0.1](https://github.com/informatievlaanderen/association-registry/compare/v3.0.0...v3.0.1) (2023-05-03)


### Reverts

* Revert "feat: or-1706 optimize artifact flow for marten" ([06ac1b4](https://github.com/informatievlaanderen/association-registry/commit/06ac1b4f78d2578d4740ec9a7a784d03826c05e8))

# [3.0.0](https://github.com/informatievlaanderen/association-registry/compare/v2.10.2...v3.0.0) (2023-05-03)


### Features

* or-1678 move initiator field from request to vr-initiator header ([a83e76c](https://github.com/informatievlaanderen/association-registry/commit/a83e76cd50f47d3ecad8d1b64b2a43e0bbdb835d))
* or-1706 optimize artifact flow for marten ([51686bb](https://github.com/informatievlaanderen/association-registry/commit/51686bbaf7c5c99fd9669a0985c9fa85f62413d2))


### BREAKING CHANGES

* requests without vr-initiator header will fail
validation.

The initiator field in the request will be ignored.

## [2.10.2](https://github.com/informatievlaanderen/association-registry/compare/v2.10.1...v2.10.2) (2023-04-27)

## [2.10.1](https://github.com/informatievlaanderen/association-registry/compare/v2.10.0...v2.10.1) (2023-04-27)


### Bug Fixes

* or-1683 fix example namespace ([98ed8cb](https://github.com/informatievlaanderen/association-registry/commit/98ed8cbb2b8234109c4a11286bc9d8a30307d8de))

# [2.10.0](https://github.com/informatievlaanderen/association-registry/compare/v2.9.0...v2.10.0) (2023-04-27)


### Features

* or-1698 validate initiator ([eb0d9c7](https://github.com/informatievlaanderen/association-registry/commit/eb0d9c73b3b45e9fd3c1a5750e7181d303d78b34))

# [2.9.0](https://github.com/informatievlaanderen/association-registry/compare/v2.8.1...v2.9.0) (2023-04-26)


### Bug Fixes

* or-1697 add telefoon to vertegenwoordiger detail ([2949edd](https://github.com/informatievlaanderen/association-registry/commit/2949edd8c312376d8ccc487af63c1a650d3478c7))
* or-1697 add voornaam and achternaam to vertegenwoordiger from magda service ([5aa9acc](https://github.com/informatievlaanderen/association-registry/commit/5aa9accd0d1c0d3408431a05c709bf81ea021a88))


### Features

* or-1698 allow vertegenwoordiger to be removed from a vereniging ([89b6386](https://github.com/informatievlaanderen/association-registry/commit/89b6386909e82c73686e3e454aa75dc5c630164d))

## [2.8.1](https://github.com/informatievlaanderen/association-registry/compare/v2.8.0...v2.8.1) (2023-04-26)


### Bug Fixes

* or-1683 add summary documentation to acm api and beheer api where it was missing ([6382e28](https://github.com/informatievlaanderen/association-registry/commit/6382e28adfd277b8cf2055838a2863c855936348))

# [2.8.0](https://github.com/informatievlaanderen/association-registry/compare/v2.7.0...v2.8.0) (2023-04-26)


### Features

* or-1697 add validation ([6f11d37](https://github.com/informatievlaanderen/association-registry/commit/6f11d3725483e1283b9b3d81ce45d20ddb2661e9))
* or-1697 allow adding a new vertegenwoordiger to vereniging ([1b107e5](https://github.com/informatievlaanderen/association-registry/commit/1b107e5972891b827b4225eb6f00148e6542bdba))
* or-1697 update acm projection with added vertegenwoordiger ([29bbd33](https://github.com/informatievlaanderen/association-registry/commit/29bbd33b848dadf499da48c82e9dc48a25dd9405))

# [2.7.0](https://github.com/informatievlaanderen/association-registry/compare/v2.6.1...v2.7.0) (2023-04-26)


### Features

* or-1676 rework intro for public api ([8cfb6b9](https://github.com/informatievlaanderen/association-registry/commit/8cfb6b9dbeacccb8e22b81936778d207d99015f0))

## [2.6.1](https://github.com/informatievlaanderen/association-registry/compare/v2.6.0...v2.6.1) (2023-04-25)


### Bug Fixes

* or-1692 modified publiek zoeken to also always show hoofdlocatie field and also always show isPrimair field for contactgegevens ([15a25e1](https://github.com/informatievlaanderen/association-registry/commit/15a25e1adc5b398b88163d897f561017b0c9a677))

# [2.6.0](https://github.com/informatievlaanderen/association-registry/compare/v2.5.5...v2.6.0) (2023-04-25)


### Features

* or-1695 use local identifier i/o insz to identify vertegenwoordigers ([fa36d7b](https://github.com/informatievlaanderen/association-registry/commit/fa36d7b99e0d120bbc41bf621b68453284d06b14))

## [2.5.5](https://github.com/informatievlaanderen/association-registry/compare/v2.5.4...v2.5.5) (2023-04-25)


### Bug Fixes

* or-1683 add documentation for metadata fields in zoeken ([271c2da](https://github.com/informatievlaanderen/association-registry/commit/271c2da9c35a2f5623dbcf57e4528f8dcbb3bb98))

## [2.5.4](https://github.com/informatievlaanderen/association-registry/compare/v2.5.3...v2.5.4) (2023-04-25)


### Bug Fixes

* or-1676 add more indents in intro ([14f8351](https://github.com/informatievlaanderen/association-registry/commit/14f835153bb2d75db0b4ced1bfe8ca633093d2a4))

## [2.5.3](https://github.com/informatievlaanderen/association-registry/compare/v2.5.2...v2.5.3) (2023-04-24)


### Bug Fixes

* or-1683 add clarification of the codes van de hoofdactiviteitenVerenigingsloket ([fd10fc1](https://github.com/informatievlaanderen/association-registry/commit/fd10fc1525ac1d51736175bbae1f6f6ef41d3420))
* or-1683 add default pargination params ([f210eca](https://github.com/informatievlaanderen/association-registry/commit/f210eca57ffef0b622b95a14a77504420c0d2207))
* or-1683 add documentation to ACM an Publiek zoeken/hoofdactiviteiten ([cff230a](https://github.com/informatievlaanderen/association-registry/commit/cff230a10d5a0d4f9396bb51af251870f12e8cc4))
* or-1683 Add the clarification that we expect a list of codes for the HoofdactiviteitenVerenigingsloket ([b70f272](https://github.com/informatievlaanderen/association-registry/commit/b70f272177b14468f3b5473dc643d232992a176e))
* or-1683 make all necessary changes to the API documentation of publiek and beheer ([77151eb](https://github.com/informatievlaanderen/association-registry/commit/77151eb79d860fa4aa049dc4b5c2968bab7c2ae4))
* or-1683 modify description of property MogelijkeDuplicateVerenigingen ([970d9d4](https://github.com/informatievlaanderen/association-registry/commit/970d9d4323464405f745b7fe186bb84c47407c76))
* or-1683 readded problemdetails usings ([209dbad](https://github.com/informatievlaanderen/association-registry/commit/209dbad812b429321b603842abbb6d8fbec62f0c))

## [2.5.2](https://github.com/informatievlaanderen/association-registry/compare/v2.5.1...v2.5.2) (2023-04-24)


### Bug Fixes

* or-1676 remove script from api documentation ([609ea9c](https://github.com/informatievlaanderen/association-registry/commit/609ea9c13f08abb3d1b1ba604546fcc33846e33c))

## [2.5.1](https://github.com/informatievlaanderen/association-registry/compare/v2.5.0...v2.5.1) (2023-04-24)


### Bug Fixes

* or-1694 modify request validation to allow for updating only hoofdactiviteiten ([240fe77](https://github.com/informatievlaanderen/association-registry/commit/240fe774ec976f2bf57d343ebab281df8dcd581a))

# [2.5.0](https://github.com/informatievlaanderen/association-registry/compare/v2.4.1...v2.5.0) (2023-04-24)


### Bug Fixes

* or-1694 fix typos ([70eae35](https://github.com/informatievlaanderen/association-registry/commit/70eae3598c2b6432e3cb46a22137b93fa8c092a3))


### Features

* or-1694 allow PUT hoofdactiviteitVerenigingsloket ([d173528](https://github.com/informatievlaanderen/association-registry/commit/d173528ebc23015cf619242ccc9754cd90eb8eb7))
* or-1694 remove tryCatch and fix test ([df443fa](https://github.com/informatievlaanderen/association-registry/commit/df443fa4d31b300c601a5ea56798cab3d6c3877e))
* or-1694 update projecties ([dcf72a6](https://github.com/informatievlaanderen/association-registry/commit/dcf72a62d6716a0fd0bf6727cff6dd9f56e8e3af))

## [2.4.1](https://github.com/informatievlaanderen/association-registry/compare/v2.4.0...v2.4.1) (2023-04-24)


### Bug Fixes

* or-1692 add hoofdlocatie property also when the value is false ([c365fb8](https://github.com/informatievlaanderen/association-registry/commit/c365fb8eb9b2bb2e514860def187e01056ed3448))

# [2.4.0](https://github.com/informatievlaanderen/association-registry/compare/v2.3.2...v2.4.0) (2023-04-24)


### Bug Fixes

* or-1676 clarify and correct documentation ([a3e284b](https://github.com/informatievlaanderen/association-registry/commit/a3e284b4d623205ef012c745ab38fad0cf1d8928))


### Features

* OR-1676 fix huge spacing between documentation sections ([84d2bf4](https://github.com/informatievlaanderen/association-registry/commit/84d2bf46cb8b3244810f0575628589cdf615fd38))
* OR-1676 remove redundant sentence ([4c65e10](https://github.com/informatievlaanderen/association-registry/commit/4c65e106b117e02b4dc073c8ebb22595409a8f1e))
* OR-1676 replace moet ([03e3d1c](https://github.com/informatievlaanderen/association-registry/commit/03e3d1cc7ed70d34f18b3a8a481bf0eaa0f37b79))
* OR-1676 replace u by je ([c12a406](https://github.com/informatievlaanderen/association-registry/commit/c12a4069465ba20ef33c76df3a0574c8622d3478))

## [2.3.2](https://github.com/informatievlaanderen/association-registry/compare/v2.3.1...v2.3.2) (2023-04-19)


### Bug Fixes

* or-1631 error responses now all use the same types in beheer API ([585f027](https://github.com/informatievlaanderen/association-registry/commit/585f0277e61ca628c3f4cf6cf72c711a476e95c6))

## [2.3.1](https://github.com/informatievlaanderen/association-registry/compare/v2.3.0...v2.3.1) (2023-04-19)


### Bug Fixes

* or-1641 replace historiek data objects with events ([32bda32](https://github.com/informatievlaanderen/association-registry/commit/32bda32979049b764e05316d0eb1e66b9356ffa3))
* or-1641 use scenario without contactgegevens to test verwijderContactgegevenWithUnknownId ([204a5c7](https://github.com/informatievlaanderen/association-registry/commit/204a5c73b29c1ff8efea4dee952321f4d7fa645a))

# [2.3.0](https://github.com/informatievlaanderen/association-registry/compare/v2.2.0...v2.3.0) (2023-04-19)


### Bug Fixes

* or-1676 reduce huge margins in public api docs ([14be45b](https://github.com/informatievlaanderen/association-registry/commit/14be45b82e93a47e353342b47b0d13189c08f86a))


### Features

* OR-1266 add list of possible exceptions ([043f049](https://github.com/informatievlaanderen/association-registry/commit/043f04911f15f988abefd2dff0a94a22fed61e8b))
* OR-1266 roll back margins fix; add break after Foutmeldingen ([310b742](https://github.com/informatievlaanderen/association-registry/commit/310b74275bb8deed3f7be9fde94d4100b3ba49f6))
* or-1676 add apiDocs config to ACM api ([0f2af52](https://github.com/informatievlaanderen/association-registry/commit/0f2af523377fafcff58530900edf5d266b923179))
* or-1676 add info around api keys to public api docs ([58cf20b](https://github.com/informatievlaanderen/association-registry/commit/58cf20b4036eb06a240386c4e68d49c4edfd5559))
* OR-1676 add license to public api docs ([223749e](https://github.com/informatievlaanderen/association-registry/commit/223749e9f4bbd9b35261b6e714f2f33def81de0a))
* OR-1676 add settings to test projects ([6c1c3c7](https://github.com/informatievlaanderen/association-registry/commit/6c1c3c7db86b22c40fd5ffe44bc186e1b18ebeb1))
* or-1676 add swagger config to appsettings ([3dfc522](https://github.com/informatievlaanderen/association-registry/commit/3dfc522a23e7301f4e05a2095c285af4c1d07925))

# [2.2.0](https://github.com/informatievlaanderen/association-registry/compare/v2.1.3...v2.2.0) (2023-04-18)


### Bug Fixes

* or-1683 use classes for response in favor of records ([440cee9](https://github.com/informatievlaanderen/association-registry/commit/440cee9451e48f1b328bc4264aa0981b52eba9bd))


### Features

* or-1683 add documentation to all endpoint requests and responses where it was missing ([fa727b2](https://github.com/informatievlaanderen/association-registry/commit/fa727b213cc5a38b106fe963a650784d520b270d))

## [2.1.3](https://github.com/informatievlaanderen/association-registry/compare/v2.1.2...v2.1.3) (2023-04-18)


### Bug Fixes

* or-1689 fix typos in descriptions of Hoofdactiviteiten ([538837b](https://github.com/informatievlaanderen/association-registry/commit/538837bd44a734d8267d2421afcd6b10e22ad4d2))

## [2.1.2](https://github.com/informatievlaanderen/association-registry/compare/v2.1.1...v2.1.2) (2023-04-18)


### Bug Fixes

* or-1681 no error on create ([07f16a7](https://github.com/informatievlaanderen/association-registry/commit/07f16a7fac5134297267f545f77b5e9fbee9f821))

## [2.1.1](https://github.com/informatievlaanderen/association-registry/compare/v2.1.0...v2.1.1) (2023-04-18)


### Bug Fixes

* or-1681 modify all projections to return empty string when there is no value instead of null ([abe84b2](https://github.com/informatievlaanderen/association-registry/commit/abe84b2c9396e9de85870e5c2e807023104b1b39))

# [2.1.0](https://github.com/informatievlaanderen/association-registry/compare/v2.0.1...v2.1.0) (2023-04-17)


### Features

* or-1680 rework contactgegevens for vertegenwoordigers ([7262012](https://github.com/informatievlaanderen/association-registry/commit/72620120f8b428101eceec57b19dee90d41aaf9a))

## [2.0.1](https://github.com/informatievlaanderen/association-registry/compare/v2.0.0...v2.0.1) (2023-04-17)


### Bug Fixes

* or-1682 add StartdatumMustNotBeInFuture rule to wijzigStartdatum ([637371d](https://github.com/informatievlaanderen/association-registry/commit/637371d1eabcb5e6f0ccb2807d5ebc92377aad82))
* or-1682 modify Startdatum to empty results in empty Startdatum in beheer detail ([1f7ed10](https://github.com/informatievlaanderen/association-registry/commit/1f7ed10accf359f10f94e561dd90ae542fc5dc1e))
* or-1682 replace publishAsync with InvokeAsync to remove versioning issue when applying changes to the search projection ([7f18545](https://github.com/informatievlaanderen/association-registry/commit/7f18545b1c8d293a8f2df1da2a887e3feac57f98))

# [2.0.0](https://github.com/informatievlaanderen/association-registry/compare/v1.146.0...v2.0.0) (2023-04-17)


### Bug Fixes

* **beheer, publiek:** or-1685 rename omschrijving to beschrijving ([6276b5c](https://github.com/informatievlaanderen/association-registry/commit/6276b5ceb05d3c83e3fa8a17899c4cfaeb6bed27))


### BREAKING CHANGES

* **beheer, publiek:** Contactgegevens - Omschrijving was renamed to Beschrijving.

# [1.146.0](https://github.com/informatievlaanderen/association-registry/compare/v1.145.5...v1.146.0) (2023-04-14)


### Bug Fixes

* or-1682 revert solutionInfo.cs ([8581c8b](https://github.com/informatievlaanderen/association-registry/commit/8581c8b5f0cc048f58106280b6cbd36f1beebbdf))


### Features

* or-1682 startdatum to be internaly nullable ([35215a5](https://github.com/informatievlaanderen/association-registry/commit/35215a5818663a8ce9fc38f46c36352144d27c58))

## [1.145.5](https://github.com/informatievlaanderen/association-registry/compare/v1.145.4...v1.145.5) (2023-04-11)


### Bug Fixes

* or-1648 add request validation on contactgegeven for Waarde which must not be empty ([c560564](https://github.com/informatievlaanderen/association-registry/commit/c560564f254f0b41ef07f6e1001482c73e2e1523))

## [1.145.4](https://github.com/informatievlaanderen/association-registry/compare/v1.145.3...v1.145.4) (2023-04-11)


### Bug Fixes

* or-1647 onbekendContactgegeven to domainException ([0c6ef27](https://github.com/informatievlaanderen/association-registry/commit/0c6ef2740919518d99a3d93424827b58054fb3a7))

## [1.145.3](https://github.com/informatievlaanderen/association-registry/compare/v1.145.2...v1.145.3) (2023-04-07)


### Bug Fixes

* or-1648 correctly validate patch request ([7bf2339](https://github.com/informatievlaanderen/association-registry/commit/7bf23392316c9e5f17859126d816eb95c60f065b))

## [1.145.2](https://github.com/informatievlaanderen/association-registry/compare/v1.145.1...v1.145.2) (2023-04-06)


### Bug Fixes

* or-1648 404 if entity not found ([ce84b1c](https://github.com/informatievlaanderen/association-registry/commit/ce84b1c0965187958295a339ceb8fdd384ad7326))

## [1.145.1](https://github.com/informatievlaanderen/association-registry/compare/v1.145.0...v1.145.1) (2023-04-06)


### Bug Fixes

* or-1648 process feedback ([52a1979](https://github.com/informatievlaanderen/association-registry/commit/52a1979ccdc8ff1f05a3a74684716c655ad97489))

# [1.145.0](https://github.com/informatievlaanderen/association-registry/compare/v1.144.0...v1.145.0) (2023-04-05)


### Bug Fixes

* add validation to contacgegevens ([fca7ff4](https://github.com/informatievlaanderen/association-registry/commit/fca7ff4cc715cafc52cefc59d416a1b25c219302))
* or-1646 fix some issues with headers ([6654d24](https://github.com/informatievlaanderen/association-registry/commit/6654d24a30e771ca1d1fcdc447e1c4fad6a2da23))
* or-1653 fix unit tests (does not work) ([d77e315](https://github.com/informatievlaanderen/association-registry/commit/d77e315e2fed37150b7f524f7ca7e5d80f2a9100))
* or-1653 prevent duplicates in contactgegevens ([8d3fa3d](https://github.com/informatievlaanderen/association-registry/commit/8d3fa3d57840c32d219f1680984da759a4319c18))
* or-1653 rebase fixes ([58655af](https://github.com/informatievlaanderen/association-registry/commit/58655aff8d19cc99d6cf282456f24bf1d3154042))


### Features

* or-1653 group contactgegevens controllers ([9249952](https://github.com/informatievlaanderen/association-registry/commit/9249952d9e134239fcc313dd478b42ebe16db424))
* or-1653 group contactgegevens operations in openapi ([e58e986](https://github.com/informatievlaanderen/association-registry/commit/e58e986e19d494a4b51cffd690c339824bfbb2d1))
* or-1653 wip convert enum to class ([ec18161](https://github.com/informatievlaanderen/association-registry/commit/ec1816148a70cd5a7aea970a0a3f93e3a6aebbbc))

# [1.144.0](https://github.com/informatievlaanderen/association-registry/compare/v1.143.0...v1.144.0) (2023-04-04)


### Features

* or-1647 verwijder contactgegeven ([f988e3f](https://github.com/informatievlaanderen/association-registry/commit/f988e3f62d90e7ad7cf21d992c1055992f768e43))

# [1.143.0](https://github.com/informatievlaanderen/association-registry/compare/v1.142.0...v1.143.0) (2023-04-03)


### Bug Fixes

* or-1653 revert more tests ([4df0ff4](https://github.com/informatievlaanderen/association-registry/commit/4df0ff4dcd6f8ddfd66fbd149581659b53b749d4))
* or-1653 revert tests for voegContactgegevenToe ([5bcd93f](https://github.com/informatievlaanderen/association-registry/commit/5bcd93f3d142336de8cdaa390f7e0407a45e96cc))


### Features

* or-1653 use contactgegevens in registreer and remove contactInfo ([6f0d852](https://github.com/informatievlaanderen/association-registry/commit/6f0d8522269307a1dc1547f912f87ec1128fa2a0))

# [1.142.0](https://github.com/informatievlaanderen/association-registry/compare/v1.141.4...v1.142.0) (2023-03-30)


### Features

* or-1646 add publiek detail contactgegeven ([dfe8b1b](https://github.com/informatievlaanderen/association-registry/commit/dfe8b1b0e06f6688dd37c8e91d7faa29f97f7b96))
* or-1646 fix docs and use enum in request ([7b89c37](https://github.com/informatievlaanderen/association-registry/commit/7b89c3754ba016cad27af91c26e8696be9c3e4df))
* or-1646 put event in historiek data ([6b918ea](https://github.com/informatievlaanderen/association-registry/commit/6b918ea96fa57e3a8e2278ea0d291fe884600e18))
* or-1646 rebase ([c482e2c](https://github.com/informatievlaanderen/association-registry/commit/c482e2c1acf2c9af775478b487c2db10106b9550))
* or-1646 voeg contactgegeven toe ([5712f8f](https://github.com/informatievlaanderen/association-registry/commit/5712f8fb683df21cf932e3cb8d229268be1e3e3c))

## [1.141.4](https://github.com/informatievlaanderen/association-registry/compare/v1.141.3...v1.141.4) (2023-03-29)


### Bug Fixes

* remove pack from build.fsx ([f14af1e](https://github.com/informatievlaanderen/association-registry/commit/f14af1e034c1901c0583472f22f17a42ff0ccdd5))
* remove pack_solution dep ([b1bd237](https://github.com/informatievlaanderen/association-registry/commit/b1bd2373d9a3bb80a67e178accaec94769adbc95))

## [1.141.3](https://github.com/informatievlaanderen/association-registry/compare/v1.141.2...v1.141.3) (2023-03-29)


### Bug Fixes

* force build and deploy ([e04e3cb](https://github.com/informatievlaanderen/association-registry/commit/e04e3cbb00df9799b4a42ec5ba44cb1dded5d644))

## [1.141.2](https://github.com/informatievlaanderen/association-registry/compare/v1.141.1...v1.141.2) (2023-03-28)


### Bug Fixes

* or-1645 startDatum verwijderen results in null data in historiek ([ea2a124](https://github.com/informatievlaanderen/association-registry/commit/ea2a12438bb8c81f5316aa75cf7527ad0a03eae4))

## [1.141.1](https://github.com/informatievlaanderen/association-registry/compare/v1.141.0...v1.141.1) (2023-03-28)


### Bug Fixes

* remove comma ([5ce0e11](https://github.com/informatievlaanderen/association-registry/commit/5ce0e11b1b18f4b8ab3f250e3771f7da580fa536))

# [1.141.0](https://github.com/informatievlaanderen/association-registry/compare/v1.140.1...v1.141.0) (2023-03-23)


### Features

* or-1645 add data to historiek ([ccde734](https://github.com/informatievlaanderen/association-registry/commit/ccde734756aa0236346f76a2825802afb5814bab))
* or-1645 add eventnaam to historiek ([532c5a7](https://github.com/informatievlaanderen/association-registry/commit/532c5a7e2d0bc72e7d120c8cae011e860a1e0fd6))
* or-1645 make korte beschrijving historiek uniform with other events ([ce36524](https://github.com/informatievlaanderen/association-registry/commit/ce36524c1158e40274296469c1485cf1941ce1d6))
* or-1645 update historiek messages ([d02948b](https://github.com/informatievlaanderen/association-registry/commit/d02948b7caedc8def8644733b1c201f6a27091a4))

## [1.140.1](https://github.com/informatievlaanderen/association-registry/compare/v1.140.0...v1.140.1) (2023-03-21)


### Bug Fixes

* or-1617 use geregistreerd io aangemaakt ([32b5b00](https://github.com/informatievlaanderen/association-registry/commit/32b5b00ddca9049b782870a4e90b5e806138d66c))

# [1.140.0](https://github.com/informatievlaanderen/association-registry/compare/v1.139.0...v1.140.0) (2023-03-21)


### Features

* or-1617 rework some test ([50179d0](https://github.com/informatievlaanderen/association-registry/commit/50179d0ce4908a0720443b3a4a5a1ac23095fb0a))

# [1.139.0](https://github.com/informatievlaanderen/association-registry/compare/v1.138.1...v1.139.0) (2023-03-20)


### Features

* or-1617 add history mesage for primairContactInfo ([99a3e49](https://github.com/informatievlaanderen/association-registry/commit/99a3e49b4dd67d56ac4b5186c0f0e4384de94080))
* or-1617 change historiek message for registreer and wijzigNaam ([e81cec1](https://github.com/informatievlaanderen/association-registry/commit/e81cec110a046771bb2c3147b73ac8455be3a820))
* or-1617 change historiek message for wijzig korte naam ([f339c7b](https://github.com/informatievlaanderen/association-registry/commit/f339c7b7a89c8437a51a0420128f65d67f284c13))
* or-1617 change history message for all events ([80aa186](https://github.com/informatievlaanderen/association-registry/commit/80aa186d935d5dbd01bb9fd4ef7e0ef819940050))
* or-1617 change history message for contactInfo ([42cc8c4](https://github.com/informatievlaanderen/association-registry/commit/42cc8c4342a0169eb0ab97a997d25c9ad3cc9bea))
* or-1617 cleanup ([dd11aea](https://github.com/informatievlaanderen/association-registry/commit/dd11aea172830a8609d89905e36386a71c602e73))
* or-1617 fix integration test ([5b356d5](https://github.com/informatievlaanderen/association-registry/commit/5b356d56c3b0e83dc6de968df7f5897db227506c))
* or-1617 update test format ([2f640c1](https://github.com/informatievlaanderen/association-registry/commit/2f640c1f079ab5c11163e5cea26db5d8cd458b0f))

## [1.138.1](https://github.com/informatievlaanderen/association-registry/compare/v1.138.0...v1.138.1) (2023-03-17)


### Bug Fixes

* or-1638 make sure null is saved when passing empty startdatum ([4941254](https://github.com/informatievlaanderen/association-registry/commit/4941254973a98cf5d012c8601d7b3c1e5a23bd42))

# [1.138.0](https://github.com/informatievlaanderen/association-registry/compare/v1.137.2...v1.138.0) (2023-03-17)


### Bug Fixes

* or-1636 fix typo ([6cd8472](https://github.com/informatievlaanderen/association-registry/commit/6cd84720047b4eb61f37cf1f6eb1c59a32e8707b))
* or-1636 fix typos ([d30b347](https://github.com/informatievlaanderen/association-registry/commit/d30b3476816455f35faab6b6b3aecb25e2ac89fa))
* OR-1639 fix typos ([260765e](https://github.com/informatievlaanderen/association-registry/commit/260765e1830e25d3a37819a97ceb1b24fb483163))


### Features

* or-1641 cleanup tests ([ce9f305](https://github.com/informatievlaanderen/association-registry/commit/ce9f305a4163046031f623aad70f743b7d0701a2))

## [1.137.2](https://github.com/informatievlaanderen/association-registry/compare/v1.137.1...v1.137.2) (2023-03-16)


### Bug Fixes

* or-1632 don't return event if no changes happen ([b2fffcd](https://github.com/informatievlaanderen/association-registry/commit/b2fffcda5cab917083b56cdb6a91b55359547c7b))

## [1.137.1](https://github.com/informatievlaanderen/association-registry/compare/v1.137.0...v1.137.1) (2023-03-16)


### Bug Fixes

* remove unnecessary ctor ([3543b4c](https://github.com/informatievlaanderen/association-registry/commit/3543b4caa5b20f2ba0119688f75b10192d53b307))

# [1.137.0](https://github.com/informatievlaanderen/association-registry/compare/v1.136.0...v1.137.0) (2023-03-16)


### Features

* or-1404 trigger again ([058292d](https://github.com/informatievlaanderen/association-registry/commit/058292d2d6faa314ef200b58ddfd87f79935f7fb))

# [1.136.0](https://github.com/informatievlaanderen/association-registry/compare/v1.135.0...v1.136.0) (2023-03-16)


### Features

* or-1404 allow modifications to contact info ([bc21d78](https://github.com/informatievlaanderen/association-registry/commit/bc21d789f9b9d1865f6e042ffb9bc849a54bf711))

# [1.135.0](https://github.com/informatievlaanderen/association-registry/compare/v1.134.0...v1.135.0) (2023-03-14)


### Features

* or-1632 cleanup ([ef315ca](https://github.com/informatievlaanderen/association-registry/commit/ef315cab5831974f5627bec2af4e6a8ae953ac30))
* or-1632 update sequence and version on contactinfoLijstGewijzigd ([beb5552](https://github.com/informatievlaanderen/association-registry/commit/beb5552402f3c3db13457d8852aaaee949134596))

# [1.134.0](https://github.com/informatievlaanderen/association-registry/compare/v1.133.0...v1.134.0) (2023-03-14)


### Features

* or-1632 allow removal of contactInfo ([9e2f69b](https://github.com/informatievlaanderen/association-registry/commit/9e2f69b7c076298052720cc9b7c1b4d6f11da84d))

# [1.133.0](https://github.com/informatievlaanderen/association-registry/compare/v1.132.0...v1.133.0) (2023-03-14)


### Features

* or-1633 add validation for contactInfoLijst ([a4d08be](https://github.com/informatievlaanderen/association-registry/commit/a4d08beb186ad036f224fea9710dae0195438c1c))

# [1.132.0](https://github.com/informatievlaanderen/association-registry/compare/v1.131.2...v1.132.0) (2023-03-14)


### Features

* or-1633 add contact info to request examples ([9844a74](https://github.com/informatievlaanderen/association-registry/commit/9844a74c207020f0eafb0ca996877987e3b4baad))
* or-1633 allow addition of contact info ([0ce9082](https://github.com/informatievlaanderen/association-registry/commit/0ce90822e0cd525fd4d4174ad29237a31c804ebb))

## [1.131.2](https://github.com/informatievlaanderen/association-registry/compare/v1.131.1...v1.131.2) (2023-03-13)


### Bug Fixes

* or-1635 add domain validation description to startdatum for wijzig basisgegevens ([d885bea](https://github.com/informatievlaanderen/association-registry/commit/d885beae256f370eea02d20de9ab873205350dfa))

## [1.131.1](https://github.com/informatievlaanderen/association-registry/compare/v1.131.0...v1.131.1) (2023-03-09)


### Bug Fixes

* or-1424 fix flaky test ([db1ca2f](https://github.com/informatievlaanderen/association-registry/commit/db1ca2fdc14cef6eddea5639f6c9ec009137ed51))
* or-1424 fix validation + complete documentation ([da0848b](https://github.com/informatievlaanderen/association-registry/commit/da0848b6b6aa41679ef3b68d52c3c456bad0d563))

# [1.131.0](https://github.com/informatievlaanderen/association-registry/compare/v1.130.0...v1.131.0) (2023-03-07)


### Bug Fixes

* or-1424 rm duplicate file ([602de44](https://github.com/informatievlaanderen/association-registry/commit/602de446db35528b7945786578b6cb5e3496b4ab))
* or-1424 roll back .net version ([e98a82b](https://github.com/informatievlaanderen/association-registry/commit/e98a82b977bb909f39654edfa4733545b787de0b))


### Features

* or-1424 add startdatum to patch call ([c944f35](https://github.com/informatievlaanderen/association-registry/commit/c944f35f8c7f614e82f1bfd9b5d9c13a20435668))
* or-1424 introduce NullOrEmpty ([0c8d82f](https://github.com/informatievlaanderen/association-registry/commit/0c8d82f4479744fb95a2214a28a5904e37c1ece3))
* or-1424 rename Startdatum ([f0a93ea](https://github.com/informatievlaanderen/association-registry/commit/f0a93eaff8b7dcecd018fb52d537a0433428a4a7))
* or-1424 update projecties ([94acfcc](https://github.com/informatievlaanderen/association-registry/commit/94acfcc059b9d9345c8c9e2ae5b1580a9fc45fd0))
* OR-1556 add docs for 409 header (and a few improvements) ([3a8c86b](https://github.com/informatievlaanderen/association-registry/commit/3a8c86bf67683ea896d5e2a3cfb191f0dc3ee8d6))

# [1.130.0](https://github.com/informatievlaanderen/association-registry/compare/v1.129.0...v1.130.0) (2023-03-02)


### Features

* or-1614 correction public documantation href ([1f1bdcc](https://github.com/informatievlaanderen/association-registry/commit/1f1bdcc7a04d3c803884cbd3c89a4d696ebad410))

# [1.129.0](https://github.com/informatievlaanderen/association-registry/compare/v1.128.0...v1.129.0) (2023-03-02)


### Features

* or-1614 add header for bevestigingsToken ([5d0b3d1](https://github.com/informatievlaanderen/association-registry/commit/5d0b3d18dcac60067079fc3749f312160b213c4c))
* or-1614 bad request if hash if faulty ([5fac380](https://github.com/informatievlaanderen/association-registry/commit/5fac38059d8e8f67bb96397105513d06100adb3b))
* or-1614 fix for tests ([691fd71](https://github.com/informatievlaanderen/association-registry/commit/691fd7162c59951bb6227bf702b298ad3f8ecd7e))

# [1.128.0](https://github.com/informatievlaanderen/association-registry/compare/v1.127.0...v1.128.0) (2023-03-01)


### Features

* OR-1556 detect potential duplicates ([4ddd048](https://github.com/informatievlaanderen/association-registry/commit/4ddd0486d0800fc10e5b567adb814273cf0be2a8))

# [1.127.0](https://github.com/informatievlaanderen/association-registry/compare/v1.126.0...v1.127.0) (2023-02-23)


### Features

* or-1597 add headers to swaggerdocs ([86274a8](https://github.com/informatievlaanderen/association-registry/commit/86274a801941ac26347cd3d1fc793ae69275a218))
* or-1597 pull swagger in extention methods ([41ace43](https://github.com/informatievlaanderen/association-registry/commit/41ace43ec1f908ba6930824ef3e729979095bdaf))
* or-1597 remove comments ([d17c0fb](https://github.com/informatievlaanderen/association-registry/commit/d17c0fbc07344c9fae33a9b3a95e76f6f33349f8))

# [1.126.0](https://github.com/informatievlaanderen/association-registry/compare/v1.125.0...v1.126.0) (2023-02-22)


### Features

* or-1412 add JsonReaderExceptionHandler ([60f18e9](https://github.com/informatievlaanderen/association-registry/commit/60f18e9404aa40cf57fd676044fcc84207b817fe))

# [1.125.0](https://github.com/informatievlaanderen/association-registry/compare/v1.124.1...v1.125.0) (2023-02-22)


### Bug Fixes

* or-1413 fix broken tests ([f059868](https://github.com/informatievlaanderen/association-registry/commit/f0598687da7ebad94e7951026d8ffb3dbb0c3c0d))


### Features

* or-1413 throw exception if duplicate properties in json request filter ([937ea6c](https://github.com/informatievlaanderen/association-registry/commit/937ea6c16183fba56aff92235b1fddee16944e12))

## [1.124.1](https://github.com/informatievlaanderen/association-registry/compare/v1.124.0...v1.124.1) (2023-02-21)


### Bug Fixes

* or-1567 remove bus from adres when no busnummer is present ([760475f](https://github.com/informatievlaanderen/association-registry/commit/760475f7481a0867ba0c294a67182fb119b00085))

# [1.124.0](https://github.com/informatievlaanderen/association-registry/compare/v1.123.0...v1.124.0) (2023-02-21)


### Features

* or-1412 trigger deploy ([23f4580](https://github.com/informatievlaanderen/association-registry/commit/23f4580c21f3e779d7e30d6c91c3fddb477427db))

# [1.123.0](https://github.com/informatievlaanderen/association-registry/compare/v1.122.0...v1.123.0) (2023-02-21)


### Bug Fixes

* or-1412 make email case insensitive ([1985366](https://github.com/informatievlaanderen/association-registry/commit/1985366b6c6e140877f7d04c4ffa77c04e490ec9))


### Features

* or-1400 make VCode in public detail case insentitive ([99ec77d](https://github.com/informatievlaanderen/association-registry/commit/99ec77d77b6bf450ec86e2c6cbe515022d6ab705))

# [1.122.0](https://github.com/informatievlaanderen/association-registry/compare/v1.121.2...v1.122.0) (2023-02-21)


### Bug Fixes

* or-1412 modify email validation to be less strict ([490c195](https://github.com/informatievlaanderen/association-registry/commit/490c1950ffbcb9f2b6f12b6dc52d1308798c6e8f))
* or-1412 update documentation of registreer vereniging ([6aa7460](https://github.com/informatievlaanderen/association-registry/commit/6aa746083852e257b182ffe2010e40db32d30632))
* or-1412 update documentation of registreer vereniging ([cd0ee9b](https://github.com/informatievlaanderen/association-registry/commit/cd0ee9b2c8bc651bf8098832cb37559e624a21c5))


### Features

* or-1412 add validation for contactinfo parts ([131090a](https://github.com/informatievlaanderen/association-registry/commit/131090a7eba89634a3317f806710d174c6f58a1d))
* or-1412 simplify email regex and use Url for website and social media ([72b8e43](https://github.com/informatievlaanderen/association-registry/commit/72b8e43d3d4f2222646c877b8b0ffc3e80d3c522))

## [1.121.2](https://github.com/informatievlaanderen/association-registry/compare/v1.121.1...v1.121.2) (2023-02-21)


### Bug Fixes

* or-1579 map PrimairContactInfo in beheer detail ([3cf2878](https://github.com/informatievlaanderen/association-registry/commit/3cf2878ca425599380dee4e54cb6dd4f131938ad))

## [1.121.1](https://github.com/informatievlaanderen/association-registry/compare/v1.121.0...v1.121.1) (2023-02-21)


### Bug Fixes

* or-1579 add primaryContactInfo to DTO object ([78f370a](https://github.com/informatievlaanderen/association-registry/commit/78f370a5477e711018e65a53e4de196cf6dc054d))

# [1.121.0](https://github.com/informatievlaanderen/association-registry/compare/v1.120.0...v1.121.0) (2023-02-20)


### Features

* or-1414 make Contactnaam unique ([bf538e7](https://github.com/informatievlaanderen/association-registry/commit/bf538e7941e7b9fd12b0944c75172f4577a25e15))

# [1.120.0](https://github.com/informatievlaanderen/association-registry/compare/v1.119.0...v1.120.0) (2023-02-20)


### Features

* or-1414 make Contactnaam required ([134b702](https://github.com/informatievlaanderen/association-registry/commit/134b702af71a1e933feac450ff2f9cae98db7059))

# [1.119.0](https://github.com/informatievlaanderen/association-registry/compare/v1.118.5...v1.119.0) (2023-02-20)


### Features

* or-1579 add primairContactInfo ([f5acad0](https://github.com/informatievlaanderen/association-registry/commit/f5acad076763eb2cdd60092962451d5960d3be0b))

## [1.118.5](https://github.com/informatievlaanderen/association-registry/compare/v1.118.4...v1.118.5) (2023-02-16)


### Bug Fixes

* or-1319 missed rename ([45b7f9f](https://github.com/informatievlaanderen/association-registry/commit/45b7f9f012364f0a4897a827f210e20eb6cd5b92))

## [1.118.4](https://github.com/informatievlaanderen/association-registry/compare/v1.118.3...v1.118.4) (2023-02-16)


### Bug Fixes

* or-1319 missed rename ([e903806](https://github.com/informatievlaanderen/association-registry/commit/e903806824abe5bbf2469b8aa2a0dfc96acbd97b))

## [1.118.3](https://github.com/informatievlaanderen/association-registry/compare/v1.118.2...v1.118.3) (2023-02-16)


### Bug Fixes

* or-1319 more renames ([b58ce48](https://github.com/informatievlaanderen/association-registry/commit/b58ce485dcfc48fafe597fe31e9c92307dd71b98))

## [1.118.2](https://github.com/informatievlaanderen/association-registry/compare/v1.118.1...v1.118.2) (2023-02-16)


### Bug Fixes

* or-1319 fix typo ([7fa60ea](https://github.com/informatievlaanderen/association-registry/commit/7fa60ea8ef41a2e28d831e59d3dd455cb03619b9))

## [1.118.1](https://github.com/informatievlaanderen/association-registry/compare/v1.118.0...v1.118.1) (2023-02-16)


### Bug Fixes

* or-1319 make vereniging serializable ([92db1e6](https://github.com/informatievlaanderen/association-registry/commit/92db1e6331b6a2205cebf759d5bc67565e501b04))

# [1.118.0](https://github.com/informatievlaanderen/association-registry/compare/v1.117.0...v1.118.0) (2023-02-16)


### Features

* or-1570 rename to hoofdactiviteitenVerenigingsloket ([39bfbec](https://github.com/informatievlaanderen/association-registry/commit/39bfbec127864a12d1f34f81657130a104359a3d))
* or-1592 add hoofdactiviteiten endpoint ([ac42e3a](https://github.com/informatievlaanderen/association-registry/commit/ac42e3a021deaee5ef7e272d65d07e45a510c1d9))

# [1.117.0](https://github.com/informatievlaanderen/association-registry/compare/v1.116.2...v1.117.0) (2023-02-15)


### Features

* or-1319 add acm projection - verenigingPerVergtegenwoordiger ([d1fb7fd](https://github.com/informatievlaanderen/association-registry/commit/d1fb7fdc8dc6ce3e5ed73c8d14e64a4e8fde1ca1))
* or-1319 clarify and clean up ([13537b3](https://github.com/informatievlaanderen/association-registry/commit/13537b30e078f86b184d11c64882710f277fd1ac))
* or-1570 add hoofdactiviteiten to registreer ([ae3c17f](https://github.com/informatievlaanderen/association-registry/commit/ae3c17f6dc4838cce0d581ce2a43a66a10edb10b))

## [1.116.2](https://github.com/informatievlaanderen/association-registry/compare/v1.116.1...v1.116.2) (2023-02-07)


### Bug Fixes

* or-1591 expect version to be current + nr of events ([26ee4be](https://github.com/informatievlaanderen/association-registry/commit/26ee4be44534693ca368592b45c71ebb68e789a6))

## [1.116.1](https://github.com/informatievlaanderen/association-registry/compare/v1.116.0...v1.116.1) (2023-02-03)


### Bug Fixes

* or-1580 allow anonymous on root path ([3451d64](https://github.com/informatievlaanderen/association-registry/commit/3451d648bae3619e0419a6a6de32b62a9a492c7b))

# [1.116.0](https://github.com/informatievlaanderen/association-registry/compare/v1.115.0...v1.116.0) (2023-02-02)


### Features

* or-1364 reabse to main ([8f619bf](https://github.com/informatievlaanderen/association-registry/commit/8f619bf2b95dc013401cafe4192220bf29a395de))
* or-1364 require authorization for admin api ([fa99fdc](https://github.com/informatievlaanderen/association-registry/commit/fa99fdca0a6d75e6c616825faab9ece146ca1495))

# [1.115.0](https://github.com/informatievlaanderen/association-registry/compare/v1.114.1...v1.115.0) (2023-02-02)


### Features

* or-1576 add vertegenwoordiger domain service ([c7977e5](https://github.com/informatievlaanderen/association-registry/commit/c7977e50eff5c3cf32a4c05aee93f6c5acd445e1))

## [1.114.1](https://github.com/informatievlaanderen/association-registry/compare/v1.114.0...v1.114.1) (2023-02-01)


### Bug Fixes

* or-1575 add validation rule for invalid characters in Insz ([d0bb9d4](https://github.com/informatievlaanderen/association-registry/commit/d0bb9d43557197fb3c6424b9f0e56d4c7ad26afa))

# [1.114.0](https://github.com/informatievlaanderen/association-registry/compare/v1.113.0...v1.114.0) (2023-02-01)


### Features

* or-1397 add contactgegevens to vertegenwoordiger ([6b258e7](https://github.com/informatievlaanderen/association-registry/commit/6b258e7077d883fe163de09c8f3ced3db90f09c9))

# [1.113.0](https://github.com/informatievlaanderen/association-registry/compare/v1.112.0...v1.113.0) (2023-02-01)


### Bug Fixes

* or-1575 add datamember to vertegenwoordiger properties ([11aba1d](https://github.com/informatievlaanderen/association-registry/commit/11aba1d93cba31de88289b80103df402d1dc7986))
* or-1575 make domain exceptions inherit from DomainException ([90472da](https://github.com/informatievlaanderen/association-registry/commit/90472dacf2bc86fc4c2b9d8e03cd4b1e2533ce7c))


### Features

* or-1575 ensure unique insz within vereniging ([ef5a46d](https://github.com/informatievlaanderen/association-registry/commit/ef5a46d92f565259701742af35f235205c8f6692))
* or-1575 validate insz ([7077aa1](https://github.com/informatievlaanderen/association-registry/commit/7077aa1257e0dd2a12155e1b631c0cecfa832112))
* or-1575 validate vertegenwoordigers for single primaryContact ([e78a5ae](https://github.com/informatievlaanderen/association-registry/commit/e78a5aed8fc8ce11b24f3dfb831847f12b8f6070))

# [1.112.0](https://github.com/informatievlaanderen/association-registry/compare/v1.111.1...v1.112.0) (2023-01-31)


### Bug Fixes

* or-1369 add handling for unparsable request when registreer ([f332cdb](https://github.com/informatievlaanderen/association-registry/commit/f332cdb18ff63eae6e27be6dccddd44b26b29bb4))


### Features

* or-1369 return badrequest when unparsable request ([2c47ee4](https://github.com/informatievlaanderen/association-registry/commit/2c47ee492ddbe881cd266149e1e87b97446a8ab8))

## [1.111.1](https://github.com/informatievlaanderen/association-registry/compare/v1.111.0...v1.111.1) (2023-01-31)


### Bug Fixes

* or-1369 add datamember to Vertegenwoordigers in request ([a25567f](https://github.com/informatievlaanderen/association-registry/commit/a25567f94b017d40218d34873c71716c0a2ab847))

# [1.111.0](https://github.com/informatievlaanderen/association-registry/compare/v1.110.0...v1.111.0) (2023-01-31)


### Features

* or-1369 add vertegenwoordiger with static naam ([c55b643](https://github.com/informatievlaanderen/association-registry/commit/c55b6430f611dd2eebe41be8b4861cff827d60a7))
* or-1369 rename rijksregisternummer to insz ([fefcdef](https://github.com/informatievlaanderen/association-registry/commit/fefcdefac56f7b7156705d1f75ca579e53faaea0))

# [1.110.0](https://github.com/informatievlaanderen/association-registry/compare/v1.109.2...v1.110.0) (2023-01-30)


### Bug Fixes

* or-1569 solve rebase issues ([ed66362](https://github.com/informatievlaanderen/association-registry/commit/ed6636220df4b6e6ccd81fce355fa2016e1244b5))


### Features

* or-1569 add controller unit test ([6c37c53](https://github.com/informatievlaanderen/association-registry/commit/6c37c537aa9d46c6b53368cc69bd56127516a7f2))
* or-1569 migrate command handler tests to new format ([115a1f5](https://github.com/informatievlaanderen/association-registry/commit/115a1f50844223a7876ef8ca2497b2794c740907))
* or-1569 migrate command handler tests to new format ([3a40d80](https://github.com/informatievlaanderen/association-registry/commit/3a40d806310f34ca867940e0720b147ca04f23cc))
* or-1569 use moq for repository ([2ade98d](https://github.com/informatievlaanderen/association-registry/commit/2ade98d792ec22b0fa6ba67942cff74ae738ae40))

## [1.109.2](https://github.com/informatievlaanderen/association-registry/compare/v1.109.1...v1.109.2) (2023-01-26)


### Bug Fixes

* or-1565: handle badhttprequestexception as bad request ([10f12a2](https://github.com/informatievlaanderen/association-registry/commit/10f12a255ca6b2c90df71c733c7bf9627cc938de))

## [1.109.1](https://github.com/informatievlaanderen/association-registry/compare/v1.109.0...v1.109.1) (2023-01-26)


### Bug Fixes

* or-1564 add namespace references ([b2c769b](https://github.com/informatievlaanderen/association-registry/commit/b2c769b22fc8fb1a56f35fa3991ddb9d87d70d36))
* or-1564 write tijdstip in correct format ([0b6010d](https://github.com/informatievlaanderen/association-registry/commit/0b6010d33b27b1d99e6fdd41d2c72ec57bbb0723))

# [1.109.0](https://github.com/informatievlaanderen/association-registry/compare/v1.108.0...v1.109.0) (2023-01-25)


### Bug Fixes

* or-1564 remove datumLaatsteAanpassing from VerenigingWerdGeregistreerd event and replace in projections by metadata.Tijdstip ([4a5b874](https://github.com/informatievlaanderen/association-registry/commit/4a5b874b482eb65d8a4008aa255853c950c75bc6))
* or-1565: handle etags more robustly ([f1c8a4e](https://github.com/informatievlaanderen/association-registry/commit/f1c8a4eb1dbb1ecf2e74a912154a5405f6208129))


### Features

* or-1564 fix projections laatste aanpassing ([9838e10](https://github.com/informatievlaanderen/association-registry/commit/9838e108e242e104759b3bc52fbdba6ebfa3b143))

# [1.108.0](https://github.com/informatievlaanderen/association-registry/compare/v1.107.0...v1.108.0) (2023-01-23)


### Features

* or-1558 add 2 new hoofdactiviteiten to the brolfeeder ([1ff6436](https://github.com/informatievlaanderen/association-registry/commit/1ff6436802ca93b17640100721b3ee3ade4cc214))

# [1.107.0](https://github.com/informatievlaanderen/association-registry/compare/v1.106.0...v1.107.0) (2023-01-23)


### Bug Fixes

* or-1564 use using for sessions ([64c9649](https://github.com/informatievlaanderen/association-registry/commit/64c9649c245b6d891b3525e58467a3f759405b69))


### Features

* or-1564 add projections ([7ba401e](https://github.com/informatievlaanderen/association-registry/commit/7ba401ecccd0c430f1a3d74a8c4cbed58f3df03c))
* or-1564 implement wijzig korte beschrijving without projections ([ae22394](https://github.com/informatievlaanderen/association-registry/commit/ae22394ccdc3d1c4fa3ba23af41e9da0a9f37729))

# [1.106.0](https://github.com/informatievlaanderen/association-registry/compare/v1.105.0...v1.106.0) (2023-01-23)


### Bug Fixes

* or-1565 dispose document store and sessions ([c6806c7](https://github.com/informatievlaanderen/association-registry/commit/c6806c70cb7ffadadc8c4e5162b18a52c4eaa8ee))
* or-1565 use assembly name without version for opentelemetry ([5dddd13](https://github.com/informatievlaanderen/association-registry/commit/5dddd1324f4a520e1c32efa66ba656974a638163))


### Features

* or-1565 add docs about if-match header ([eec2922](https://github.com/informatievlaanderen/association-registry/commit/eec29224d9e1173dbc93dd1936be53e8bd9bae70))
* or-1565 add tests to verify documentation ([b956254](https://github.com/informatievlaanderen/association-registry/commit/b9562543b0312e53b7cd9f179e733369743783eb))
* or-1565 allow concurrency checks ([022de6f](https://github.com/informatievlaanderen/association-registry/commit/022de6f1fba081378a5dc64d98b0a4c7ebe4aa3f))
* or-1565 move tests to adminApiFixture2 ([e6672c2](https://github.com/informatievlaanderen/association-registry/commit/e6672c2d2d5d70ac6388ad8a1b0364f5c1191a03))

# [1.105.0](https://github.com/informatievlaanderen/association-registry/compare/v1.104.0...v1.105.0) (2023-01-23)


### Bug Fixes

* or-1563 add validation on initiator ([0fff161](https://github.com/informatievlaanderen/association-registry/commit/0fff16149afdb63d5b42f5fc89dfdb83b5b8b859))
* or-1563 check that initiator is required ([4ae0ef0](https://github.com/informatievlaanderen/association-registry/commit/4ae0ef0b66a0cceae3078f748ff2da38756857c0))
* or-1563 don't crash on no events ([7be8906](https://github.com/informatievlaanderen/association-registry/commit/7be8906496b588d9ab3351b8059892d1e1c148fd))


### Features

* or-1363 add KorteNaamWerdGewijzigd including beheer projections ([25cfecc](https://github.com/informatievlaanderen/association-registry/commit/25cfecc61388c2db03c2d9b10d77ffff30720d6c))
* or-1563 add validation for patch kortenaam ([9bc409c](https://github.com/informatievlaanderen/association-registry/commit/9bc409cb94f8e09d30f205d30e395262123151e8))
* or-1563 handle korteNaamWerdGewijzigd in public projections ([41ea6e6](https://github.com/informatievlaanderen/association-registry/commit/41ea6e634198b8217672968ad7923806816abb3f))

# [1.104.0](https://github.com/informatievlaanderen/association-registry/compare/v1.103.0...v1.104.0) (2023-01-19)


### Bug Fixes

* or-1365 add Id property mapping to VCode in elastic search ([2b49929](https://github.com/informatievlaanderen/association-registry/commit/2b49929b1eee1d555cd8997d2c2c6778ceefe08d))


### Features

* or-1365 add wijzignaam to search ([4a8b905](https://github.com/informatievlaanderen/association-registry/commit/4a8b90539c05916390728be5d06a89fce4049549))

# [1.103.0](https://github.com/informatievlaanderen/association-registry/compare/v1.102.0...v1.103.0) (2023-01-18)


### Bug Fixes

* or-1365 fix warnigs ([08eba1f](https://github.com/informatievlaanderen/association-registry/commit/08eba1fa6a44ced065cf25fa3752855d3371feb1))


### Features

* or-1193 expand docs ([0638a84](https://github.com/informatievlaanderen/association-registry/commit/0638a8498275ea6f94bde892edc5f58decdcd28e))
* or-1365 add projections ([4407189](https://github.com/informatievlaanderen/association-registry/commit/44071897bc40a53f32d1fb3130026ea9195a4c22))
* or-1365 add sequence header in response ([08f2832](https://github.com/informatievlaanderen/association-registry/commit/08f2832cccc31b9ef5c112bfc5a36b4ff5417a0a))
* or-1365 allow wijzig naam ([d808564](https://github.com/informatievlaanderen/association-registry/commit/d808564a86e57475d9a10d1166b059a2d0a1cbf3))
* or-1365 ignore null naam ([1c8e524](https://github.com/informatievlaanderen/association-registry/commit/1c8e52462c1130ac909e851230287dc201655fab))
* or-1365 use testcontainer for postgresDb AdminApi ([c9ae1c3](https://github.com/informatievlaanderen/association-registry/commit/c9ae1c3208c5d771d263458f7083849d722a7de4))

# [1.102.0](https://github.com/informatievlaanderen/association-registry/compare/v1.101.5...v1.102.0) (2023-01-12)


### Features

* or-1415 elaborate on vr-sequence ([18021dd](https://github.com/informatievlaanderen/association-registry/commit/18021dd0ee945471757cd7bf1c2611c6dd0fb35e))

## [1.101.5](https://github.com/informatievlaanderen/association-registry/compare/v1.101.4...v1.101.5) (2023-01-12)

## [1.101.4](https://github.com/informatievlaanderen/association-registry/compare/v1.101.3...v1.101.4) (2023-01-12)

## [1.101.3](https://github.com/informatievlaanderen/association-registry/compare/v1.101.2...v1.101.3) (2023-01-12)


### Bug Fixes

* or-1551 rename locatie type to locatietype in search projection ([3e591be](https://github.com/informatievlaanderen/association-registry/commit/3e591be1c5a1cb801f2f67e60d52b6354003c668))

## [1.101.2](https://github.com/informatievlaanderen/association-registry/compare/v1.101.1...v1.101.2) (2023-01-12)


### Bug Fixes

* or-1551 always use locatieType ([2717e73](https://github.com/informatievlaanderen/association-registry/commit/2717e734fbc509b088ba79b0e85f73ed2fbd62c4))
* or-1551 correct hoofdlocatie to always be lower case ([8d564f8](https://github.com/informatievlaanderen/association-registry/commit/8d564f8e50e5994e3050e55c3c8a4dbeb094c11c))
* or-1551 rename locatieType to locatietype ([afd86fb](https://github.com/informatievlaanderen/association-registry/commit/afd86fb89702893e4899d3ab93b0c9a7b413986d))
* or-1551 renamed Id to VCode in ACM.Api ([5270a8b](https://github.com/informatievlaanderen/association-registry/commit/5270a8bd99b69ef4877a66e03bedc94a59a5f0cb))

## [1.101.1](https://github.com/informatievlaanderen/association-registry/compare/v1.101.0...v1.101.1) (2023-01-11)


### Bug Fixes

* fix typos ([917eaff](https://github.com/informatievlaanderen/association-registry/commit/917eaff6b53cb3bea0dbfc062fccaf8ff91f8519))

# [1.101.0](https://github.com/informatievlaanderen/association-registry/compare/v1.100.0...v1.101.0) (2023-01-11)


### Features

* or-1337 translate error messages ([ba3c129](https://github.com/informatievlaanderen/association-registry/commit/ba3c129a3795503c7b4f8ab58d9067809c9d33df))

# [1.100.0](https://github.com/informatievlaanderen/association-registry/compare/v1.99.0...v1.100.0) (2023-01-11)


### Features

* or-1297 use console lifetime ([35cea0d](https://github.com/informatievlaanderen/association-registry/commit/35cea0dadc8cdde6c31d814896526b2378072deb))

# [1.99.0](https://github.com/informatievlaanderen/association-registry/compare/v1.98.0...v1.99.0) (2023-01-11)


### Features

* or-1297 add health check ([c689a44](https://github.com/informatievlaanderen/association-registry/commit/c689a44b478adb2c1b1fd42bee0811db8bce5226))

# [1.98.0](https://github.com/informatievlaanderen/association-registry/compare/v1.97.0...v1.98.0) (2023-01-11)


### Features

* or-1297 add test to verify no exception is thrown when unhandled event is processed ([430c394](https://github.com/informatievlaanderen/association-registry/commit/430c3943a3510ac4f170bd83baa3da5d6d59a22c))

# [1.97.0](https://github.com/informatievlaanderen/association-registry/compare/v1.96.0...v1.97.0) (2023-01-10)


### Features

* or-1415 make expected sequence optional ([45b18f4](https://github.com/informatievlaanderen/association-registry/commit/45b18f426170718692db8ce058af09587a0119e8))

# [1.96.0](https://github.com/informatievlaanderen/association-registry/compare/v1.95.6...v1.96.0) (2023-01-10)


### Features

* or-1297 rename contacten; make sure projections don't interfere with eachother ([548b901](https://github.com/informatievlaanderen/association-registry/commit/548b901ec2bed938d2a7322e35ea8620d64b68c1))

## [1.95.6](https://github.com/informatievlaanderen/association-registry/compare/v1.95.5...v1.95.6) (2023-01-10)


### Bug Fixes

* remove dod.yml, rename main workflow ([782a74e](https://github.com/informatievlaanderen/association-registry/commit/782a74e0a4bdd834473f9958cb5c81cfc0e021a7))

## [1.95.5](https://github.com/informatievlaanderen/association-registry/compare/v1.95.4...v1.95.5) (2023-01-10)


### Bug Fixes

* remove sonar scanner from build, rename build to release ([9e3a56e](https://github.com/informatievlaanderen/association-registry/commit/9e3a56e84f09ad4fd26fc852d8626b2c0ab1bfbe))
* remove sonar scanner from build, rename build to release ([4a9657c](https://github.com/informatievlaanderen/association-registry/commit/4a9657cca17be735c62142448979d7ad912177d5))

## [1.95.4](https://github.com/informatievlaanderen/association-registry/compare/v1.95.3...v1.95.4) (2023-01-10)


### Bug Fixes

* add docker registry ([8ea23cc](https://github.com/informatievlaanderen/association-registry/commit/8ea23ccb9dacd0b0987bb217d19e7afd0d55a0fa))
* build acm api separately ([2602153](https://github.com/informatievlaanderen/association-registry/commit/2602153d5b4c54bc70e4ea2dad78a79b9d5d06e3))
* pass semver, build all via reusable ([6c7d4ae](https://github.com/informatievlaanderen/association-registry/commit/6c7d4ae48a1fefdfd3f0ea3e3bdaad1c8e71e980))
* reusable workflow depends on set-release ([bb73f77](https://github.com/informatievlaanderen/association-registry/commit/bb73f772a74e89c3464e55032a447dbbe06ef6a4))
* run all services ([6569511](https://github.com/informatievlaanderen/association-registry/commit/6569511619ef5312b1b972adc8f2310355f4ab2b))
* try reusable workflow ([4bf4492](https://github.com/informatievlaanderen/association-registry/commit/4bf44920b8b7307ee42700822c14bbdea74593b4))
* typo in artifact name ([b175606](https://github.com/informatievlaanderen/association-registry/commit/b175606f4c9db5021c8c945e7c5b3379f068f05f))
* use build targets ([039d3bd](https://github.com/informatievlaanderen/association-registry/commit/039d3bdbbd7b1ac745c7bd6d5d4d521924591283))
* use correct dist folder ([a2b949a](https://github.com/informatievlaanderen/association-registry/commit/a2b949a0a458c66217466928197599fbbe0e9a33))
* use correct path ([17ff060](https://github.com/informatievlaanderen/association-registry/commit/17ff06057537f4058b5be65211b05543c437aa5e))

## [1.95.3](https://github.com/informatievlaanderen/association-registry/compare/v1.95.2...v1.95.3) (2023-01-09)


### Bug Fixes

* echo next version during analyzeCommits ([65166b8](https://github.com/informatievlaanderen/association-registry/commit/65166b854a1e95ed61609fbdec7a8dbd954ff5a0))
* echo next version during generateNotes ([2dae4b6](https://github.com/informatievlaanderen/association-registry/commit/2dae4b6ed434e3a7f2147c484e39df07064670b2))
* set release version after dry-run ([1e90160](https://github.com/informatievlaanderen/association-registry/commit/1e90160edcadcc5af18757f4187b7b76f92153d2))
* try verify-release ([047e990](https://github.com/informatievlaanderen/association-registry/commit/047e990314a6736178406afc246d703896fb5218))


### Reverts

* Revert "fix: use --prepare" ([c7c744c](https://github.com/informatievlaanderen/association-registry/commit/c7c744c581ab0b897279cdba4b77246d6ba0c974))
* Revert "fix: move prepare steps to publish" ([39b6614](https://github.com/informatievlaanderen/association-registry/commit/39b6614627acd1c7c954a220e05aa2b1cb03fc21))

## [1.95.1](https://github.com/informatievlaanderen/association-registry/compare/v1.95.0...v1.95.1) (2023-01-09)


### Bug Fixes

* or-1297 use real logger ([7406e7a](https://github.com/informatievlaanderen/association-registry/commit/7406e7a37475525dc3ea3d36a758661bf5b1e66d))

# [1.95.0](https://github.com/informatievlaanderen/association-registry/compare/v1.94.0...v1.95.0) (2023-01-09)


### Features

* or-1193 make admin api work without vbr lib ([#141](https://github.com/informatievlaanderen/association-registry/issues/141)) ([6cd15c1](https://github.com/informatievlaanderen/association-registry/commit/6cd15c11e7a2f9b315a1ee25408a4813e36dc6fa))

# [1.94.0](https://github.com/informatievlaanderen/association-registry/compare/v1.93.0...v1.94.0) (2023-01-05)


### Features

* OR-1278 adjust namespaces ([de2af4c](https://github.com/informatievlaanderen/association-registry/commit/de2af4ca65bd96ec6debfabc1e54cc9eae8de518))
* OR-1278 flatten tests ([f3d021c](https://github.com/informatievlaanderen/association-registry/commit/f3d021c308ef792e9ebcf36f13596568ab4d2bff))
* OR-1278 restructure admin api ([569dea1](https://github.com/informatievlaanderen/association-registry/commit/569dea1db0882c31a6ca3209a46a80c94a8c2c93))
* OR-1278 restructure public api ([fe94415](https://github.com/informatievlaanderen/association-registry/commit/fe9441515450244edb9dccaf28d77dac817563cd))
* OR-1297 rename extensions ([b4cb84b](https://github.com/informatievlaanderen/association-registry/commit/b4cb84b04609999943ac813dc8345dd40b3e93d1))
* OR-1297 use wolverine in projections ([efb6b19](https://github.com/informatievlaanderen/association-registry/commit/efb6b1980de7648ab292a4d29cf8c81761924fdd))

# [1.93.0](https://github.com/informatievlaanderen/association-registry/compare/v1.92.0...v1.93.0) (2023-01-03)


### Features

* OR-1297 don't add port to host ([39f9790](https://github.com/informatievlaanderen/association-registry/commit/39f979045f77baf065906bf552df838f21deb813))

# [1.92.0](https://github.com/informatievlaanderen/association-registry/compare/v1.91.0...v1.92.0) (2023-01-03)


### Features

* OR-1297 add wolverine to tracing ([b02f6dc](https://github.com/informatievlaanderen/association-registry/commit/b02f6dcc436e3b1be69978cc6699d8ac325a2249))

# [1.91.0](https://github.com/informatievlaanderen/association-registry/compare/v1.90.0...v1.91.0) (2023-01-03)


### Features

* OR-1297 add rebuild functionality ([f8753db](https://github.com/informatievlaanderen/association-registry/commit/f8753db141717ae38c6df9ba5a29bd7ffa2e31d4))
* OR-1297 add some vbr code (maybe) ([4b6ccaa](https://github.com/informatievlaanderen/association-registry/commit/4b6ccaaee103c845fee13d95eba0cd721c3dbb08))
* OR-1297 move projections into projection host (tests working) ([d59635b](https://github.com/informatievlaanderen/association-registry/commit/d59635b23a27718d9e267dbf621bdb0d50510050))
* OR-1417 separate hoofdactiviteiten facets from querystring and use global filter ([b780d0c](https://github.com/informatievlaanderen/association-registry/commit/b780d0cce8864b215de9b49c4e3e494a8e88c98f))

# [1.90.0](https://github.com/informatievlaanderen/association-registry/compare/v1.89.0...v1.90.0) (2022-12-26)


### Features

* OR-1415 make location header absolute ([9a14e3e](https://github.com/informatievlaanderen/association-registry/commit/9a14e3e184e05b568fdf1fe33c58d894dbf76d92))
* OR-1415 return sequence ([a92f155](https://github.com/informatievlaanderen/association-registry/commit/a92f1551d3f22b51bef80c141b2079d8d92282bc))
* OR-1415 use sequence for Historiek ([e9d119e](https://github.com/informatievlaanderen/association-registry/commit/e9d119e43a74984623540e52ce24aa0f32a43bd5))
* OR-1415 use sequence to check precondition ([8d854a5](https://github.com/informatievlaanderen/association-registry/commit/8d854a55df0d8438bdb96b74964f2dd28200655c))

# [1.89.0](https://github.com/informatievlaanderen/association-registry/compare/v1.88.1...v1.89.0) (2022-12-23)


### Bug Fixes

* or-1330 increase waiting for stale data to 20 seconds ([b3a89cc](https://github.com/informatievlaanderen/association-registry/commit/b3a89cce2c57230ea1b10e540c8d2acbebce4a01))


### Features

* or-1330 add detail vereniging to beheer api ([a402678](https://github.com/informatievlaanderen/association-registry/commit/a402678717b4f111b0e22b8da4b30276d8b1fef6))

## [1.88.1](https://github.com/informatievlaanderen/association-registry/compare/v1.88.0...v1.88.1) (2022-12-23)


### Bug Fixes

* or-1406 fix double code for hoofdactiviteiten ([b65d76f](https://github.com/informatievlaanderen/association-registry/commit/b65d76fcd70b6e0b55cad3eaf883d456057da34b))

# [1.88.0](https://github.com/informatievlaanderen/association-registry/compare/v1.87.0...v1.88.0) (2022-12-22)


### Bug Fixes

* or-1278 match duplicate locations ([be75558](https://github.com/informatievlaanderen/association-registry/commit/be755586ccad25e83e25778f679a109adeb6cfb5))


### Features

* or-1276 add postcode and gemeente to search projection location ([3fe381d](https://github.com/informatievlaanderen/association-registry/commit/3fe381d39fc6a79c4487e615d4b395e9bede5908))

# [1.87.0](https://github.com/informatievlaanderen/association-registry/compare/v1.86.0...v1.87.0) (2022-12-22)


### Features

* or-1278 rename detail response to ContactInfoLijst ([60250cb](https://github.com/informatievlaanderen/association-registry/commit/60250cbc69db8c0c27de2194684eb44bbd8a3fbe))
* or-1406 replace hoofdactiviteiten list in brolfeeder ([f334bf6](https://github.com/informatievlaanderen/association-registry/commit/f334bf6405e7f9256afcca37c5adbbc0add62362))

# [1.86.0](https://github.com/informatievlaanderen/association-registry/compare/v1.85.0...v1.86.0) (2022-12-22)


### Features

* OR-1278 make new fields nullable ([c83ca3b](https://github.com/informatievlaanderen/association-registry/commit/c83ca3b2bb352438a043400de8ec62bcbecfb005))

# [1.85.0](https://github.com/informatievlaanderen/association-registry/compare/v1.84.0...v1.85.0) (2022-12-22)


### Features

* OR-1278 rename to ContactInfoLijst ([ce6c016](https://github.com/informatievlaanderen/association-registry/commit/ce6c016ed2dda1bbe38a0dc3ab3406cf07ea03ba))

# [1.84.0](https://github.com/informatievlaanderen/association-registry/compare/v1.83.0...v1.84.0) (2022-12-22)


### Features

* or-1276 add locatie to initial create ([95a7e0d](https://github.com/informatievlaanderen/association-registry/commit/95a7e0d4c53af5e60b3541b907f233aed8cb8df7))
* or-1276 remove WithActualData postfix ([555169f](https://github.com/informatievlaanderen/association-registry/commit/555169f8b9693c36ee4b49d58e024ecbaf3bf8e4))
* or-1276 rename mapping methods ([5e90302](https://github.com/informatievlaanderen/association-registry/commit/5e90302d40ea2256b0fa08b01085eefb3d7f0965))
* or-1278 refactor tests ([372eeee](https://github.com/informatievlaanderen/association-registry/commit/372eeee559e163639a100bc40de67a2ddbf94597))

# [1.83.0](https://github.com/informatievlaanderen/association-registry/compare/v1.82.0...v1.83.0) (2022-12-22)


### Bug Fixes

* or-1278 add environment variable to acm container ([f300d70](https://github.com/informatievlaanderen/association-registry/commit/f300d701839ad5dd023469d6504b1b173a9c2bfa))
* OR-1278 use correct config folder env var ([235422b](https://github.com/informatievlaanderen/association-registry/commit/235422bab3a7addcb65e930d32c0f45ad7d1b67e))


### Features

* or-1278 add comments to request ([dec255b](https://github.com/informatievlaanderen/association-registry/commit/dec255b6edf17030f507522d48247514219a49d8))
* OR-1278 add contactgegevens ([4afc6b0](https://github.com/informatievlaanderen/association-registry/commit/4afc6b01ccd49a96c8efc46d7a97d38d015f0a5e))
* OR-1278 add logs ([8baeb65](https://github.com/informatievlaanderen/association-registry/commit/8baeb6566773d0fe0769b353c4366a62dc9cf05d))
* OR-1278 add needs ([683518c](https://github.com/informatievlaanderen/association-registry/commit/683518c9c1263908f20fb630779f105b2bc0c36b))
* or-1278 add social media as contactInfo ([27ad76b](https://github.com/informatievlaanderen/association-registry/commit/27ad76b813503e105a33b6d9faaf35f38d9e2fc6))
* or-1278 add social media as contactInfo ([df74b11](https://github.com/informatievlaanderen/association-registry/commit/df74b1134f87edac8910e3aa45edce97d0f2438a))
* OR-1278 checkout in separate job ([3a52343](https://github.com/informatievlaanderen/association-registry/commit/3a52343b7691e0b455932012ea9830f5dd6e0f64))
* OR-1278 checkout in separate job ([19dcd11](https://github.com/informatievlaanderen/association-registry/commit/19dcd11bc29b9466b616cdb8f58671a9a44ca3d0))
* OR-1278 detach ([24b544e](https://github.com/informatievlaanderen/association-registry/commit/24b544e0d08421ca8f0883e70b82962316304896))
* or-1278 fix acm commands ([6cd4603](https://github.com/informatievlaanderen/association-registry/commit/6cd46033412d302864e0fee21f367229248d2286))
* OR-1278 list identityserver dirs ([cfbe7a4](https://github.com/informatievlaanderen/association-registry/commit/cfbe7a487870703a8ca346cc86a8afb1ddb90de1))
* OR-1278 list identityserver dirs ([1f92b10](https://github.com/informatievlaanderen/association-registry/commit/1f92b104f3df5cdf4b12bfc403516e7e91f76298))
* OR-1278 list identityserver dirs ([2e1a9b2](https://github.com/informatievlaanderen/association-registry/commit/2e1a9b201737b2267ca4f42fa7d6458cbf845cde))
* OR-1278 list identityserver dirs ([974acff](https://github.com/informatievlaanderen/association-registry/commit/974acff5c0cc77d9f593b8cb3bfc51a920e7d7fd))
* OR-1278 list identityserver dirs ([3d9ca63](https://github.com/informatievlaanderen/association-registry/commit/3d9ca63fe80ccb194efc27dd6a93249d8fd4c05c))
* OR-1278 log always ([1d72beb](https://github.com/informatievlaanderen/association-registry/commit/1d72bebaae346089d1003cf30ccf9408116189ec))
* OR-1278 output acm response ([8a07442](https://github.com/informatievlaanderen/association-registry/commit/8a07442ac16de70b47a742a9be47c1ce8655ae73))
* or-1278 remove 'actual data' for deatils ([f4d120b](https://github.com/informatievlaanderen/association-registry/commit/f4d120b1aebc3d3070cf9f06349d4ea829dfa3f6))
* or-1278 remove 'actual data' for deatils ([202b218](https://github.com/informatievlaanderen/association-registry/commit/202b21819dfba812dc5a449cbfe4a2f159a08972))
* OR-1278 remove mapping ([df49a54](https://github.com/informatievlaanderen/association-registry/commit/df49a54113e448ab73fa5f3769af455a34f5718b))
* OR-1278 remove static endpoints ([6f411e3](https://github.com/informatievlaanderen/association-registry/commit/6f411e39b3db54bf0a0d1b831875546a61cfb94f))
* OR-1278 rename TelefoonNummer to Telefoon ([7c8d6b2](https://github.com/informatievlaanderen/association-registry/commit/7c8d6b26a8ca3ccb35337282a3ec5671779c9128))
* OR-1278 run acm container and only that one ([1f91204](https://github.com/informatievlaanderen/association-registry/commit/1f912041e0de2c279415e2efcf8341bc6caf0600))
* OR-1278 run docker from bash ([3836abe](https://github.com/informatievlaanderen/association-registry/commit/3836abed785899abbd25431d557d0ec12bf891b5))
* OR-1278 run on selfhosted ([41cc8c5](https://github.com/informatievlaanderen/association-registry/commit/41cc8c5f02c88081322d25328a99d21c400b4235))
* OR-1278 run on selfhosted ([af35798](https://github.com/informatievlaanderen/association-registry/commit/af3579875b4204b0ccbd3a36f6ec49a64ebd1b5c))
* OR-1278 run on ubuntu-latest ([574af7e](https://github.com/informatievlaanderen/association-registry/commit/574af7e416dd9eabb0d99f6f34f785a104a63fc6))
* OR-1278 trim trailing slashes ([1fe6e99](https://github.com/informatievlaanderen/association-registry/commit/1fe6e99ad61fd63c86ac48c464d08febb3471f13))
* or-1278 update formating ([67b7c8f](https://github.com/informatievlaanderen/association-registry/commit/67b7c8f6c8cda469329d232fcae6a56993f832cc))
* OR-1278 update identity-server-fake img ([44c4b1a](https://github.com/informatievlaanderen/association-registry/commit/44c4b1acc435a21f2ceb859c850885147a73e718))
* OR-1278 update identity-server-fake img to 5d2dc68 ([8fd8f9e](https://github.com/informatievlaanderen/association-registry/commit/8fd8f9e94032db93538811ea4bacdf953e162b82))
* OR-1278 update identity-server-fake img to e52b719 ([ede000f](https://github.com/informatievlaanderen/association-registry/commit/ede000f0d02f45fe56d25755d3c3dab923c36b7e))
* OR-1278 use correct folder in docker ([16afb1f](https://github.com/informatievlaanderen/association-registry/commit/16afb1f9f0e8a8cc14aad9d3fd8f86eca37a127b))
* OR-1278 use parent folder ([5b8d3cc](https://github.com/informatievlaanderen/association-registry/commit/5b8d3cc60d0a84e288d6b8e4f3597aafe2684c62))
* OR-1278 use parent folder ([20e1400](https://github.com/informatievlaanderen/association-registry/commit/20e140062d03e6a8ed60ae9901b35badc8f6c12c))
* OR-1278 use tmp folder ([ce83e87](https://github.com/informatievlaanderen/association-registry/commit/ce83e8712352a7a61da1ca7bed7672f28117eca8))
* OR-1278 validate contact input ([788b0ea](https://github.com/informatievlaanderen/association-registry/commit/788b0eaf58f298d91e00f0bbbe624f5796004356))


### Reverts

* Revert "feat: OR-1278 run on ubuntu-latest" ([93cf117](https://github.com/informatievlaanderen/association-registry/commit/93cf11713fce29af014175ca48279d9513ee0290))
* Revert "feat: OR-1278 remove mapping" ([95d4d0a](https://github.com/informatievlaanderen/association-registry/commit/95d4d0a4ab0a1954cefe87258b078c550366aeca))

# [1.82.0](https://github.com/informatievlaanderen/association-registry/compare/v1.81.1...v1.82.0) (2022-12-14)


### Bug Fixes

* or-1190 add paket dependencies ([c82cb55](https://github.com/informatievlaanderen/association-registry/commit/c82cb55de1ebd37614f3b89e313c2f2b721a3cdc))


### Features

* or-1190 add otlp in all projects ([eb8560b](https://github.com/informatievlaanderen/association-registry/commit/eb8560b0972bd66cbb22a19ac08d084f5771506e))
* or-1190 npgsql telemetry ([5d8dfb7](https://github.com/informatievlaanderen/association-registry/commit/5d8dfb77d71912c0384b4490cc2c844e6645ec90))
* or-1190 provide simple otel-collector; don't log via otlp ([6e9ab64](https://github.com/informatievlaanderen/association-registry/commit/6e9ab644e67023849577e288a1960292eb4f8c59))

## [1.81.1](https://github.com/informatievlaanderen/association-registry/compare/v1.81.0...v1.81.1) (2022-12-13)


### Bug Fixes

* or-1310 correct Tijdstip ([8d33a33](https://github.com/informatievlaanderen/association-registry/commit/8d33a33f0f9c73c569b989fd66471cfdc0fcd9b8))
* or-1310 fix unknown vcode test ([5223b8c](https://github.com/informatievlaanderen/association-registry/commit/5223b8c4dce6d13d6b9212875531bff0e3503e40))
* or-1310 remove unneeded test code ([fdcf28c](https://github.com/informatievlaanderen/association-registry/commit/fdcf28cf9f8a644fb32fcbfbaa59d7fb2a50662d))
* or-1310 unknown vcode returns 404 ([94dd629](https://github.com/informatievlaanderen/association-registry/commit/94dd62982a633f29cab0ccd8bc7db58ba3778e26))

# [1.81.0](https://github.com/informatievlaanderen/association-registry/compare/v1.80.0...v1.81.0) (2022-12-13)


### Bug Fixes

* OR-1310 pass initiator to test request ([5eb8332](https://github.com/informatievlaanderen/association-registry/commit/5eb8332b0e1a046c2cc21638b97579b3eee7e6ee))
* or-1310 revert sdk ([7070487](https://github.com/informatievlaanderen/association-registry/commit/7070487bebc0954efb4d65f0a6fd260afd971180))
* or-1310 run on hosted ([4529002](https://github.com/informatievlaanderen/association-registry/commit/452900295bb380420a772522a423a3bf7355a151))
* or-1310 update db name ([144d5d1](https://github.com/informatievlaanderen/association-registry/commit/144d5d18522d83ffca0d90caff36af87d865cbdc))


### Features

* or-1310 add historiek for verenigingen ([e4b25a5](https://github.com/informatievlaanderen/association-registry/commit/e4b25a57e7209f7add24e423e6c8f5fd229dd974))
* or-1310 add root conn ([0daa408](https://github.com/informatievlaanderen/association-registry/commit/0daa4089dc93574428f710f2af6209445b1ae8cb))
* or-1310 deserialize tijdstip as string io datetime ([c007428](https://github.com/informatievlaanderen/association-registry/commit/c00742878587f13db8fe021f555ee5a926d076e0))
* or-1310 don't wait for postgres and use root conn ([72ee705](https://github.com/informatievlaanderen/association-registry/commit/72ee7055d08062d6719fd15cba311e6d259ce706))
* or-1310 revert some changes ([5abde27](https://github.com/informatievlaanderen/association-registry/commit/5abde270ee927807c2fc46403c0c2a3c918a3a81))
* or-1310 run daemon when not disabled ([f0ad0b8](https://github.com/informatievlaanderen/association-registry/commit/f0ad0b863b61f784416418e5cc7f6c6d53bdf62f))


### Reverts

* Revert "chore: or-1310 disable a bunch of tests" ([8bee38f](https://github.com/informatievlaanderen/association-registry/commit/8bee38f7362285da541e573acb2d59464e0e1c7a))

# [1.80.0](https://github.com/informatievlaanderen/association-registry/compare/v1.79.0...v1.80.0) (2022-12-09)


### Features

* or-1310 fix hasHeader ([6dda0eb](https://github.com/informatievlaanderen/association-registry/commit/6dda0eb598143be6cfa11e1f0a97538af8c5a026))

# [1.79.0](https://github.com/informatievlaanderen/association-registry/compare/v1.78.0...v1.79.0) (2022-12-08)


### Features

* or-1310 fix ternary ([f6b2ed2](https://github.com/informatievlaanderen/association-registry/commit/f6b2ed2ea73ba15f026d52afc5bf2d2d4fdb4f7e))

# [1.78.0](https://github.com/informatievlaanderen/association-registry/compare/v1.77.0...v1.78.0) (2022-12-08)


### Features

* or-1310 accept existence of events without initiator ([21d9c21](https://github.com/informatievlaanderen/association-registry/commit/21d9c212cbfcb83cb44c19f3efc2e090bca7c8f9))

# [1.77.0](https://github.com/informatievlaanderen/association-registry/compare/v1.76.0...v1.77.0) (2022-12-08)


### Features

* or-1310 add historiek projection and actual metadata ([8429543](https://github.com/informatievlaanderen/association-registry/commit/842954338151c6d6650a4ae48436f0e3c101fb52))
* or-1310 add test for metadata ([0093551](https://github.com/informatievlaanderen/association-registry/commit/00935512c386c30989058b5cc8e848c3cd5396f1))
* or-1310 add well-known media types ([11f676e](https://github.com/informatievlaanderen/association-registry/commit/11f676eda8c53b597394c927c9e7b536d006fa07))
* or-1310 enable headers ([0de16c3](https://github.com/informatievlaanderen/association-registry/commit/0de16c3d83d960236530e65226f8d29ec55c9949))
* or-1310 enable headers and add 404 ([ec9ff70](https://github.com/informatievlaanderen/association-registry/commit/ec9ff70d9f195890f0f009989303082bb332c683))
* or-1310 run daemon when not disabled ([238d6e9](https://github.com/informatievlaanderen/association-registry/commit/238d6e955dde1df818fe1d9e2dcd9641ed528ab7))


### Reverts

* Revert "feat: or-1310 add test for metadata" ([928a128](https://github.com/informatievlaanderen/association-registry/commit/928a128f6f6beb3da4829fadf672563afd4e89e0))

# [1.76.0](https://github.com/informatievlaanderen/association-registry/compare/v1.75.0...v1.76.0) (2022-12-08)


### Features

* or-1190 add opentelemetry ([44f9e6c](https://github.com/informatievlaanderen/association-registry/commit/44f9e6c7369712f8de1461248f972a8e5e81e3d3))

# [1.75.0](https://github.com/informatievlaanderen/association-registry/compare/v1.74.1...v1.75.0) (2022-12-08)


### Features

* or-1310 add metadata ([da57bf5](https://github.com/informatievlaanderen/association-registry/commit/da57bf5469c55286c8314641ba79afd689900681))
* or-1310 support nonnullable ref types ([98f9bae](https://github.com/informatievlaanderen/association-registry/commit/98f9baed3fc8c027242f5855ba3dcd77083e4f2d))

## [1.74.1](https://github.com/informatievlaanderen/association-registry/compare/v1.74.0...v1.74.1) (2022-12-05)


### Bug Fixes

* or-1376 empty kbonumber should not be validated ([4c1161c](https://github.com/informatievlaanderen/association-registry/commit/4c1161c04efc8492e58e862281a02874fedd1ee8))

# [1.74.0](https://github.com/informatievlaanderen/association-registry/compare/v1.73.0...v1.74.0) (2022-12-01)


### Features

* or-1338 fix issue with existing data ([e03a3e2](https://github.com/informatievlaanderen/association-registry/commit/e03a3e2eeb02412284343ef4dea3bc6cafb45b46))

# [1.73.0](https://github.com/informatievlaanderen/association-registry/compare/v1.72.0...v1.73.0) (2022-11-30)


### Features

* or-1338 add variations to vcode + refactor ([a30d347](https://github.com/informatievlaanderen/association-registry/commit/a30d3474e21f5059b789b2b1c85992d2c8ffb953))
* or-1338 improve vCode ([2cf40b1](https://github.com/informatievlaanderen/association-registry/commit/2cf40b17c71a2c1678176707e9845e4fbc70c09c))
* OR-1338 make tests fail ([949f212](https://github.com/informatievlaanderen/association-registry/commit/949f212d91e60914c27a56e9c924fd4c1a8a111d))
* or-1338 make vcode futureproof ([10f8512](https://github.com/informatievlaanderen/association-registry/commit/10f8512dd62356f7e59c7689fbb456e3d0318e1d))

# [1.72.0](https://github.com/informatievlaanderen/association-registry/compare/v1.71.0...v1.72.0) (2022-11-29)


### Features

* or-1301 update uri for public api ([52a1265](https://github.com/informatievlaanderen/association-registry/commit/52a126594f482870dce9099b54545e861ac2c031))

# [1.71.0](https://github.com/informatievlaanderen/association-registry/compare/v1.70.0...v1.71.0) (2022-11-16)


### Features

* or-1267 use tryparseexact instead of try-catch for dateonly convertors ([b1437cd](https://github.com/informatievlaanderen/association-registry/commit/b1437cd0c01727b10098633ccf4ffbecc877d511))
* or-1303 add tests for facets, fixed some bugs ([31949ac](https://github.com/informatievlaanderen/association-registry/commit/31949ac4c2aa130158962290b05d76d53e556bbd))

# [1.70.0](https://github.com/informatievlaanderen/association-registry/compare/v1.69.0...v1.70.0) (2022-11-16)


### Features

* or-1267 add custom validation message for kbonummer too short ([5b97398](https://github.com/informatievlaanderen/association-registry/commit/5b9739814c30575abdb85d391bf680998f667486))
* or-1267 add nice validation message for invalid date format ([dd80745](https://github.com/informatievlaanderen/association-registry/commit/dd807458cc9afbea18cc46612a58ae0e50a4cfbb))
* or-1303 add query to hoofdactiviteiten facets ([4dbf820](https://github.com/informatievlaanderen/association-registry/commit/4dbf820ee96439a9fe7c7ed2faf56720524a0a24))

# [1.69.0](https://github.com/informatievlaanderen/association-registry/compare/v1.68.0...v1.69.0) (2022-11-15)


### Features

* or-1280 refactor: move stuff to domain project ([8ffa35a](https://github.com/informatievlaanderen/association-registry/commit/8ffa35ad7c5fc109b2e47cf62cfc608852a2ba5d))

# [1.68.0](https://github.com/informatievlaanderen/association-registry/compare/v1.67.0...v1.68.0) (2022-11-15)


### Features

* or-1280 cleanup test project ([69b810b](https://github.com/informatievlaanderen/association-registry/commit/69b810bade9cf79d49953295c906d4e2d0e9c7c2))
* or-1280 first try ([6c9f502](https://github.com/informatievlaanderen/association-registry/commit/6c9f5028f50dfd9e431a00f29594c40161cbc33d))
* or-1280 fix tests ([4be691b](https://github.com/informatievlaanderen/association-registry/commit/4be691b9d4b2e5fca8bf4233c821f81001046738))
* or-1280 make exceptions serializable ([382488a](https://github.com/informatievlaanderen/association-registry/commit/382488a93dfccbd9be47dce7e36eaf2108d2d00a))
* or-1280 remove obsolete DomainException ([9773eeb](https://github.com/informatievlaanderen/association-registry/commit/9773eeb2b54635d6e71a38d51316b232d5ead2ba))
* or-1280 return 400 on invalid kbo nummer ([cd03352](https://github.com/informatievlaanderen/association-registry/commit/cd03352a8a58169736052e770afe365e73b81cf3))
* or-1280 return 400 on invalid startdatum ([febbb5b](https://github.com/informatievlaanderen/association-registry/commit/febbb5baa58aa6a200362f1e96c842300df42cd0))

# [1.67.0](https://github.com/informatievlaanderen/association-registry/compare/v1.66.0...v1.67.0) (2022-11-15)


### Features

* or-1303 cleanup ([4710ae3](https://github.com/informatievlaanderen/association-registry/commit/4710ae3d223f432ce564862dae2a11d62bcdb361))
* or-1303 modify hoofdactiviteitfacets so code and naam is returned instead of naam only ([5172675](https://github.com/informatievlaanderen/association-registry/commit/51726758a18d6a9a625a2198004422e37c179fbf))
* or-1303 remove brolfeederhoofdactiviteit, move hoofdactiviteit to common project ([b545d41](https://github.com/informatievlaanderen/association-registry/commit/b545d415d582822140b88f751ea44b710bd32da0))

# [1.66.0](https://github.com/informatievlaanderen/association-registry/compare/v1.65.0...v1.66.0) (2022-11-14)


### Features

* or-1267 datumlaatsteaanpassing in metadata voor vereniging detail ([e36e773](https://github.com/informatievlaanderen/association-registry/commit/e36e77311d110cf0e670251e353affb883911682))

# [1.65.0](https://github.com/informatievlaanderen/association-registry/compare/v1.64.0...v1.65.0) (2022-11-10)


### Features

* or-1267 add test for mod97 ([2f3c59e](https://github.com/informatievlaanderen/association-registry/commit/2f3c59e8b0339804b9585fe0802eac213670574c))

# [1.64.0](https://github.com/informatievlaanderen/association-registry/compare/v1.63.0...v1.64.0) (2022-11-10)


### Features

* or-1267 add some forgotten exceptions ([3b42334](https://github.com/informatievlaanderen/association-registry/commit/3b42334950a332f50c3bc06d506c9c77e8fe57fe))
* or-1267 extra validatie, oa mod97 ([482d9e2](https://github.com/informatievlaanderen/association-registry/commit/482d9e21f76c6da0b8a5df774efc1bd85f9b51af))

# [1.63.0](https://github.com/informatievlaanderen/association-registry/compare/v1.62.0...v1.63.0) (2022-11-09)


### Features

* or-1267 add fields to registreer vereniging ([4bdd708](https://github.com/informatievlaanderen/association-registry/commit/4bdd70819ca33e4cb2266a3d8c207851618934e8))
* or-1267 cleanup vereniging state ([e90e6a1](https://github.com/informatievlaanderen/association-registry/commit/e90e6a19cf07d306e6b9f188a92cb35fde1e6288))
* or-1267 cover extra validation ([a141891](https://github.com/informatievlaanderen/association-registry/commit/a141891a21a1b15d93564febda55c435cd909018))
* or-1267 extra fields registreer vereniging: write-side (WIP) ([2f27b3e](https://github.com/informatievlaanderen/association-registry/commit/2f27b3e9a239b693ec5ff3330171dbd96256e8c3))

# [1.62.0](https://github.com/informatievlaanderen/association-registry/compare/v1.61.0...v1.62.0) (2022-11-08)


### Features

* or-1277 use BaseUrl appsetting instead of AssociationRegistryUrl ([505b915](https://github.com/informatievlaanderen/association-registry/commit/505b91554baf8bb1e38ac89916cfe040578e9d92))

# [1.61.0](https://github.com/informatievlaanderen/association-registry/compare/v1.60.0...v1.61.0) (2022-11-08)


### Features

* or-1277 add link in search verenigingen response to vereniging detail ([b1360ea](https://github.com/informatievlaanderen/association-registry/commit/b1360ea5d2c78e2cd2b2ce8e0219030c849726d8))

# [1.60.0](https://github.com/informatievlaanderen/association-registry/compare/v1.59.0...v1.60.0) (2022-11-07)


### Features

* or-1271 add code to hoofdactiviteit ([533f20b](https://github.com/informatievlaanderen/association-registry/commit/533f20b037fe903809c87bab407994175420407a))

# [1.59.0](https://github.com/informatievlaanderen/association-registry/compare/v1.58.0...v1.59.0) (2022-11-07)


### Features

* or-1245 add vereniging detail projection + endpoint ([02bcfd0](https://github.com/informatievlaanderen/association-registry/commit/02bcfd000fe881e3ad424eeb06e019fdc06d3ae0))
* or-1245 add verenigingen detail with actual data ([4ea361c](https://github.com/informatievlaanderen/association-registry/commit/4ea361c70afd17ddd792f5e0136cb55761bc9ea2))

# [1.58.0](https://github.com/informatievlaanderen/association-registry/compare/v1.57.0...v1.58.0) (2022-11-04)


### Features

* or-1293 update beheer and acm swagger docs ([108b32a](https://github.com/informatievlaanderen/association-registry/commit/108b32a865bc951ad355f05c8231effc08f13942))
* or-1295 add swagger example validationProblemDetail ([704c85a](https://github.com/informatievlaanderen/association-registry/commit/704c85af88e91ee6166a7481743967e57c422f9b))

# [1.57.0](https://github.com/informatievlaanderen/association-registry/compare/v1.56.0...v1.57.0) (2022-11-04)


### Features

* or-1295 add tests for validation ([56865e6](https://github.com/informatievlaanderen/association-registry/commit/56865e62768581e0780f0a7ca6a9120d7c4609ea))

# [1.56.0](https://github.com/informatievlaanderen/association-registry/compare/v1.55.1...v1.56.0) (2022-11-04)


### Features

* or-1295 throw validationException when modelstate is invalid ([765002e](https://github.com/informatievlaanderen/association-registry/commit/765002e90f5ed5a14b9730f6b24a67d13c287f9c))

## [1.55.1](https://github.com/informatievlaanderen/association-registry/compare/v1.55.0...v1.55.1) (2022-11-03)


### Bug Fixes

* or-1293 fix uri in docs ([8453e7d](https://github.com/informatievlaanderen/association-registry/commit/8453e7dde9240ff0f04ed28b695a0945bd8d63b9))

# [1.55.0](https://github.com/informatievlaanderen/association-registry/compare/v1.54.0...v1.55.0) (2022-11-02)


### Features

* or-1265 add dotcover to build.sh ([59ac443](https://github.com/informatievlaanderen/association-registry/commit/59ac4439676796aaffb7e36a1146cff9375c5b20))

# [1.54.0](https://github.com/informatievlaanderen/association-registry/compare/v1.53.0...v1.54.0) (2022-11-02)


### Features

* or-1268 use fluent mapping for elastic ([6227ee0](https://github.com/informatievlaanderen/association-registry/commit/6227ee01b3bcfb0a76aacc040f9e94a9762d59e9))

# [1.53.0](https://github.com/informatievlaanderen/association-registry/compare/v1.52.0...v1.53.0) (2022-10-31)


### Bug Fixes

* or-1293 adjust namespaces ([36cdde5](https://github.com/informatievlaanderen/association-registry/commit/36cdde5dd246f9a651681836cd964d403e8edefd))
* or-1293 move fix to base class ([848184e](https://github.com/informatievlaanderen/association-registry/commit/848184e188b999c7df0556f8b0e141f0bdd4749f))
* or-1293 try manually setting DisplayNameResolver ([062a3fc](https://github.com/informatievlaanderen/association-registry/commit/062a3fcf952c6bb76043b29a5bf2b44dab712a99))
* or-1293 use validator test class on inner classes ([0c4d217](https://github.com/informatievlaanderen/association-registry/commit/0c4d21783dde12bcd04c1e06127e9b4abe7f15e1))


### Features

* or-1293 update swagger docs ([88d2aa3](https://github.com/informatievlaanderen/association-registry/commit/88d2aa31772ed074fa1d94a4bf27c88dad5cc3d3))

# [1.52.0](https://github.com/informatievlaanderen/association-registry/compare/v1.51.1...v1.52.0) (2022-10-31)


### Features

* or-1273 move facets to specific object ([4d89787](https://github.com/informatievlaanderen/association-registry/commit/4d897878c85d08c2a235d69af04f32294b97d171))

## [1.51.1](https://github.com/informatievlaanderen/association-registry/compare/v1.51.0...v1.51.1) (2022-10-28)


### Bug Fixes

* or-1279 clean up ([5ac6c9c](https://github.com/informatievlaanderen/association-registry/commit/5ac6c9ce56d7bff811b9b1488cad64321167b6dd))

# [1.51.0](https://github.com/informatievlaanderen/association-registry/compare/v1.50.0...v1.51.0) (2022-10-28)


### Features

* or-1270 extra logging ([d7f8aff](https://github.com/informatievlaanderen/association-registry/commit/d7f8aff117773b9bae24020d44fc0ce6e5f0d609))
* or-1270 fix wait for ([e752b58](https://github.com/informatievlaanderen/association-registry/commit/e752b580e1fbe51237100e1dc0dc63fb512fd638))
* or-1270 refactor 2.0 ([472fc87](https://github.com/informatievlaanderen/association-registry/commit/472fc876d608635b721b2c4c85cf59a6635a1b83))
* or-1270 wait for elastic to become available ([ef0201c](https://github.com/informatievlaanderen/association-registry/commit/ef0201cf78e76895033bf61cea7d110f4b7a0fe4))

# [1.50.0](https://github.com/informatievlaanderen/association-registry/compare/v1.49.0...v1.50.0) (2022-10-27)


### Features

* or-1244 throw exception when naam is null or empty ([11fc059](https://github.com/informatievlaanderen/association-registry/commit/11fc0591db3287f1155e737c873cb75647e09ff5))

# [1.49.0](https://github.com/informatievlaanderen/association-registry/compare/v1.48.0...v1.49.0) (2022-10-27)


### Features

* or-1244 make event mediator async ([9dd59ac](https://github.com/informatievlaanderen/association-registry/commit/9dd59ac8954cc40ec823fc8e8c2415082ed59f8f))

# [1.48.0](https://github.com/informatievlaanderen/association-registry/compare/v1.47.0...v1.48.0) (2022-10-26)


### Features

* or-1274 remove adressvoorstelling + make hoofdlocatie of type locatie ([6cbe68f](https://github.com/informatievlaanderen/association-registry/commit/6cbe68f984631729068d40c5b99bd8351e92c42c))

# [1.47.0](https://github.com/informatievlaanderen/association-registry/compare/v1.46.3...v1.47.0) (2022-10-25)


### Features

* or-1274 add locatie object to projection ([fc1fc94](https://github.com/informatievlaanderen/association-registry/commit/fc1fc9437de8caceb2fb44fd554f2cadab700f5f))

## [1.46.3](https://github.com/informatievlaanderen/association-registry/compare/v1.46.2...v1.46.3) (2022-10-25)

## [1.46.2](https://github.com/informatievlaanderen/association-registry/compare/v1.46.1...v1.46.2) (2022-10-25)

## [1.46.1](https://github.com/informatievlaanderen/association-registry/compare/v1.46.0...v1.46.1) (2022-10-25)

# [1.46.0](https://github.com/informatievlaanderen/association-registry/compare/v1.45.0...v1.46.0) (2022-10-24)


### Features

* or-1244 use options in startup - public API ([b686b84](https://github.com/informatievlaanderen/association-registry/commit/b686b843a8b08b21a4afadffbc36329d3477de77))
* remove connectionstring from config ([d6b4ba8](https://github.com/informatievlaanderen/association-registry/commit/d6b4ba8c98be6c2243e44df94ed08777926f5186))

# [1.45.0](https://github.com/informatievlaanderen/association-registry/compare/v1.44.0...v1.45.0) (2022-10-24)


### Features

* or-1244 add correct xml comments ([2cee54e](https://github.com/informatievlaanderen/association-registry/commit/2cee54e21e787eb4fc4ceb975518a158f414bb1b))
* or-1244 add documentation ([a2600a2](https://github.com/informatievlaanderen/association-registry/commit/a2600a253931477f6518d1650722a5ad10ac5317))
* or-1244 add facets - Does not yet work ([20c9329](https://github.com/informatievlaanderen/association-registry/commit/20c932915fce56569b5ea8cca48f2c077f552cfe))
* or-1244 don't use automapping for now ([665c656](https://github.com/informatievlaanderen/association-registry/commit/665c6563350b6eee83d077b9a114a13ec7043a2b))
* or-1244 make exceptions constructor public ([44ad49f](https://github.com/informatievlaanderen/association-registry/commit/44ad49f15b74714dc132575dd75fd2efe14ff814))
* or-1244 make exceptions serializable ([068ea40](https://github.com/informatievlaanderen/association-registry/commit/068ea40a47c2b4c84aa78c2798df591296b530b0))
* or-1244 make exceptions serializable ([eee2c0d](https://github.com/informatievlaanderen/association-registry/commit/eee2c0dbbe69863e5b4e4d66600d70058a2c9852))
* or-1244 provide moar randomness ([04897d8](https://github.com/informatievlaanderen/association-registry/commit/04897d8af64d8bac70ef64ee07384d391eb1451b))
* or-1244 rename Id to VCode in API search response ([2bc6b85](https://github.com/informatievlaanderen/association-registry/commit/2bc6b85a9a64f0d028f1f8c860119f2d2e59962c))
* or-1244 use class io record so we can add xml docs ([85844c0](https://github.com/informatievlaanderen/association-registry/commit/85844c0fe3c1c78aa39a8e33f6458d2476f65c8d))
* or-1244 WIP pagination (untested) ([a41a245](https://github.com/informatievlaanderen/association-registry/commit/a41a24511406caae8c6c59cd115e997c2559f87c))

# [1.44.0](https://github.com/informatievlaanderen/association-registry/compare/v1.43.0...v1.44.0) (2022-10-19)


### Features

* or-1244 create repository for elastic (testablity) ([a347b2f](https://github.com/informatievlaanderen/association-registry/commit/a347b2f5b1678af492c176cb70dc5c3fd57fca8e))
* or-1244 rename repo.save -> repo.index + async ([bda4c98](https://github.com/informatievlaanderen/association-registry/commit/bda4c98ceeca320e3d297119ede7ba633473400b))

# [1.43.0](https://github.com/informatievlaanderen/association-registry/compare/v1.42.0...v1.43.0) (2022-10-19)


### Features

* or-1244 expand brolfeeder with locaties and activiteiten ([170d756](https://github.com/informatievlaanderen/association-registry/commit/170d756f4f05514d6446b0ed202dc3e53c6c8080))
* or-1244 expose new ES fields through API ([21ec93b](https://github.com/informatievlaanderen/association-registry/commit/21ec93bdacfe1d91fb75dd8df00eeb478032da78))
* or-1244 refactor VCode tests ([9698a1b](https://github.com/informatievlaanderen/association-registry/commit/9698a1ba7f18667cfd6d2b5e9ff804bd6f68404a))

# [1.42.0](https://github.com/informatievlaanderen/association-registry/compare/v1.41.1...v1.42.0) (2022-10-18)


### Features

* or-1244 add brolFeeder ([014f0ac](https://github.com/informatievlaanderen/association-registry/commit/014f0aca3c0b142a03ca0e0bada33440a0824bc9))
* or-1244 add dummy data for projections ([c2b05d2](https://github.com/informatievlaanderen/association-registry/commit/c2b05d2d5430c9d933536e4fb1ee6dd0387786ed))
* or-1244 add VCode as value object ([077f629](https://github.com/informatievlaanderen/association-registry/commit/077f6295f555e424b9373a933f0344251e11e0c1))

## [1.41.1](https://github.com/informatievlaanderen/association-registry/compare/v1.41.0...v1.41.1) (2022-10-17)

# [1.41.0](https://github.com/informatievlaanderen/association-registry/compare/v1.40.0...v1.41.0) (2022-10-13)


### Features

* or-1201 wire everything up ([f67156d](https://github.com/informatievlaanderen/association-registry/commit/f67156dc1da9046c1d45f28f9691e05677f7c9e4))
* or-1213 configure marten deamon ([0cec9b7](https://github.com/informatievlaanderen/association-registry/commit/0cec9b7427dacdc409ec4ed922979f2e46bf17f8))

# [1.40.0](https://github.com/informatievlaanderen/association-registry/compare/v1.39.0...v1.40.0) (2022-10-13)


### Features

* or-1201 add es to github actions ([aaad251](https://github.com/informatievlaanderen/association-registry/commit/aaad2513af8b158b7a88a051d0c55a60bffeab62))
* or-1201 allow search on elasticsearch index ([cd703b0](https://github.com/informatievlaanderen/association-registry/commit/cd703b0008ebfcba67a13bf89bf57e713cf1f5fc))
* or-1201 clean up ([8a34106](https://github.com/informatievlaanderen/association-registry/commit/8a34106410e4252274e69e79709e2b1894673d24))
* or-1201 map elasticsearch to search verenigingen response ([d44a04a](https://github.com/informatievlaanderen/association-registry/commit/d44a04a82d695e34459c15bd3caaa25e8bd018f8))
* or-1201 more cleanup ([cf7f3ff](https://github.com/informatievlaanderen/association-registry/commit/cf7f3ff520f1c9d93294ef93cae872e2ad8a20bc))
* or-1201 use env ([6c19b09](https://github.com/informatievlaanderen/association-registry/commit/6c19b092217e732cdb4417e339a48a8769bef50c))

# [1.39.0](https://github.com/informatievlaanderen/association-registry/compare/v1.38.0...v1.39.0) (2022-10-13)


### Features

* or-1213 add eventstore, repository and commandhandler ([a07da03](https://github.com/informatievlaanderen/association-registry/commit/a07da0393af3bcb5a51b873de25a8fbd071d0d86))
* or-1213 apply review comments ([0f71a8e](https://github.com/informatievlaanderen/association-registry/commit/0f71a8e82d87744acbbf64f196ef71f0d4ac7830))
* or-1213 cleanup ([bfcd5e0](https://github.com/informatievlaanderen/association-registry/commit/bfcd5e0fdcea70041c3201eaf391d7c2c2769edb))
* or-1213 cleanup + try fix unittest ([9e97d68](https://github.com/informatievlaanderen/association-registry/commit/9e97d68dbb16b48d4ac789ebf702721f6011a890))
* or-1213 create API endpoint for create verenigingen ([7c84363](https://github.com/informatievlaanderen/association-registry/commit/7c843635f79768f3fb3f4d67eba3ec37edecbfd7))
* or-1213 create api project, add postgress db to docker compose ([812fbc2](https://github.com/informatievlaanderen/association-registry/commit/812fbc21a463c188457e9f44991616c465e56714))
* or-1213 implement waitFor ([9d74f30](https://github.com/informatievlaanderen/association-registry/commit/9d74f30c9ce3676717f12a65ed384259a16db0dd))
* or-1213 implement waitFor amendum ([d7139c8](https://github.com/informatievlaanderen/association-registry/commit/d7139c89bbafca1595a15d53cede6dfcc786ff12))
* or-1213 limit number of tries for WaitFor ([239c26f](https://github.com/informatievlaanderen/association-registry/commit/239c26f8af4882dea975ff109b314182894c2e55))
* or-1213 refactor tests ([9a41eaf](https://github.com/informatievlaanderen/association-registry/commit/9a41eaf13ac70d5bbefa90bf2eeb36d5fdeb46fd))
* or-1213 try fix hanging tests ([7038473](https://github.com/informatievlaanderen/association-registry/commit/70384731c3fb9c1d47bf77532196242ac4d97838))

# [1.38.0](https://github.com/informatievlaanderen/association-registry/compare/v1.37.0...v1.38.0) (2022-10-07)


### Features

* OR-1234 use workspace env var ([#51](https://github.com/informatievlaanderen/association-registry/issues/51)) ([705ec56](https://github.com/informatievlaanderen/association-registry/commit/705ec56cfab6dca7e7b1aac4c1be0bcc2f91d5ac))

# [1.37.0](https://github.com/informatievlaanderen/association-registry/compare/v1.36.0...v1.37.0) (2022-10-07)


### Features

* or-1234 enable identityserver fake image for VR ([fec8eeb](https://github.com/informatievlaanderen/association-registry/commit/fec8eebf1c15d05dcf10df0b03de984371d77737))
* or-1234 rollback github_workspace ([bfc85c2](https://github.com/informatievlaanderen/association-registry/commit/bfc85c218effbada08328f00ba048aca893e558a))
* or-1234 try with ([222fe01](https://github.com/informatievlaanderen/association-registry/commit/222fe015841d9804c5c022f7e5bc2e1d362f36d9))

# [1.36.0](https://github.com/informatievlaanderen/association-registry/compare/v1.35.0...v1.36.0) (2022-10-04)


### Features

* or-1237 allow short rrns ([1b073ca](https://github.com/informatievlaanderen/association-registry/commit/1b073ca50baa5b1726b1237ea346d58563a7899e))
* or-1237 allow sort rrns in request ([277830f](https://github.com/informatievlaanderen/association-registry/commit/277830f5a315cd92995ab97a6f7390a2ce56d763))

# [1.35.0](https://github.com/informatievlaanderen/association-registry/compare/v1.34.0...v1.35.0) (2022-09-30)


### Features

* OR-1229 update base uri ([23f2883](https://github.com/informatievlaanderen/association-registry/commit/23f28837c1d05ed8d9b4a9b01e49cef0d267d18b))

# [1.34.0](https://github.com/informatievlaanderen/association-registry/compare/v1.33.0...v1.34.0) (2022-09-30)


### Features

* or-1224 fix swagger documentation for dateonly ([30e5fb9](https://github.com/informatievlaanderen/association-registry/commit/30e5fb9e630483092ca6c4f0db7f5815f698b2be))

# [1.33.0](https://github.com/informatievlaanderen/association-registry/compare/v1.32.0...v1.33.0) (2022-09-29)


### Features

* rename search to zoeken ([f3e397d](https://github.com/informatievlaanderen/association-registry/commit/f3e397ddccc10c4ecee86897fed8f0a299134493))

# [1.32.0](https://github.com/informatievlaanderen/association-registry/compare/v1.31.0...v1.32.0) (2022-09-29)


### Features

* or-1234 clean up ([a64d959](https://github.com/informatievlaanderen/association-registry/commit/a64d9590ee979c60b3223fda7144ba9993468676))
* or-1234 make identity server image generic ([cdde91c](https://github.com/informatievlaanderen/association-registry/commit/cdde91cfc1bdc23464ea092c6fe8899339388b81))

# [1.31.0](https://github.com/informatievlaanderen/association-registry/compare/v1.30.0...v1.31.0) (2022-09-29)


### Features

* or-1177 add security policy to ACM API ([65290b5](https://github.com/informatievlaanderen/association-registry/commit/65290b57447caafa5f2ced462b80e355389641a7))
* or-1177 skip security tests until we have an identity server image that we can use in the CI for VR ([bb48a87](https://github.com/informatievlaanderen/association-registry/commit/bb48a876550708945a7fe50c8908bfb563ca52fb))

# [1.30.0](https://github.com/informatievlaanderen/association-registry/compare/v1.29.0...v1.30.0) (2022-09-28)


### Features

* or-1230 fix urls of activiteiten ([5552c75](https://github.com/informatievlaanderen/association-registry/commit/5552c7545d158f9207c1bc999fca00a12acc0645))

# [1.29.0](https://github.com/informatievlaanderen/association-registry/compare/v1.28.0...v1.29.0) (2022-09-28)


### Features

* OR-1230 add meaningfull examples ([b76892c](https://github.com/informatievlaanderen/association-registry/commit/b76892c41bb094e01d14a330698e8c950710e5a4))

# [1.28.0](https://github.com/informatievlaanderen/association-registry/compare/v1.27.0...v1.28.0) (2022-09-28)


### Features

* OR-1229 replace json-ld contexts with Uri ([b62baf5](https://github.com/informatievlaanderen/association-registry/commit/b62baf547e29f1416958dc22597b03ee7659936e))

# [1.27.0](https://github.com/informatievlaanderen/association-registry/compare/v1.26.0...v1.27.0) (2022-09-27)


### Features

* or-1209 expose json-ld context files ([fa3107a](https://github.com/informatievlaanderen/association-registry/commit/fa3107a46730f0b3212661a6b89fecbc34cadaf4))

# [1.26.0](https://github.com/informatievlaanderen/association-registry/compare/v1.25.0...v1.26.0) (2022-09-27)


### Features

* or-1224 also apply dateonly convertor in examples ([3496b86](https://github.com/informatievlaanderen/association-registry/commit/3496b862fef515ce8690616d444de9467bad668c))
* or-1224 cleanup ([7bf39d7](https://github.com/informatievlaanderen/association-registry/commit/7bf39d7843d7540b4446f6496b7695faafcad227))
* or-1224 moar cleanup ([93c0f87](https://github.com/informatievlaanderen/association-registry/commit/93c0f875744e5a2d08820f92e7bf59566d66e3bd))

# [1.25.0](https://github.com/informatievlaanderen/association-registry/compare/v1.24.1...v1.25.0) (2022-09-26)


### Features

* or-1220 add beheerder to activity ([7c5157e](https://github.com/informatievlaanderen/association-registry/commit/7c5157e8b804153482ba9b82d27be0ad7a590366))

## [1.24.1](https://github.com/informatievlaanderen/association-registry/compare/v1.24.0...v1.24.1) (2022-09-26)


### Bug Fixes

* fix build ([e51bcfb](https://github.com/informatievlaanderen/association-registry/commit/e51bcfbba6734ebb903fd6e9278521bdd1add2bb))

# [1.24.0](https://github.com/informatievlaanderen/association-registry/compare/v1.23.0...v1.24.0) (2022-09-26)


### Features

* or-1221 add laatstgewijzigd date to vereningdetail ([46a6083](https://github.com/informatievlaanderen/association-registry/commit/46a60838027e76c084bab0fda486cbe9ed8b2291))

# [1.23.0](https://github.com/informatievlaanderen/association-registry/compare/v1.22.1...v1.23.0) (2022-09-26)


### Features

* or-1224 format start and end dates ([bc5af3a](https://github.com/informatievlaanderen/association-registry/commit/bc5af3a4a20616fcff3f076fb7d0321aa9143755))

## [1.22.1](https://github.com/informatievlaanderen/association-registry/compare/v1.22.0...v1.22.1) (2022-09-20)


### Bug Fixes

* OR-1210 fix staging push images ([392af58](https://github.com/informatievlaanderen/association-registry/commit/392af58e3b6ed5596ca4a3bce2f12418092f0a42))

# [1.22.0](https://github.com/informatievlaanderen/association-registry/compare/v1.21.0...v1.22.0) (2022-09-20)


### Features

* or-1210 fix build version ([6873168](https://github.com/informatievlaanderen/association-registry/commit/6873168c64391b487647ace0ff0fbdb7736e1041))

# [1.21.0](https://github.com/informatievlaanderen/association-registry/compare/v1.20.2...v1.21.0) (2022-09-20)


### Features

* or-1210 add build dependency ([115f88c](https://github.com/informatievlaanderen/association-registry/commit/115f88c6aa6e59ee951dd3e475790d28aaba77b8))

## [1.20.2](https://github.com/informatievlaanderen/association-registry/compare/v1.20.1...v1.20.2) (2022-09-20)


### Bug Fixes

* OR-1210 parse repo name ([cda5a23](https://github.com/informatievlaanderen/association-registry/commit/cda5a235f32ebf8d5597e1d2bbfeafaa9e7869b4))

## [1.20.1](https://github.com/informatievlaanderen/association-registry/compare/v1.20.0...v1.20.1) (2022-09-20)


### Bug Fixes

* OR-1210 install python deps ([75800ed](https://github.com/informatievlaanderen/association-registry/commit/75800ed32adc38912ff197e36e65a5b300a6dfb7))

# [1.20.0](https://github.com/informatievlaanderen/association-registry/compare/v1.19.0...v1.20.0) (2022-09-19)


### Features

* or-1210 add cache python ([0d93e18](https://github.com/informatievlaanderen/association-registry/commit/0d93e1807828f1f75e0da3906960ffea0d1a9f63))

# [1.19.0](https://github.com/informatievlaanderen/association-registry/compare/v1.18.0...v1.19.0) (2022-09-19)


### Features

* or-1210 add python + use needs ([773e400](https://github.com/informatievlaanderen/association-registry/commit/773e400bdd22d6ce6626df2958c29d6fa11d5f93))

# [1.18.0](https://github.com/informatievlaanderen/association-registry/compare/v1.17.0...v1.18.0) (2022-09-19)


### Features

* or-1210 fix cache paket for atlassian ([3106eef](https://github.com/informatievlaanderen/association-registry/commit/3106eef244d74dc9c077654832815f2645b932e7))

# [1.17.0](https://github.com/informatievlaanderen/association-registry/compare/v1.16.0...v1.17.0) (2022-09-19)


### Features

* or-1210 fix download path of artifact ([96f039b](https://github.com/informatievlaanderen/association-registry/commit/96f039b71480e4521f06a946413facc2bd5757d9))

# [1.16.0](https://github.com/informatievlaanderen/association-registry/compare/v1.15.0...v1.16.0) (2022-09-19)


### Features

* or-1210 modify docker image names ([b24fb10](https://github.com/informatievlaanderen/association-registry/commit/b24fb109787524a62791e9414116dc7a61d6ed96))

# [1.15.0](https://github.com/informatievlaanderen/association-registry/compare/v1.14.1...v1.15.0) (2022-09-19)


### Features

* or-1210 split build with artifacts ([25f2893](https://github.com/informatievlaanderen/association-registry/commit/25f2893e1735672efd15c7ef53a3ea5d606a622c))

## [1.14.1](https://github.com/informatievlaanderen/association-registry/compare/v1.14.0...v1.14.1) (2022-09-19)


### Bug Fixes

* OR-1181 modified awscurl polling action path in github workflows ([e970e97](https://github.com/informatievlaanderen/association-registry/commit/e970e97208db50f3fa88ec4720130ef6d91f2a17))

# [1.14.0](https://github.com/informatievlaanderen/association-registry/compare/v1.13.0...v1.14.0) (2022-09-19)


### Features

* import resharpersettings from OR ([31cda81](https://github.com/informatievlaanderen/association-registry/commit/31cda81514f68f4bdd6545706ee20aced9456a0e))

# [1.13.0](https://github.com/informatievlaanderen/association-registry/compare/v1.12.0...v1.13.0) (2022-09-15)


### Features

* or-1181 add detail vereniging ([f091615](https://github.com/informatievlaanderen/association-registry/commit/f09161517961455a9bbb8ff81737abd0c60dc3f8))

# [1.12.0](https://github.com/informatievlaanderen/association-registry/compare/v1.11.0...v1.12.0) (2022-09-09)


### Features

* or-1182 add xml comments ([abe42ff](https://github.com/informatievlaanderen/association-registry/commit/abe42ff7d16c92906cefd8ea5acee5a1ff3bdf85))

# [1.11.0](https://github.com/informatievlaanderen/association-registry/compare/v1.10.0...v1.11.0) (2022-09-08)


### Bug Fixes

* OR-1182 add default bucket ([56674ee](https://github.com/informatievlaanderen/association-registry/commit/56674ee3654ad7a118a6f356f4563bf895018e5c))
* OR-1182 add default bucket to pre merge tests ([52eab89](https://github.com/informatievlaanderen/association-registry/commit/52eab898df402b99c1d89880ddc193976ca5b239))
* OR-1182 add minio to pre-merge tests workflow ([7af2f21](https://github.com/informatievlaanderen/association-registry/commit/7af2f21ef9ef1209e5cbfd6a3e882cd0ca22fc9a))
* OR-1182 seed minio with json ld context blob ([ae0fb53](https://github.com/informatievlaanderen/association-registry/commit/ae0fb536a2021408eab8d6192555e689c9d8d280))


### Features

* OR-1182 add json-ld context to ListVerenigingen ([a9220f6](https://github.com/informatievlaanderen/association-registry/commit/a9220f645280af2a2e1fed37bd8b5d9248c06e50))

# [1.10.0](https://github.com/informatievlaanderen/association-registry/compare/v1.9.0...v1.10.0) (2022-09-07)


### Features

* or-1182 implement verenigingen controller + éfirst integration tests ([c31052d](https://github.com/informatievlaanderen/association-registry/commit/c31052dd7e0e296325e2fca22edc76613711edd0))
* or-1182 optimisation of resources in tests ([895736d](https://github.com/informatievlaanderen/association-registry/commit/895736d889bcadfa18b723026ae41e809a7e3097))

# [1.9.0](https://github.com/informatievlaanderen/association-registry/compare/v1.8.0...v1.9.0) (2022-08-31)


### Features

* or-1158 add get all enpoint + add put data endpoint ([2ea586a](https://github.com/informatievlaanderen/association-registry/commit/2ea586ab512a431137997f5fa37aab78c4a31440))

# [1.8.0](https://github.com/informatievlaanderen/association-registry/compare/v1.7.0...v1.8.0) (2022-08-30)


### Features

* or-1158 fix ([0405576](https://github.com/informatievlaanderen/association-registry/commit/040557622cc4cc9949106bb604de629ce1c38a44))
* or-1158 move startup and program to root ([aa414b4](https://github.com/informatievlaanderen/association-registry/commit/aa414b4077bdef52788a6fec6dd93d93e2f8d5c0))

# [1.7.0](https://github.com/informatievlaanderen/association-registry/compare/v1.6.1...v1.7.0) (2022-08-30)


### Features

* or-1175 add manual deploy workflow ([30ed93d](https://github.com/informatievlaanderen/association-registry/commit/30ed93ded8d8ef5cc1549e4c45768a5da6e26132))

## [1.6.1](https://github.com/informatievlaanderen/association-registry/compare/v1.6.0...v1.6.1) (2022-08-29)


### Bug Fixes

* or-1175 check if blob exists before trying to delete ([9c025b6](https://github.com/informatievlaanderen/association-registry/commit/9c025b6dfa4d1fabde7ee9d3693a502303858f0a))

# [1.6.0](https://github.com/informatievlaanderen/association-registry/compare/v1.5.1...v1.6.0) (2022-08-29)


### Features

* or-1175 log s3 bucket and blob name ([ab6e01d](https://github.com/informatievlaanderen/association-registry/commit/ab6e01de2ab9e6b54aaae3a95865a90b5d48ef78))

## [1.5.1](https://github.com/informatievlaanderen/association-registry/compare/v1.5.0...v1.5.1) (2022-08-26)


### Bug Fixes

* or-1175 remove unused code ([02929f4](https://github.com/informatievlaanderen/association-registry/commit/02929f466534db8ddd00a57ac9454e75f9c5bfd6))

# [1.5.0](https://github.com/informatievlaanderen/association-registry/compare/v1.4.6...v1.5.0) (2022-08-26)


### Features

* or-1175 allow minio to be empty on startup + use S3 ([65249a7](https://github.com/informatievlaanderen/association-registry/commit/65249a7c10f48c861efd882e3ed4c0806790edab))
* or-1175 integratie met s3 ([eaecac7](https://github.com/informatievlaanderen/association-registry/commit/eaecac7d68bf7fd8001f1f115f666eea1cfc54d0))
* or-1175 integratie met s3 ([7535985](https://github.com/informatievlaanderen/association-registry/commit/753598556695e1ca6aa62ae1d49ed9adf5646ba3))

## [1.4.6](https://github.com/informatievlaanderen/association-registry/compare/v1.4.5...v1.4.6) (2022-08-24)


### Bug Fixes

* or-1170 change ports of docker app ([e60100f](https://github.com/informatievlaanderen/association-registry/commit/e60100fbba3ff84be1dce237f9ed035d20bb10db))

## [1.4.5](https://github.com/informatievlaanderen/association-registry/compare/v1.4.4...v1.4.5) (2022-08-24)


### Bug Fixes

* or-1170 tag before push ([6248733](https://github.com/informatievlaanderen/association-registry/commit/6248733364cce3af741f63c4022ce9dfd0cc3bd7))

## [1.4.4](https://github.com/informatievlaanderen/association-registry/compare/v1.4.3...v1.4.4) (2022-08-24)


### Bug Fixes

* or-1170 remove unused env var ([bf6debb](https://github.com/informatievlaanderen/association-registry/commit/bf6debb8962c9c6c3021dd29e0d0a4200efa1bd0))

## [1.4.3](https://github.com/informatievlaanderen/association-registry/compare/v1.4.2...v1.4.3) (2022-08-23)


### Bug Fixes

* or-1170 also push to staging ([8d45ee3](https://github.com/informatievlaanderen/association-registry/commit/8d45ee338a8f431696b1c075346eaa891365014c))

## [1.4.2](https://github.com/informatievlaanderen/association-registry/compare/v1.4.1...v1.4.2) (2022-08-23)


### Bug Fixes

* or-1170 rename ([c0b1984](https://github.com/informatievlaanderen/association-registry/commit/c0b1984374c861ef348b19146c8336c9dbf18000))

## [1.4.1](https://github.com/informatievlaanderen/association-registry/compare/v1.4.0...v1.4.1) (2022-08-23)


### Bug Fixes

* or-1170 use TST registry to tag ([7855da6](https://github.com/informatievlaanderen/association-registry/commit/7855da6c81b531730b9189007183f4b2ca0cc4df))

# [1.4.0](https://github.com/informatievlaanderen/association-registry/compare/v1.3.0...v1.4.0) (2022-08-23)


### Features

* or-1170 push image to TST ([8cb4b67](https://github.com/informatievlaanderen/association-registry/commit/8cb4b67456a6a716a0521482fb9741c4a27b2b3a))

# [1.3.0](https://github.com/informatievlaanderen/association-registry/compare/v1.2.0...v1.3.0) (2022-08-23)


### Features

* or-1170 try updating deps with critical issues ([a0504dc](https://github.com/informatievlaanderen/association-registry/commit/a0504dc9bc2d677c7d11e46d4a107e7c27a78168))

# [1.2.0](https://github.com/informatievlaanderen/association-registry/compare/v1.1.0...v1.2.0) (2022-08-23)


### Features

* or-1170 add dependabot ([afdb6d0](https://github.com/informatievlaanderen/association-registry/commit/afdb6d045680d2826116d8d55726626ce322c6eb))

# [1.1.0](https://github.com/informatievlaanderen/association-registry/compare/v1.0.0...v1.1.0) (2022-08-23)


### Features

* enable jira/confluence releases ([53b0220](https://github.com/informatievlaanderen/association-registry/commit/53b0220505e286ac8c379c55e3c0a17b2f7afedb))

# 1.0.0 (2022-08-23)


### Bug Fixes

* or-1170 fix namespacing and docs ([b0c1f74](https://github.com/informatievlaanderen/association-registry/commit/b0c1f74d37bdf9fa8061b05c0a46786065e47b01))
* or-1170 fix trailing comma ([d12107f](https://github.com/informatievlaanderen/association-registry/commit/d12107fabac030948dcfde65632802b00685307c))
* or-1170 use github token ([b947878](https://github.com/informatievlaanderen/association-registry/commit/b9478780f8bf7184ea09e6cdba7f36c3e2c242b7))


### Features

* or-1130 implement possibility to change verenigingen for a rijksregisternummer ([19763b4](https://github.com/informatievlaanderen/association-registry/commit/19763b45fa7ae02840535d1581293a6e57bea087))
* or-1130 set up acm api ([5280fb4](https://github.com/informatievlaanderen/association-registry/commit/5280fb42a0995776fe6f36566926a54f612dbec9))
* or-1150 add static data ([ea1962c](https://github.com/informatievlaanderen/association-registry/commit/ea1962c4289fe0c1068d63e7fcab014a2c5d0cf4))
* or-1150 handle not found ([11442d2](https://github.com/informatievlaanderen/association-registry/commit/11442d2a2f1152935a85c1bfee09caabdbf90e86))
* or-1170 add ci workflow ([ba6dcb4](https://github.com/informatievlaanderen/association-registry/commit/ba6dcb4ac862d5cb81b56df30ae02d05e8553d1c))
* or-1170 add code owners file ([0559c8e](https://github.com/informatievlaanderen/association-registry/commit/0559c8ea6a8cb3bc894640577ee1f87b85b78def))
* or-1170 add package.json and fix paket.template ([9d8d1aa](https://github.com/informatievlaanderen/association-registry/commit/9d8d1aaeb92c29af7f57b53ea29aeb7c00c89b0d))
