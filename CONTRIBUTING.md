# Thanks!

Thank you for considering a contribution to durexForth!

## Submitting an Issue

Found a problem?  Have a great idea?  Check the Issue database!

If you don't see your problem or idea listed there, it might
be time to open a new issue. To do so, we have some guidelines:

* Check for duplicate issues first, including closed issues
* Give your issue a descriptive title
* Put details about bug or idea in the body
* Include relevant details, such as the version number


## Contributing Code

### Adding or modifying source code

We welcome new features and bug fixes!  To keep things sane, we suggest:

* Use a text editor that supports EditorConfig to keep source tidy
* Follow the conventions and idioms of the surrounding code
  - Indentation, letter case, etc.
* Include stack effect comments on new words
* Include comments documenting complex or non-self-descriptive code

### Building durexForth

Building Durexforth requires the following software:

* acme - the cross-assembler (v0.97 or greater)
* vice - the c64 emulator
* c1541 - comes with Vice
* make - the build system
* asciidoctor and asciidoctor-pdf - text publishing

Obtaining and installing above software is beyond the scope of this document.

Build the durexForth disk and cartridge by executing:
```
# make clean && make deploy
```
The base system and documentation is produced, after which the Forth code is compiled in Vice.
Once completed, a new Vice instance will execute the test suite.
You may want to disable warp mode to hear the music test.
After the tests, the cartridge image is built.
If the program is too large, the cartridge conversion will fail.

Once make completes, the generated files can be found in the `deploy/` directory.
You can test the cartridge image with e.g.
```
# x64sc deploy/durexforth-3.0.0-M.crt
```

### Submitting a Pull Request

When submitting changes to durexForth, you should:

* `make deploy` to run the test suite. If existing tests do not cover your changes, consider adding new ones.
* If the change is significant to end users, describe it in `CHANGELOG.md`
* Give the pull request a descriptive name
  - e.g.: "Changed FOO to BAR"
  - not e.g.: "Updated file.ext"
* Elaborate on your change in the body of the pull request
* Include links to related issues

The text in the body of the pull request should explain what problem the patch
solves, or what new functionality it affords.  Give as much detail as required.
