![http://duplicati.googlecode.com/svn/images/Screenshots/Add/Page9.png](http://duplicati.googlecode.com/svn/images/Screenshots/Add/Page9.png)

Each time you make a backup, only some of the files are changed. Duplicati can detect what files (and what part of the files) are changed, and only backup the changed portion. This makes the backup take up much less space and run much faster. This type of backup is called an incremental backup.

Too many incremental backups can make the system slower, because it has to look back into all the partial backups to locate a single file. For this reason there should regularly be created full backups. On [the previous page](AddPage8.md) you can read about the special time syntax used for entering how often a full backup should be performed. A common setting is "Each month" (1M) which is also the default.

Since no storage is unlimited, old data should be cleaned up regularly as well. Duplicati offers two method for doing this, either by counting the number of full backups, or by time. If you select the default of 4 full backups, Duplicati will automatically remove backups older than the last 4 full backups. If you have a full backup each month, you can recover documents from as far back as 4 months.

Instead of counting the number of full backups, you an enter how old data you want to keep. If you want to be able to read data as it was 2 years ago, you can enter "2Y" in the "Never keep backups older than this".

_You can use both clean up methods, but this is not recommended._

When you are done, click the "Next >" button.

[<< Go to previous page](AddPage8.md) - [Go to next page >>](AddPage10.md)
