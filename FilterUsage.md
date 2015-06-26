# Introduction #

Duplicati supports using filters to provide fine grained control over which files are included and which files are not.

This page describes some of the more non-obvious parts of how filters work.

# File globbing vs. regular expressions #
Internally Duplicati only uses [regular expressions](http://en.wikipedia.org/wiki/Regular_expression) to match file and folder names.

You can supply filters in the simpler form, known as file globbing or wildcard matching.
An example of file globbing / wildcard matching:
```
*.txt
Log_no_?.txt
```
This document is not intended to be a guide into regular expressions, but this example shows the above filters with regular expressions:
```
.*\.txt
Log_no_.\.txt
```
If the two were equivalent, there would be no need to use the more complicated regular expressions, so here is an example of something that cannot be expressed using wildcards:
```
Log_no_[0-9]+\.txt
[(first)|(last)]\-log\.txt
```

# Absolute vs. relative paths #
Duplicati 1.0 only supports a single source folder, so it is quite simple to obtain the full path given a relative one, and vice versa. However in newer versions of Duplicati, multiple source folders are supported, so this mapping is no longer simple.

In Duplicati 1.0, only relative paths are supported. To support the previous versions, relative folders are still supported, but now absolute paths are also supported.

# Directories #
Internally Duplicati appends the directory separator character ("\" or "/") to directories to be able to distinguish them from files.

When creating a filter, this can be used to match a specific folder, such as (wildcard type):

Windows:

`D:\Downloads\`

Linux

`/Downloads/`

Note that if the trailing slash is missing, the filter will not match the folder, but rather a file with the same name.

# Filter order #
Duplicati uses the "first match" method when evaluating filters. This means that if a file or directory matches a filter, that filter determines if the item is included or excluded. No subsequent filters are evaluated.
Commandline Example:

`--include=*.txt --exclude=*\Thumbs.db --include=*`

GUI example:

![http://duplicati.googlecode.com/svn/images/WikiImages/filters/basic.png](http://duplicati.googlecode.com/svn/images/WikiImages/filters/basic.png)

In the above example all files named Thumbs.db will be excluded even though there is a "catch all" filter that matches all files. The last filter can safely be omitted, as Duplicati automatically includes all files that are not specifically excluded.

When filtering a directory, be aware that the directory itself is processed _before_ any files in it. This means that if you exclude a folder, no files in that folder can be included, even if the filter to include them is placed before the folder exclusion filter.
Example filesystem contents:
```
D:\Downloads\Myfile.txt
```
Example filter setup:
```
Include: *.txt
Exclude: D:\Downloads\
```

Even though the filter states that the txt file should be included, it is **not**. When looking for files, Duplicati first discovers the folder, and then determines to exclude the folder. This means that the file is never evaluated.

The reason for this is performance. If Duplicati had to run through all excluded folders it would take much longer to complete. Imagine for example that the user has chosen to backup the C drive and excludes the folder "C:\Windows", on an average system, it could take a few minutes to search that folder and its subfolders. On linux this would be equvalent to backing up the root, /, and then excluding /var, /usr, /opt, which would also take quite a while to run through.

# Include some files exclude everything else #
A common setup is to include all files of a certain type, and then exclude everything else. The initial attempt is usually to add a rule for including the desired files, e.g. `*.cpp`, and then add a rule to exclude everything else, i.e. `*`.

This does not work because the exclude `*` rule also matches all folders, meaning that all folders are excluded and thus never checked for matching files. To get this working, you need a special regular expression that matches every file instead of every folder.

A regular expression for Windows that matches all files:
```
.*[^\\]
```

and on Linux/OSX it is:
```
.*[^/]
```

![http://duplicati.googlecode.com/svn/images/WikiImages/filters/exclude-all-files.png](http://duplicati.googlecode.com/svn/images/WikiImages/filters/exclude-all-files.png)

If you are using a filter like this, your backup usually contains a lot of empty folders, which are all the folders that did not have any of the desired files. To avoid this, you can set the advanced option "exclude-empty-folders" to "true":

![http://duplicati.googlecode.com/svn/images/WikiImages/filters/exclude-empty-folders.png](http://duplicati.googlecode.com/svn/images/WikiImages/filters/exclude-empty-folders.png)

You can edit the advanced settings by selecting the options in the wizards:

![http://duplicati.googlecode.com/svn/images/WikiImages/filters/advanced-setup.png](http://duplicati.googlecode.com/svn/images/WikiImages/filters/advanced-setup.png)