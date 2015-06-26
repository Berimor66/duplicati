## Introduction ##

This page contains advanced instructions on how to recover data
from Duplicati backups without using Duplicati.

It is intended to help explain how Duplicati works.
It is NOT intended for everyday use.
You should never have to follow these steps, but you can do so,
if you want to verify that Duplicati works,
and that you can recover your data, even
if Duplicati is somehow broken.

If you are just looking for some way to automate Duplicati,
without using the GUI, please [use the Command line version](CommandLineHowto.md).

## Step 1 - Recover the files ##

Before you can start a manual restore, you need to have the Duplicati
content files on your local disk. You do not need the manifest and
signature files, they are only used to create incremental backups.

Depending on what backend you use, you need some program to get it.
For backends like SSH or FTP, the [FileZilla](http://filezilla-project.org/) program works great.

## Step 2 - Decrypt the files ##

If you do not use encryption, simply skip this section.

If you use GPG, you need to install GPG on your machine.
When GPG is installed, use this command to decrypt each file:
```
gpg --armor --output <outputfile> --decrypt <inputfile>
```

If you use the AES encryption method (default), use this command to decrypt each file:
```
Duplicati.CommandLine.Decrypter.exe <passphrase> <inputfile> <outputfile>
```
The CommandLine.Decrypter.exe is avalible in the binaries as well as in the MSI.

In Duplicati 1.2, the encryption is done with [AESCrypt](http://www.aescrypt.com/) compatible encryption, so you may use their program instead.

## Step 3 - Unzip the files ##

Use your favorite zip program to unzip the decrypted files.
All files with identical timestamps are considered a "set", and must be extracted to the same folder.

The first set is a full backup, all others are incremental.
The incrementals must be applied in the order they were created.

It is easiest to keep track of the incrementals if you name the full backup set "0" and then give each incremental set an increasing number.

## Step 4 - Patching, etc ##

Inside the backup set "0" from the above step, there is a folder named "snapshot". This folder contains all the files as they were when the backup set was created. The paths inside the "snapshot" folder corresponds to the location of the files, relative to the backup source folder.

Inside the other backup sets, there is also a folder called "diff". This folder contains delta files that must be applied using [RDiff](http://linux.die.net/man/1/rdiff). The paths inside the "diff" folder also corresponds to the location of the files, relative to the backup source folder.

An incremental set may also have a "snapshot" folder, that contains files that were added after the original backup set.

There are also a number of other files that keep track of deleted files and folders, which you can ignore.

To apply a delta file, you must issue this command on each file:
```
rdiff patch <inputfile> <deltafile> <outputfile>
```

Repeat this procedure with all incremental sets, where the file is present in the "diff" folder.

When you apply a delta, you are basically "updating" the file from the previous version to the next. You can stop applying delta files at any time, if you want a specific version of the file.

If you cannot find a suitable rdiff program for your platform, Duplicati 1.2 contains the file "Duplicati.Library.SharpRSync.exe" which you can use instead of the official rdiff program.

## Duplicati 1.2 extra ##

To support a fixed volume size, Duplicati 1.2 adds support for fragmented files, meaning that a file can span multiple volumes. The support for this is not implemented as the traditional multi-volume zip files, as that would require that the entire set of volumes are downloaded before a restore can begin.

Instead, Duplicati writes a control file called "incomplete\_file.txt" which contains the name of the file in this volume that is not yet completed. The structure of this file is four lines: filename, offset, length, filesize. The "filename" indicates the file that is not a full file. The "offset" is the offset into the file where writing starts. The "length" is the size of data contained in this volume, and finally the "filesize" is the target size of the file.

When a file is completed, a control file called "completed\_file.txt" is written in the volume, containing the same four lines as found in "incomplete\_file.txt".

Since at most one file can be partial, there can only be one "incomplete\_file.txt" in an archive. But, as a partial file can be completed and a new one be partial, there are two files that control the splitting of a file.

Duplicati 1.2 also controls the file timestamps, so the file timestamp in the volume is the time when the file was last modified, but recorded in the UTC timezone.

## Closing remarks ##

Thats all there is to it!
I recommend that you only test the procedure to ease your mind, as it is non-trivial due to the amount of files present in a backup.

You can create batch files to automate the process, but you would basically produce a simple Duplicati.