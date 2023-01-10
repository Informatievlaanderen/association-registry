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
