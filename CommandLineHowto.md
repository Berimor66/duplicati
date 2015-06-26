# Introduction #

This document describes how to use Duplicati from a commandline interface (CLI). You can use the CLI if you want to run a backup triggered by a scheduler, or run a backup when a special event occurs.


This overview only shows the most important options. To view all supported operations and options supported by Duplicati, run the program in a command prompt, without any arguments. You might want to write this output to a file to get a better overview. Under Windows the required command is `Duplicati.CommandLine.exe > readme.txt` to write all instructions to the file readme.txt.

Internally the Duplicati GUI calls the exact same code as the commandline version, so the two versions are always equally stable and capable.

Basically Duplicati CLI distinguishes between
  * Actions like backup, restore, delete, list, ... (some of them are optional)
  * Options like --full-if-older-than
  * Arguments like 2D (2 days) or a target URL
So, the basic syntax for a Duplicati CLI command is
```
Duplicati.CommandLine.exe <action> <options> <arguments>
```

The following sections provide a quick overview:


# Actions #

## Backup ##
```
Duplicati.CommandLine.exe backup <source folder> <target url>
Duplicati.CommandLine.exe backup --full-if-older-than=1M <source folder> <target url>
```

The action `backup` is optional. Duplicati CLI will guess the right action by parsing the arguments and options. Backups are created as incrementals, if a full backup exists. Use the `--full-if-older-than` option to make full backups regularly. Or use the `--full` option to force a full backup.

## Restore ##
```
Duplicati.CommandLine.exe restore <source url> <target folder>
```

The action `restore` is optional. Duplicati CLI will guess the right action by parsing the arguments and options. Restore always restores the most recent version of the file, use the `--restore-time` to restore from a specific time. Use the `--file-to-restore` or the `--filter` to control what files to restore. The `--file-to-restore` can take multiple filenames, if they are seperated with semicolon (or whatever your OS uses as path seperator).

## Delete ##
```
Duplicati.CommandLine.exe delete-all-but-n-full <target url>
Duplicati.CommandLine.exe delete-all-but-n <target url>
Duplicati.CommandLine.exe delete-older-than <target url>
Duplicati.CommandLine.exe cleanup <target url>
```

The delete commands can be used to delete old backup sets, so the backup size does not grow infinitely. You can safely issue the commands, as **no files will be deleted without the `--force` switch**. Please note: Incremental backups require a full backup; so there is a dependency between backup files. Duplicati takes care of that dependency and will not delete any backup that another backup is based on.

The cleanup version will remove partially uploaded files. You can also supply the `--auto-cleanup` switch to have Duplicati remove such leftovers when encountered.


## Show data ##
```
Duplicati.CommandLine.exe list <target url>
Duplicati.CommandLine.exe list-current-files <target url>
Duplicati.CommandLine.exe list-actual-signature-files <target url>
```

The "list" command will list files found at the supplied URL. The "list-current-files" works similar to restore, but only shows what files would be restored.
The "list-actual-signature-files" works similar to restore, but will show what files are contained in a specific backup set, with information about what files and folders are deleted or added.

# Options and Arguments #
## Password and username ##
Most backends support reading the username and password directly from the url, using the format:
```
protocol://username:password@example.com
```

But if the username or password contains special charaters, it can be difficult to enter them into the url. Instead you may use the two commandline options `--ftp-username` and `--ftp-password` to supply them instead. See instructions for each backend in the Duplicati usage screen you get by invoking the program without parameters.

If you are using linux, it is possible for unprivileged users to see the commandline for processes executing in other user accounts. If you supply your password as a commandline parameter, others may be able to see it. To prevent this, you can use the environment variables "FTP\_PASSWORD" and "FTP\_USERNAME" instead. You can also use the environment variables on windows, if you prefer.

## Passphrase ##
Each backup is protected by a passphrase. If you do not wish to encrypt the backups, you can supply the `--no-encryption` option.

A passphrase can be supplied with the commandline option `--passphrase` or the environment variable "PASSPHRASE". If no passphrase was found, you will be prompted for one.

## Dates and times ##
Duplicati implements the same time system as Duplicity, but uses actual calendar calculations rather than the simplified "1 month equals 30 days" rule that Duplicity uses.

Duplicati supports the follwing time specifiers:
  * Y - Year
  * M - Month
  * D - Day
  * W - Week
  * h - Hour
  * m - Minute
  * s - second

If you want to specify that you want a full backup each 14 days, you can use any of the following options:
```
--full-if-older-than=2W
--full-if-older-than=14D
--full-if-older-than=1W7D
--full-if-older-than=3D2D2D1W
```

The last two are a bit obscure, but demonstrates that you can combine the time specifiers as you please. If you just enter a number as a time, it is assumed that it is a time in seconds. You can also enter an absolute time, in your local format, like "01-14-2000" or "01 jan. 2004".

The special time `now` indicates the current time.

## Sizes ##

Whenever a size is required, you can use any of the following suffixes:
  * B - Bytes
  * kB - Kilobytes (1024 bytes)
  * MB - Megabytes (1024 kilobytes)
  * GB - Gigabytes (1024 megabytes)

For speed limits, the size is entered in data pr. second. If you enter `1MB` it indicates 1MB/sec.

# Example #

The following example for the windows command line describes how to make sure that the backups of the last 10 days remain available. A single folder is backed up. The backup is not encrypted and transered to an FTP server. The command line code is
```
"C:\Program Files\Duplicati\duplicati.commandline"
backup 
--full-if-more-than-n-incrementals=10 
--auto-cleanup 
--no-encryption 
"D:\source" 
"ftp://username:password@ftp.example.com/target" 

&& 

"C:\Program Files\Duplicati\duplicati.commandline" 
delete-older-than 10D 
--force 
--no-encryption 
"ftp://username:password@ftp.example.com/target"
```


So what does this do? It will create one full backup. Then it will create up to 10 incremental backups before the next full backup is done. As soon as a backup file is older than 10 days it will be deleted if it is not required anymore (you might need an old full backup to restore a younger incremental backup). When this backup script is executed once a day there will never be more than 2 full backups.