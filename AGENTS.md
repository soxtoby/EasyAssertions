# Working on EasyAssertions

## Changelog

A change a user would notice gets an entry under `## Unreleased` in `CHANGELOG.md`, under one of these headings, which `./do prepare-release` reads to choose the next version.
Keep entries short and user-focused.

- `### Breaking`: an assertion, API, or dependency changed in a way that breaks existing tests or custom assertions. Major.
- `### Added`: a new assertion or API. Minor.
- `### Changed`: a deliberate change to existing behaviour, including failure messages. Minor.
- `### Fixed`: a bug fix. Patch.

## Releasing

1. `./do prepare-release` moves the Unreleased notes under the next version and bumps `<Version>` in `EasyAssertions.csproj`.
2. Commit, then tag the commit with the version prefixed by `v` (e.g. `v4.0.1`) and push the tag.
3. The Release workflow tests, packs, publishes to NuGet, and creates the GitHub release from the changelog notes.
