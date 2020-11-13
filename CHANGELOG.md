# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed
 - LS abort on LOADB error, PAGE upon success.
 - RDIR improve placement of MORE.
 - LS now accepts wildcards, drive #'s or not.
 - Dictionary restructured, now header and code data are split.
 - Header data is not a linked list, and grows downward from $9fff. Record structure: `len_flags | str | xt`
 - Prompt displays `ful` when there is less than 256 bytes of dictionary space left.
 - FIND-NAME now returns a name token per the standard proposal
 - V internal words hidden, BUFSTART no longer variable, fixed at $7000.
 - LATEST changed from VARIABLE to VALUE
### Added
 - RDIR will display directory formatted data anywhere in memory.
 - PAD Scratch pad memory set to cassette buffer. Untouched by DurexForth
 - DOWORDS, which allows executing an xt for every word in the wordlist 
 - Turn-key operation utilities in TURNKEY:
 - SAVE-PACK packs the dictionary together before saving, which unpacks at runtime.
 - SAVE-PRG removes the dictionary and saves the program
 - TOP returns the address of the last byte of the header structure. The value at this address will always be 0.
 - TOP! can be used to specify the position of header data.
 - HIDE removes a word from the word list, while leaving its definition in place.
 - DEFINE assigns HERE to a word in the word list and begins compilation.
 - DEFCODE does the same as DEFINE, but begins a CODE: segment instead.
 - V commands: A, R, +, -, HOME, e, C, D, s, S, H, L, M, ^w, f, F
### Fixed
 - V did not compile in DECIMAL mode.
 - V long line
 - Documented SEE concatenating subsequent :NONAME
 - SP-X! had bug in most significant bit
 - BYE now calls warm reset vector

## [2.0.0] - 2020-03-22

### Changed
 - Startup BASE is now DECIMAL instead of HEX.
 - S" now only works when compiling.
 - Renamed VI to V, to avoid confusion with vi.
 - Renamed >CFA to >XT.
### Added
 - :NONAME, BYE, /STRING, PARSE-NAME, FIND-NAME
 - VICE label dumper. 'include viceutil' and DUMP-LABELS
   to generate a label file for VICE debugging.
 - V: * and n commands
### Fixed
 - Evaluating an INCLUDE would corrupt the evaluated string.
   Broke in 1.6.7.
 - Code stripping tutorial did not work.
 - WORD did not work while interpreting.
 - REQUIRED marked a file as included even if it was not found.
 - MARKER did not reset REQUIRED state.
 - V: / was broken when searching for a single character.
 - V: q: switched in BASIC rom.
 - V: error when clearing buffer with dd.
 - V: cancelling :w! changed the active filename.

## [1.6.8] - 2019-05-22

### Changed
 - when parsing, treat shift+space like space. it used to
   be like this but accidentally (?) changed in 1.6.0.

## [1.6.7] - 2019-05-20

### Fixed
 - include, included would clobber the text input buffer.

## [1.6.6] - 2019-05-12

### Changed
 - sprite: sp-data now use . instead of bl to set 0 bits
 - forth: accept now reads keyboard only
### Fixed
 - sprite: sp-on, sp-off had inverted sprite number
 - gfx: text now supports upper/lower case

## [1.6.5] - 2018-10-24

 - editor: fixed dd crash at end of buffer
 - editor: slower key repeat settings
 - editor: made f7 work in insert mode

## [1.6.4] - 2018-04-22

 - forth: INCLUDED now interprets the file
 - forth: added ?DO
 - forth: added DEFER DEFER! IS to compat
 - forth: made loadb return address after last written byte
 - debug: made see show branch

## [1.6.3] - 2017-10-07

 - bring back float module

## [1.6.2] - 2017-08-03

 - forth: various see improvements
 - forth: moved >body to compat

## [1.6.1] - 2016-02-10

 - forth: evaluate failed for strings that ended at page boundary
 - forth: print error in case include fails
 - forth: space optimize of
 - forth: moved 2over, 2swap to compat
 - cart: made cartridge boot faster
 - cart: use Simons' Basic cart type to make $a000-$bfff RAM
 - editor: starting without filename now doesn't read from disk
 - editor: :w on unnamed buffer now prints "no filename"

## [1.6.0] - 2016-02-02

 - forth: now available as a 16 kB cartridge image. the cartridge
   has the same functionality as the disk version, but boots faster.
 - forth: removed gfx from standard modules
 - forth: dropped all usage of BASIC (including float module)
 - forth: moved rarely used words to compat module
 - asm: renamed zptmp zptmp2 zptmp3 => w w2 w3
 - asm: renamed sp0 sp1 => lsb msb
 - gfx: moved bitmap graphics area from $8c00-$8fff, $a000-$bfff
   to $cc00-$cfff, $e000-$ffff
 - doc: expanded assembler documentation
 - editor: removed fg

