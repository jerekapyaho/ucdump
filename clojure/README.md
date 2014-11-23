# ucdump

Unicode character dump for UTF-8 encoded files

## Installation

Create an uberjar with Leiningen and copy it somewhere suitable for you.

## Usage

Run ucdump from the standalone JAR file
like any JVM application. Specify the name of a UTF-8 encode
file as a command-line argument:

    $ java -jar ucdump-0.1.0-standalone.jar filename

## Options

None.

### Bugs

Does not read from standard input like a proper UNIX-style tool should.

## License

Copyright © 2014 Conifer Productions Oy

Distributed under the [MIT License](http://opensource.org/licenses/MIT).
