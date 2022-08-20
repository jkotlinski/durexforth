## How to Release
1. `git checkout master`
1. Update CHANGELOG.md with new version and release date.
1. Commit CHANGELOG.md.
1. `git tag v#.#.#`
1. `git push --tag`
1. `make deploy`
1. Create new release in Github. Copy change list from CHANGELOG.md and add the binaries in deploy folder.
1. `git checkout github-pages`
1. `git merge master`
1. `make docs`
1. `git commit docs`
1. `git push`
