# Duplicity on Windows #

I have used Duplicity on Linux for some time, and found it to be a
fantastic program. I was sad to see that it did not run on Windows,
so I could recommend it Windows users.

So, for a while, I have been working on porting Duplicity to Windows.
The initial attempts were successfull, and there is now a Duplicity
version that runs on Windows.

Unfortunately the backend implementations are not as portable as I had hoped.
The SSH backend depends on the pexpect library, which is not easily portable.
In Linux it is possible to create a memory pipe, then call fork(), and the
two processes can communicate over the pipe. It is also possible to pass on
that handle to an external process, and using this system, it is easy to
perform interproccess communication.

Unfortunately, on Windows, file handles are not shared between processes,
and there is no fork() call avalible. This makes it a non-trivial process to
rewrite pexpect and other calls in duplicity.

On top of this, Duplicity uses the TAR file format, which is an old format
intended to be used for tape backups, and it is very tied to the way the
UNIX filesystem works (it uses one owner user and group pr. file), which
is not usefull on Windows.

This made me reconsider my initial efforts. I then started to look at what
the attractive features of Duplicity is, and came up with this:
  * No server requirements except GET, PUT, LIST and DELETE
  * Very simple backends (Due to the above)
  * Open Source
  * Using standard formats

The things I dislike about Duplicity can be summed like this:
  * It's Python (I dislike the lack of static type checking)
  * It uses heavy interprocess communication (which is not great for Windows)
  * Very tied to UNIX

Then there are some stuff that I would like to see in Duplicity:
  * Partial backups, in case the net breaks during backups
  * Suspend an active backup, usefull for laptop backups
  * A GUI to monitor and control Duplicity

What I attempted to do with Duplicati was to use Duplicity "as-is" and put a GUI on top.
However, I wanted the GUI to be avalible in both Windows and Linux. This proved
troublesome, due to the interprocess communication problems mentioned. While I was
building the GUI, I realised that it would be extremely usefull to support partial
backups and supspension of backups when the machine is connected to the internet on
an unstable line, or must be used for other work.

I do not have enough Python experience to figure out how to support partial backups
and suspension, so I decided to look at how it all works.

## How Duplicity works ##

The core workhorse in Duplicity is the RSync algorithm. It is used through the librsync
module that compiles for both Windows and Linux. RSync only works on a single file at a
time. Initially you create a "signature" file, that describes the content of the file.
Then you store both the signature and the file. The trick is that using only the small
signature file, and the new file, RSync can create a diff (aka patch, or delta) of the files.
This is the key to efficient backups, because you can store only the changed part,
and you do not have to keep a copy of the original file on the machine.

As mentioned, RSync does not work on folders, only on files. A program called rsync-backup works on folders.
I suspect that Duplicity lends its algorithm from this program. Prior to starting a backup, an
empty folder is created. In this folder, three subfolders are created, called: signature, delta and base.
For each file backed up, there will be an entry in signature, and either one in base or delta.
If the temporary folder is "/tmp/backup" and the file is "/usr/bin/program", the signature will
be named "/tmp/backup/signature/usr/bin/program".  This way the folder structure is preserved.

Once all files are backup up, two archives are created, one with the signatures, and one with the
base and delta files. For an incremental backup, the signatures are downloaded, and compared with
the files, and a set of backup files are generated. This can go on forever.

For each backup set, a manifest file is created, containing a signed hash of the archives.

