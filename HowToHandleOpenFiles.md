# The open file problem #

Making a backup of a file that is opened by another process is a classic problem. Essentially the problem is that the process reading the file (the backup program) cannot communicate with the program that is writing the file. This means that there is a chance that the file is being written at the exact same time as the backup occurs. In this scenario, the backup copy will contain a partially updated file.

If the writer is a database system or similarly advanced program, it can deal with partially written files, because that is the same problem that occurs if the machine is abruptly powered off while the file is being written.

If the writer is a text editor, the file will likely contain parts of the original version and parts of the updated version. While text files may be partly recoverable by manual editing, it is highly likely that some data will be missing. If the file is binary, it is likely very hard, if not impossible, to recover any meaningful data.

# The snapshot solution #

One common solution for backing up files that are in use, is to use a snapshot of the disk. This is usually implemented as a [Copy-on-write](http://en.wikipedia.org/wiki/Copy-on-write) solution, and makes it possible for the backup program to read the contents of the files as they looked at the time of the backup. This does not entirely solve the problem of partial files, as the write may have occurred when the snapshot was created.

One of the reasons why snapshots are commonly used, is that they offer semantics similar to an unclean power of. That means that the contents of the snapshot is similar to what one could recover from the disk, if the machine was abruptly powered down. Since databases commonly support this scenario, it can at least guarantee that this usage works as expected.

Another reason is that snapshots also guarantee that related files are consistently updated. For instance a program that has a data file and an index file will not be backed up in a state where the files are more out of sync, than what an unclean power off can produce.

Finally, snapshots also handle all scenarios related to file locking, as the file being read is something similar to a copy of the file. This means that all files can be backed up, regardless of the exclusive state of the original file. And it also means that the snapshot does not interfere with any running programs, as it essentially opens a different file.

# The read open file solution #

Unfortunately the snapshot solution requires operating system support, as well as administrative privileges. As the snapshot is lock-free, a process could potentially read confidential data from the snapshot, so there is no workaround for allowing non-privileged users access to a snapshot.

Many files are simply opened with a locking semantic that states that other programs may read the data while the file is opened. In this case, it is possible for a backup program to read the file contents without having special privileges.

This obviously requires that the writer program has not requested exclusive access to the file, meaning that this will only work for some files.

This method seems to be the same as the snapshot approach, but it has a further weakness, in that the file may be written backwards. It could be that there is a control structure at the end of the file, which is written first, and then the actual entry in the beginning of the file is written. In this case, reading the open file from beginning to end, will not guarantee that the copy is equivalent to an unclean shutdown. It is also possible that multiple related files are updated in a manner that is not detected by the backup program.

# What does Duplicati do? #

Version 1.0 of Duplicati does not support reading any type of locked file. Version 1.2 adds the options "--snapshot-policy" and "--open-file-policy" to control the use of either version.

## Snapshot usage ##

The "--snapshot-policy" is considered most important, so if a snapshot is created, the "--open-file-policy" setting is ignored.

The default setting for "--snapshot-policy" is "off", meaning that no snapshot is created. This is done because a snapshot requires both administrative privileges as well as operating system support.

The "--snapshot-policy" also supports being set to "on" which will attempt to activate the snapshot, and print a log warning if it fails. The "auto" option does the same but does not print any warnings. Finally it can be set to "required" which will abort the backup if the snapshot could not be created.

## Open file usage ##

If the snapshot is not activated, and the setting is not "required", the "--open-file-policy" setting will be used to determine how to handle open files.

The default setting for "--open-file-policy" is "snapshot", which will read the file as-is. For most situations, this will produce an acceptable copy of the file, but as noted above, there are situations where this will not be the case. To aid in detecting some of those situations, a warning will be written in the log, if the file changed during the backup procedure.

The "--open-file-policy" setting can also be set to "copy", which will make a copy of the file before performing the backup. This is implemented in such a way that the file is read twice, ensuring that the file has not changed within a limited time frame, which also provides some protecting against partial updates. The drawback from this method is that it requires a full file copy, which can be a problem with large files.

Finally the setting "ignore" will revert to the Duplicati 1.0 setting of simply ignoring files that are locked.

# What is recommended? #

If you can use snapshots, I would recommend that you use snapshots. If your data is primarily in the form of documents, it is unlikely that you will experience problems, even if you do not use snapshots.

The default setting for "--open-file-policy" in Duplicati 1.2, means that you will get a potentially broken copy, as opposed to not getting a copy at all. If this is a concern for you, I would recommend setting the "--open-file-policy" to "copy".

# Operating System specific notes #

As the snapshots are OS dependent there are a few things to note about each OS implementation.

## Mac support for snapshots ##

Currently there is no support for snapshots on Mac. If you have an idea for supporting this, [please file an issue](http://code.google.com/p/duplicati/issues/list).

## Linux support for snapshots ##

Duplicati supports snapshots if the system is using the [Logical Volume Manager](http://en.wikipedia.org/wiki/Logical_Volume_Manager_(Linux)) system. Unfortunately the default Ubuntu installer does not support LVM, but the "alternative" version does. The use of LVM snapshots is implemented through some scripts that are included in the Duplicati package. The scripts should work right away, but requires that you make them executable, eg.:
```
chmod +x *.sh
```

If you are using LVM, you must also ensure that some [extra space is available for the snapshot](http://tldp.org/HOWTO/LVM-HOWTO/snapshots_backup.html).

If your system is non-standard, you may have to edit the script files to better fit your system. If you have ideas for improving LVM snapshot support, be sure to [file an issue](http://code.google.com/p/duplicati/issues/list).

## Windows support for snapshots ##

Windows support for snapshots is implemented using [Volume Snapshot Service](http://en.wikipedia.org/wiki/Shadow_Copy). The VSS implementation requires that you install the [Microsoft Visual C++ 2008 SP1 Redistributable Package](http://www.microsoft.com/downloads/en/details.aspx?familyid=a5c84275-3b97-4ab7-a40d-3802b2af5fc2&displaylang=en). If you use a 64bit version of Windows, use [this link](http://www.microsoft.com/downloads/en/details.aspx?familyid=ba9257ca-337f-4b40-8c14-157cfdffee4e&displaylang=en).

The VSS implementation is slightly more advanced than a simple snapshot. It also supports a notification system, through which any program can register itself as a writer. When a snapshot is requested, VSS will notify each registered writer. This allows any program to flush data to disk before creating a snapshot, ensuring that the snapshot is consistent, an improvement over the regular unclean shutdown promise. Only a limited set of applications are registered as VSS writers, meaning that many programs do not use this feature.

Unfortunately this extra service sometimes breaks, because the writers fail to respond properly to the snapshot request. As any writer can prevent the snapshot from being created, Duplicati also supports excluding particular writers. You can get a list of registered writers and their respective GUIDs by issuing the following command in a command prompt with administrative rights:
```
vssadmin list writers
```

If an error occurs when creating the snapshot, the eventlog will usually report the offender. Use the option "--vss-exclude-writers" to exclude the offending writers:
```
--vss-exclude-writers="{d61d61c8-d73a-4eee-8cdd-f6f9786b7124};{0bada1de-01a9-4625-8278-69e735f39dd2}"
```