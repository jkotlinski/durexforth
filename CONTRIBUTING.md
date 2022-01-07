# Thanks!

Thank you for considering a contribution to Durexforth!

## Submitting an Issue

Found a problem?  Have a great idea?  Check the Issue database!

If you don't see your problem or idea listed there, it might
be time to open a new issue. To do so, we have some guidelines:

* Check for duplicate issues first, including closed issues
* Give your issue a descriptive title
* Put details about bug or idea in the body
* Include relevant details, such as the version number


## Contributing Code

### Building Durexforth

Compiling the Durexforth source requires the following software:

* acme - the cross-assembler
* vice - the c64 emulator
* pdflatex - provided by a TeX installation
* make - the build system

Obtaining and installing this software is beyond the scope of this document.

Building the durexforth disk and cart can be achieved by executing
```
# make clean && make deploy
```
The base system and documentation will be produced, then the remainder of the
system will be compiled in the Vice emulator.

Once the system is saved to disk and the prompt is visible, the program has
been successfully built.  At this point you may test the system by typing
at the prompt.  You may wish to disable Vice Warp Mode before doing so by
pressing MOD-W or clicking on the green light next to 'Warp' on the status
bar.  To completely test the system, you may use the command
```
include test
```
When you are finished testing, close the emulator.

Then, the compiled software is built into the cart image. If the program
is too large, the cartridge conversion will fail.

The outputs of the build process are placed in the `deploy/` directory.
You can test the cartridge image with
```
# x64sc deploy/durexforth-3.0.0-M.crt
```

### Submitting a Pull Request

If you plan to submit changes to Durexforth, you should:

* Always `include test` before committing code to the branch
* Give the pull request a descriptive name
  - e.g.: "Changed FOO to BAR"
  - not e.g.: "Updated file.ext"
* Elaborate on your change in the body of the pull request
* Include links to related issues

The text in the body of the pull request should explain what problem the patch
solves, or what new functionality it affords.  Give as much detail as required.
