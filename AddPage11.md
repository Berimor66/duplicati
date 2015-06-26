![http://duplicati.googlecode.com/svn/images/Screenshots/Add/Page11.png](http://duplicati.googlecode.com/svn/images/Screenshots/Add/Page11.png)

Filters in Duplicati control what files are included in a Backup.
As a default, all files are included in the backup, so if there are no
filters, all files are included.

A standard backup created by Duplicati will have an exclusion on the Thumbs.db files
that windows create. These files are not required, and will cause the backups to
grow in size.

Duplicati uses a "first-touch" method for determining if a file should be included.
This means that the first rule that matches a file, includes or excludes the file.
If the first rule is to exclude Thumbs.db, none of the other rules can cause it to be
included.

The filters must match the entire relative path. If the folder to back up is "C:\Folder",
The file "C:\Folder\File.txt" can be matched with "\File.txt" or "`*`\File.txt", but not "File.txt".
If no filters match a filename, the file is included.

All Duplicati filters are regular expressions, but those can be a little hard to learn, so
it is possible to use the format usually known as file-globbing, where the "`*`" character can match
any number of characters (also known as a wildcard), and the "?" matches any single character.
Once a filter is added, it is converted to a regular expression, and cannot currently be converted back.

In the test field you can write a path to test for inclusion or exclusion.

When you are satisfied with the filters, click the "Next >" button.

[<< Go to previous page](AddPage10.md) - [Go to next page >>](AddPage12.md)