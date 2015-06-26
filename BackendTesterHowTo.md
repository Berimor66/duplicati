# Introduction #
Duplicati supports a number of backends.

Each backend supports four methods: Get, Put, List, Delete.

To verify that backends work correctly, Duplicati comes with a backend tester application.
This document describes how to use the backend tester.


# Details #
The backend tester only works in a console (aka a terminal or command prompt).

In the terminal, you can get usage information by invoking the tester:
```
Duplicati.CommandLine.BackendTester.exe
```

Each backend has a "protocol" which tells Duplicati what backend to use.
As an example, this would use the SSH backend:
```
Duplicati.CommandLine.BackendTester.exe ssh://username@server/folder
```

You can enter options to speed up the test, and produce more debug output like this:
```
Duplicati.CommandLine.BackendTester.exe ssh://username@server/folder --debug-to-console --max-file-size=1mb
```

The output should explain what the the unittest performs currently.

The test consists of the following steps:
  * List files on the backend, the list must be empty
  * Generate files with random size, name and content
  * Upload files
  * List files, verify that the list is as expected
  * Download files, and verify the content
  * Delete files
  * List files, verify that the list is empty

The test is repeated 5 times with 10 files each time as a default.

Any line that starts with three stars is an error, eg.:
```
*** Hash check failed
```