Once a restore is requested, the initial backup with the contents is downloaded. Then the files are
restored (there can be no delta's in the original backup). Then the delta's are applied in turn, the
oldest first. It seems that Duplicity (and rsync-backup?) does not delete folders. The file names
are used to indicate which backups are the oldest. In the filename is also encoded backup type
(incremental or full) and type (signatures, content or manifest).

Duplicity uses the TAR file format to build the packages, the BZip2 compression method, and
GnuPG for encryption of the compressed tarball.

## Building it all with Windows support in mind ##

After giving up on porting the SSH backend to windows, I started implementing the entire Duplicity
system in .Net managed code. I can see that Duplicity is usefull for server implementations, because it
is a console based application. I decided to make a very simple commandline frontend, that calls the library.
This way it is possible to have the benefits from a commandline application, as well as the posibility for
tighter integration with the GUI, without relying on communication with an external program.

Just like Duplicity, the application should be easy to extend with new backends. I have designed a
very simple interface, and a very simle algorithm that allows loading .Net backends if the dll's
are placed in a folder.

### RSync in .Net: SharpRSync ###

Since the core item in Duplicity is the RSync algorithm, I decided to start with that one.

There exists libraries for both Windows and Linux that enable RSync operations. It turned out that
the libraries was target towards C/C++ implementers, and not very nice to use via. P-Invoke from .Net.
After reading about the implementation I started building a pure C# version of the library.
While doing so, I managed to discover a small optimization that will reduce the size of delta
files. The optimization is simply a test that determines if the next block in the stream matches the
hash of the next block. This situation will occur often in files that contain repetitive data.
Without this optimization there will be more commands embedded in the delta stream.

I figured SharpRSync might be usefull in other projects, so I made it LGPL and a standalone dll.
The output from SharpRSync is fully compatible with RDiff, and vice versa.

### Handling folders ###
As mentioned, the RSync algorithm only deals with files, not folders. I built a module
called RSyncDir, that does just that. It works by storing three files in the root of the
archive: "deleted\_files.txt", "deleted\_folders.txt" and "added\_folders.txt". Each of
the files contains a number of lines that specify the path to a file or folder.

Using this simple scheme, it is possible to handle deleted files and folders. The
"added\_folders.txt" file, makes sure that empty folders are restored as well.

Once the RSyncDir implementation is in place, it is possible to create backups.

### Compressing the content ###
Duplicity is a unix program, and thus uses the standard tar/bz2 combination.
It works by using the TAR format as a container for files, and then applying compression
to the tarball.

The TAR format is originally designed for storing backups on magnetic tapes. Magnetic tapes
usually have very slow seeking features, so the format has no index. It is only possible to
find a specific file by reading data from the tape, until the file data appears.

This is a perfectly logical format for a tape based backup, and for packages that are
usually fully decompressed. The TAR format is very old, and has many versions, which
are semi-detectable. It also contains special fields to store the GID/UID for the file.
On windows, this is completely useless, as windows rely on Discrete Access Control Lists.
So, using the TAR format is a logical choice for unix, but a poor choice for windows.

I decided to change the compression engine to use Zip files instead, as they are the standard
on windows. Zip files also have an index, so files can easily be located and extracted on
demand. This is the major issue that makes Duplicati incompatible with Duplicity.

### Encryption ###
Duplicity uses GnuPG, which is avalible for both windows and unix. I like the GnuPG project,
and I have implemented the posibility to use GnuPG as the encryption method. Unfortunately,
GnuPG is not easily to integrate with, and must rely on commandline communication. The even
exists a GnuPG wrapper for Python, because it is difficult to use. The more standardized way
of encrypting data in the .Net framework, is by using the built-in encryption algorithms.
I chose the AES (Rijndael) algortihm as the default encryption mode. Using AES does not affect
the length of data (except for the last block encrypted), where GnuPG seems to format the output
for sending by email, meaning that it is base64 encoded with linefeeds for each 80 characters,
resulting in files that are significantly larger than the original files.

To make sure that you can test the decryption, and verify that the backed up files are avalible
as you would expect, I have made a standalone decryption tool that uses the same code.

This deviates a little from the objective of using only standards, but the AES algorithm is very
standard, so the deviation is minor.

### Backends ###
When I first hit the problem with porting the SSH backend, I thought about making an all-purpose
backend, that would register as all the backends, and then keep implement only the backend code
in .Net. I did accomplish some of this, and the result is included in Duplicati.

The main design priciples are kept, and the requirements are that the backend supports, LIST, GET,
PUT and DELETE.

I have currently implemented backends for SMB destinations (windows shared folders), FTP, SSH and S3.

### Optimizations ###
After having achieved a working version of Duplicati, I started optimizing away. One of the
optimizations that I managed to implement, is to reduce the amount of temporary files and folders
used. The primary source of this optimization, is the ability to view a Zip file as if it was
a folder on disk. This is not really possible for TAR volumes, due to the missing indexes.
This optimization means that it is possible that some files are never extracted from the archives,
and for most files (such as signatures) they can be kept entirely in memory.

### Going beyond Duplicity ###
Besides the optimizations I have made for Duplicati, I have also added a few unique things already.
One of them is the possibility to visually display and select files to restore.
Another is the ability to use logins for windows shared folders.
I have also added an option to control the thread priority as well as the bandwidth used by the backends.
The Duplicati engine is also capable of stopping the backup after writing a volume to the backend,
and then pick up where it left the next time.

### The user experience ###
My first encounter with Duplicity was a bit confusing, costing of some trial-and-error. While I am
pretty skilled as such stuff, I found it easy enough, but I also know that most people I know, would
never guess where to start, and would certainly never finish. Duplicati aims to remedy this problem
by providing the user with a very intuitive user interface, based on a wizard guide where the entire
process is broken into very small subtasks. I have tried to keep the user interface to an absolute
minimum pr. default, so the inexperienced user will have as little trouble as possible getting started.

### Summing up ###
This document has summed up my reasons for not wanting to continue on the original path, of cretating a
Duplicity GUI, but rather re-implement the idea. There is nothing that prevents the implementation from
supporting TAR files, and thus becoming compatible with the original Duplicity client. I have decided
not to do so, because I don't expect many people to switch from Duplicity anyway.

If you want to continue along the original idea, the SVN repository still contains the 0.5 alpha release,
which is based on a Duplicity with a few windows attachments.