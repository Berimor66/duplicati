# Introduction #
This page contains a manually maintained list of commonly asked questions

### Q: Can I backup locked/open files? ###
### A: Yes, but you need version 1.2 or greater. ###
There is [a page here describing the available options](HowToHandleOpenFiles.md).

### Q: What is the right volume size ? ###
### A: It depends on how stable/fast your connection is. ###
Basically a connection failure would require that the volume upload has to resume from the beginning.
If your connection is really stable, you can choose a large volume size.

However, the benefits from using a larger volume size are probably not very large.
It is a tradeof between the number of requests and the amount of data transfered in each request.
If each request/file is expensive (in time or money), you may benefit from larger volumes.
But if you occasionally perform a single file restore, you may find that you need to download
large files just to get that single file, which may be more expensive (in time or money).

### Q: Restore is slow, can it work like RSync and store the newest copy? ###
### A: Unfortunately no. ###
RSync works by keeping the newest copy, and then storing differentials to produce the
previous versions. This is a very nice way of doing it, since the most recent version
is readily available, and older version can easily be discarded. This requires that
special software is installed on the remote server to process incoming data
and modify the files to produce the current versions.

Duplicati works the other way around, storing the initial copy and then
differentials to go from the initial version to the current. This
means that it may be neccesary to process a large number of files
before a file can be restored. The reason for this is that Duplicati works with a
number of different backends, eg. FTP, WEBDAV and S3.  For these protocols, it is not
always possible to install special software to handle the incoming data. Duplicati
is focused on working with as many different backends as possible and this means
that the backends requirements are set as low as possible. Each backend only has to
support LIST, GET, PUT and DELETE. This means that Duplicati cannot modify a file on
the backend, other than using a sequence of GET, modify locally, PUT request. But
downloading a file and uploading it can be quite costly.

There are a number of enhancement requests that will adress the restore speed,
without breaking the limitations set but the simple backend requirements:
  * Compacting incrementals, [issue #4](https://code.google.com/p/duplicati/issues/detail?id=#4)
  * High efficiency full backup, [issue #75](https://code.google.com/p/duplicati/issues/detail?id=#75)

### Q: Can the files be stored outside of zip archives? ###
### A: No. ###
A common request is to have the files mirrored onto the storage, instead of having them in volumes. This is not possible because it would require uploading the entire file after a change, since many of the backends do not allow updating file contents. See also the above question.

Another reason it cannot be done, is that Duplicati is a backup system, which means that you store multiple versions of the files. If you retain the folder structure, you need a server mechanism to ensure that you can access multiple versions of the files, which does not exist on all the supported backends.

Yet another issues, is that you must be sure that the backend allows the same set of features as the local filesystem, i.e. allows the same set of special characters. You also loose all confidentiality, as the filenames and sizes are easily readable.

However, if this is what you want, you can search for _file mirror tool_ or _file sync tool_. Some open source projects that look interesting: [SparkleShare](http://sparkleshare.org/), [Mirall](http://en.opensuse.org/Mirall), [OwnCloud](http://owncloud.org/), [cSync](http://www.csync.org/), [S3 File Sync](https://amazons3filesync.codeplex.com/). There are many commercial solutions as well.

### Q: Can I store multiple backups on the same server? ###
### A: Yes, you should create a folder for each backup. ###

The easiest (and recommended) way of doing this is simply to create a folder for each backup. This way it is easy to see what files belong to what backup, and there are fewer settings to fiddle with in Duplicati.

If, for some reason, you must have all backups in the same folder, you can set the filenames that Duplicati uses. This is done by setting the advanced option "backup-prefix", which defaults to "duplicati".

On the commandline this would be done with --backup-prefix=backup1. In the GUI this would be done by checking the box "Manually override settings" and then changing the value of "backup-prefix".

The reason that this is not recommended is that you must also set this option if you need to restore the files later, which you can avoid if you just store the backups in separate folders.

**Warning:** You cannot store two backups in the same folder with the same backup prefix. If you do it anyway, Duplicati will always perform full backups.

### Q: Can I run Duplicati as a service/daemon? ###
### A: No, not yet, see [issue #9](https://code.google.com/p/duplicati/issues/detail?id=#9) for status ###

Besides waiting for [issue #9](https://code.google.com/p/duplicati/issues/detail?id=#9) to be completed, you can also consider running the commandline version of Duplicati, which you can execute from a service or cron job. Simply run "Duplicati.CommandLine.exe" without any arguments to get a list of support commands and usage instructions. The GUI and the commandline versions use the same underlying code, so all features are present in either version.

### Q: The files appear to be processed in random order? ###
### A: Yes, that is by design ###

Suppose that you run the backup every day, but have a very unstable connection such that Duplicati never manages to back up more than a single volume before the network acts up. If the files are processed in a specific order, your backup will always contain changed files from the start of the list (i.e. files and folders starting with A) but never have any files from the end of the list (i.e. files and folders starting with Z). By processing the files in a random order, you are more likely to have a recent copy of your files, regardless of the file name.

This has the disadvantage that if you want to restore a single folder, the files are spread out over many volumes, and each volume with such a file needs to be downloaded. For this reason, many users prefer to set the option "sorted-filelist", which will process files in alphabetical order.

On the commandline this would be done with --sorted-filelist. In the GUI this would be done by checking the box "Manually override settings" and then changing the value of "sorted-filelist" to "true".