## [1.5.4] - 2016-01-30

 - bugfix: vi :w didn't work with filenames longer than 6 characters
 - bugfix: vi :w! captured control characters in filename
 - bugfix: changed vi :w to avoid 1541 bugs
 - editor: improved feedback when saving
 - editor: stop using backup files
 - forth: added require, required
 - doc: documented assembler, sid

## [1.5.3] - 2016-01-20

 - bugfix: various fixes for evaluate/vi F7

## [1.5.2] - 2016-01-16

 - bugfix: find now handles the empty string correctly
 - bugfix: editor didn't work correctly with big files (over $1000 bytes)
 - forth: the interpreter now prints "ok" after interpreting
 - forth: abort" now inverts the text
 - editor: open files with 'vi file' instead of 's" file" vi'
 - editor: no longer possible to join lines by shift+backspace
   in insert mode

## [1.5.1] - 2016-01-10

 - bugfix: evaluate and vi F7 didn't work for multi-line buffers
 - bugfix: see did not work on the latest word
 - bugfix: e.g. 'A' was parsed as 'a'
 - bugfix: case insensitivity did not work for z/Z
 - editor: use fg to bring back editor instead of RESTORE
 - disk: added include (e.g. "include gfx" loads gfx module)
 - disk: added device ( device# -- ) to switch active device
 - disk: renamed load => included
 - forth: added marker
 - forth: made dump work in decimal mode
 - forth: made .( immediate
 - forth: removed no-tce
 - forth: bring back modules

## [1.5.0] - 2016-01-04

 - forth: made +loop work according to standard
 - forth: added support for #, $, %, char parsing.
          e.g. #123, $d020, %1001001, 'z'
 - forth: ' now aborts if it does not find a word
 - forth: made "type" disable c64 quote mode (fixes "words")
 - forth: state ( -- flag ) changed to state ( -- addr )
 - forth: added count, >body, <#, #>, hold, sign, #, #s, >number, move,
          accept
 - forth: removed hide, hide-to, hidden, forget, scratch, cmove, cmove>
 - forth: fixed yet another tail call elimination crash
 - forth: made while/repeat implementation conform to standard
 - forth: made word conform to standard
 - forth: made "immediate", "no-tce" non-immediate
 - forth: made sure immediate does not toggle

Thanks to polluks for suggestions!

## [1.4.6] - 2015-12-30

 - bugfix: disabled tail call elimination for literal ['] [char]
           to avoid some crashes
 - bugfix: <, rshift, u. did not support negative numbers
 - forth: made word finding + numbers case insensitive
 - forth: renamed char to getc, and added a new char that works
          according to the Forth 2012 standard.
 - forth: added abort, abort", 2@, 2!, char+, chars, align, aligned
 - forth: key, key? now work only with keyboard
 - forth: made quit reset input and fall back to keyboard
 - forth: fill ( ch addr len ) changed to fill ( addr len ch )
 - forth: allot ( -- n ) changed to allot ( -- )
 - forth: made base a variable instead of a value
 - forth: changed variable; data field is now after code field

Thanks to Christian Johansson for suggestions!

## [1.4.5] - 2015-12-23

 - forth: / now uses floored signed division
 - forth: bugfix for negative number parsing (2- was
   interpreted as -20)
 - forth: made 2/ sign-extend negative numbers
 - forth: added spaces source >in 2over 2swap m+ m* sm/rem fm/mod dabs dnegate s>d
 - forth: changed word order of double-cell integers
 - editor: backspace on start of line now doesn't join lines
 - doc: mention in tutorial that default is hexadecimal

Thanks to Christian Johansson and polluks for suggestions!

## [1.4.4] - 2015-11-12

 - forth: added u. unloop leave cell+ cells page key?
 - forth: allow input of negative numbers
 - forth: optimized cmove, cmove>
 - forth: made . print signed numbers
 - forth: various bugfixes
 - doc: simplified & improved intro + tutorial
 - editor: bugfix, exiting editor disabled key repeat

Thanks to Christian Johansson and polluks for bug reports!

## [1.4.3] - 2015-11-01

 - forth: made +loop work like it should
 - forth: removed <=, >=
 - forth: renamed <, > to u<, u>
 - forth: renamed s<, s> to <, >
 - forth: renamed d* to um*
 - forth: optimized loop, r>, >r, r@, i
 - forth: disabled tail call elimination for (
 - editor: faster key delay+repeat
 - editor: hid fg
 - editor: various bugfixes
 - doc: documented min, max, within

## [1.4.2] - 2015-10-30

 - renamed EOL comment # to \ (PC) or £ (C64)
 - renamed :asm => code
 - renamed ;asm => ;code
 - renamed tell => type
 - disabled tail call elimination for s" : ]
 - minor optimizations

## [1.4.1] - 2015-08-13

 - forth: added tail call elimination, i.e., the practice of replacing
   jsr/rts with jmp. words that should not be subject to tail call
   elimination must be tagged with "no-tce".
 - forth: renamed jsr => sys
 - forth: renamed immed => immediate
 - forth: added postpone
 - forth: removed [compile]
 - forth: inline drop, 2drop

## [1.4] - 2015-08-08

 - forth: durexForth is now subroutine threaded instead
   of direct threaded. This gives a huge speed improvement,
   at the cost of increased RAM usage.
 - forth: added "compile," word.
 - disk: improved EOF handling.
 - editor: bugfixed CTRL+u/d.
 - editor: bugfix: "fg" was by mistake hidden.
 - doc: added inline assembly example.
 - doc: bugfixed turtle graphics demo.

## [1.33] - 2015-06-24

 - editor: changed half-page scrolling from U/D to CTRL+u/d
 - editor: many bugfixes and optimizations
 - forth: many small optimizations
 - forth: made cmove> ANS Forth compliant
 - doc: updated memory map documentation

## [1.32] - 2015-03-27

 - switched to dual parameter stack that is split up in MSB/LSB sections.
   this is a nice optimization!
 - optimized "branch"
 - renamed "var" to "variable"
 - added "depth" (gives depth of parameter stack)
 - added "sp0" (bottom address of LSB parameter stack)
 - added "sp1" (bottom address of MSB parameter stack)
 - removed "sp@"
 - bugfixed & optimized "ls"
 - moved zptmp, zptmp2, zptmp3 and ip to new locations

## [1.31] - 2014-04-20

 - changed ' ['] to be according to standard
 - re-add sid.fs, which contains SID manipulation words
 - bugfix . in mml player

## [1.3] - 2014-04-16

 - Music Macro Language (MML) support!
 - Renamed some assembly mnemonics.
    asla, lsra, rora, rola,  =>
    asl,a lsr,a ror,a rol,a
 - Allow strings longer than 256 bytes.
 - Optimized jsr. (Thanks to Kevin Lee Reno!)

## [1.28] - 2014-03-31

 - Improved documentation, describe demos and fix some
   error in tutorial.
 - Bugfixed some demos.

## [1.27] - 2014-03-30

 - "forget" did not handle missing word
 - factor "more" to separate word
 - improve seeing of do..loop

## [1.26] - 2014-03-15

 - Improved line buffering, removed "linebuf"
 - Added assembler labels
 - Added "base", "hex", "decimal"
 - Renamed "exec" => "execute"
 - Added "[char]"
 - Added some support for floating point values (float.fs)
 - Removed "?immed"
 - Made "words" pause when screen is full
 - Added "lshift", "rshift"
 - Added demo how to modify character ROM
 - Optimizations, bugfixes, more documentation

## [1.25] - 2013-07-22

 - math: added um/mod, */, */mod
 - loops: added do, loop, +loop, i, j
 - interpreter: make key up recall previous line entered
 - bugfixed r@
 - dropped "not" word - replaced by 0=
 - rearranged zeropage usage so that parameter stack is a bit bigger
 - documented layout of forth words
 - bugfixed docs: invert, not "not", flips all bits
 - added "bl" word (for space)

## [1.24] - 2013-06-22

 - Added create/does>
 - Removed fg word - just use vi
 - Renamed dec to decimal
 - Make c. add trailing space
 - Split up ." in ." (compile-time) and .( (run-time)
 - Various small improvements

## [1.23] - 2012-11-25

 - Some pretty good speed and size optimizations.
 - Removed rdrop.

## [1.22] - 2012-11-01

 - Bugfixed erase, see
 - Removed 2swap, number
 - Moved editor buffer from 5000 to 6000, giving more space for code
 - Make "fg" fail if editor buffer has been overwritten by code
 - Improved border flashing + cursor blinking during compile
 - Improved error handling further
 - Minor optimizations

## [1.21] - 2012-10-14

 - Handle failed compiles better. Now it will not be necessary to
   reboot computer that often.
 - Editor:
	- After pressing F7 to compile & run, Restore brings you back to editor.
	- "vi" with empty stack now starts editor with empty buffer.
	- Added yy command.
	- Various bugfixes.
 - Bugfix text, ldx,
 - Rename jsr-wrap to jsr
 - Added 100/.
 - Change blkcol to take column, row instead of x, y.
 - Updated documentation.

## [1.2] - 2012-08-07

 - Added hi-res graphics module.
 - Added sinus/cosinus module.
 - Added 2*, 2/, s<, s>, abs, negate, 0<, d*.
 - Assembler: Added :+, :-, +branch
 - Bugfix s" - now it behaves the same in compile and immediate mode.
 - Removed cells, 0>, write, char.
 - Updated documentation.
 - Made "load" load byte-by-byte again.
 - Added hexdec, ls modules by Kevin Reno.
 - Various bugfixes and optimizations.

## [1.1] - 2011-08-08

 - Added experimental sid module.
 - Added hide.
 - Removed land.
 - Bugfixed not, invert, c@.
 - Assembler: Added lbl, -branch, :asmsub, ;asmsub
 - Editor
   - Renamed edit => vi.
   - Renamed 0: => g
   - Speed-ups, bug-fixes

## [1.0] - 2009-02-06

 - Initial version uploaded to CSDB.
