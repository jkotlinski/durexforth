## How to Release
1. Update CHANGELOG.md with new version and release date.
2. Commit CHANGELOG.md.
3. `git tag v#.#.#`
4. `git push --tag`
5. `make deploy`
6. Create new release in Github. Copy change list from CHANGELOG.md and add the binaries in deploy folder.
