![http://duplicati.googlecode.com/svn/images/Screenshots/Add/Page10.png](http://duplicati.googlecode.com/svn/images/Screenshots/Add/Page10.png)

On this page you can set various limits Duplicati will obey when making a backup.
The upload and download limits are active when transfering data, except when using the SSH backend.

The backup size limit allows you to limit the total size of a backup set. If the limit is activated,
the backup may not contain all the required data. The next incremental backup will then include this,
provided that the size limit is not exceeded.

The volume size is used to prevent Duplicati from making very large files. If there is a limit
on the maximum file size on the backend, you can set it here.

Note that the current version of Duplicati does not completely obey the size limits, and the files
may exceed the limits given, especially if the files being backed up are large.

If the asynchronous upload checkbox is unchecked, Duplicati will create and upload a volume before
the next one is created. If the chexkbox is checked, Duplicati will create volumes, and upload them
independantly. This can speed up the backup sequence, but is usually only effective if the backups are large.
Checking the flag will require more temporary disk space.

The thread priority option, can be used to force Duplicati to run as a low priority process, which will
make the backups take longer time, but interfere less with other running programs.

When you are done, click the "Next >" button.

[<< Go to previous page](AddPage9.md) - [Go to next page >>](AddPage11.md)
