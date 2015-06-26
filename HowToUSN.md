# What is USN? #

USN is short for [Update Sequence Number](http://en.wikipedia.org/wiki/USN_Journal) and is a numbering system that makes it easy to track all changes to a disk.

USN has two major benefits in Duplicati.

Firstly, it significantly reduces the time required to list all files on a disk, making backups perform much faster, and with significantly less disk usage.

Secondly, it reduces the number of files to compare, as the OS can simply report what files that were changed since the last backup.

# How can I use USN? #

Since USN allows access to a full list of files on any supported disk, it can potentially reveal confidential information. For this reason, only users with the administrative privilege can use USN. This means that you must run Duplicati with administrative access for the USN support to be active.

USN support can be controlled with the option "--usn-policy", which is set by default to "auto", meaning that if USN is available, it will be used, otherwise Duplicati will silently switch to the normal directory enumeration.

If you want to ensure that USN is activated, a setting of "on" will write a warning in the log if USN could not be activated. Setting "--usn-policy" to "required" will abort the backup if USN fails to activate. Use the setting "off" to disable any attempts to use USN.

# Requirements #

USN is only supported for NTFS volumes and only for Windows. If you have an idea for supporting something USN-like on Linux or Mac, [please file an issue](http://code.google.com/p/duplicati/issues/list).

Besides requiring a NTFS volume, the volume must also have USN journaling activated. On Windows 7, this appears to be on by default, but on Windows XP you must activate it.

If you need to manually change settings related to USN, you can use the [fsutil program](http://www.microsoft.com/resources/documentation/windows/xp/all/proddocs/en-us/fsutil_usn.mspx?mfr=true). To see if USN is activated for a certain volume, issue the command:
```
fsutil usn queryjournal C:
```

If you want to create a journal for your drive, use this command:
```
fsutil usn createjournal m=1000 a=100 C:
```

# Mixed mode #

A mixed mode USN system is supported, which means that if you back up data from two or more volumes, it is not a requirement that all volumes support USN. In a mixed mode scenario, the USN enabled volumes will use USN, where the non-USN enabled volumes will use the normal enumeration system.

# Recreating a journal #

Occasionally the change journal will get filled and needs to be recreated. The OS should do this for you automatically. When this happens, the journal number for the volume changes, and Duplicati will not be able to use the USN numbers to find the changed files, but will still be able to perform a fast file listing. If this happens you will get a warning in the backup log. This is intended as it should be a fairly rare situation.