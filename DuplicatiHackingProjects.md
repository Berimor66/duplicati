# Introduction #

This page is meant for developers who wish to participate in Duplicati development, but are unsure of how to get started. Attempting to comprehend Duplicati in its entirety is fairly difficult, and will likely discourage most developers.

On this page there is a list of issues that have limited scope, meaning that they only affect a very small part of Duplicati and hence only requires understanding a small part of Duplicati.

If you wish to participate in Duplicati development, pick an issue below and start hacking away. You should pick an issue that interests you, preferable one that you would use if it was implemented.

# General notes #
For each issue, there is a link to the original issue. You should read the description of the issue, as there are usually some user comments on what the problem is.

Before starting, you should have skimmed the [HowToBuild](HowToBuild.md) page and the [DeveloperGuide](DeveloperGuide.md).

The issues below are sorted somewhat by perceived complexity, starting with the least complex ones.

For each issue, there is a short description of the task, and some notes on my ideas for fixing the issue.

Since I did not actually fix the issues, there is a possibility that I have missed something that makes the task several orders of magnitude more complex.

# [Issue #331](https://code.google.com/p/duplicati/issues/detail?id=#331) Restore setup keeps the local signature path and temporary folder settings #
The "restore setup" feature merely restores the users settings database, and replaces the current database with it. This causes some problems if the new machine is not identical to the old one, as the database contains some paths. This should be fixable by only modifying the file "FinishedRestoreSetup.cs" and only the function "Restore". If the paths already exist and are accesible by the current user, they should be preserved, otherwise they should be set to "null", which would cause them to be set to the current defaults.

# [Issue #339](https://code.google.com/p/duplicati/issues/detail?id=#339) Restore operation is shown in the status as backup #
This should be possible to fix by only changing the file "ServiceStatus.cs" and only the function "BuildRecent". The data for this is already available as "string action = l.Action;", where action can be either "Backup" or "Restore". You will need to create a new icon/png file for displaying the restore operation. If you want it to be a bit more fancy, you can also create an icon for "Restored with warnings".

# [Issue #70](https://code.google.com/p/duplicati/issues/detail?id=#70) Portable app version #
Duplicati already supports running in portable mode, by simply starting Duplicati with the commandline option --portable-mode. To solve this task, you would need to examine the Portable Apps packaging system, and then produce a batch/python script that re-packages a zip file as a portable app, including a menu item that starts Duplicati with the --portable-mode option set.

# [Issue #278](https://code.google.com/p/duplicati/issues/detail?id=#278) manage db-password from GUI #
If there are not permission issues involved, this should be as easy as editing "ApplicationSetup.cs" and adding a database password field. When saving the dialog, the database should be saved with the new password by something similar to "DbCon.ChangePassword(...)" and the environment variable should be changed by "Environment.SetEnvironmentVariable(Program.DB\_KEY\_ENV\_NAME, ...newpassword...);"

If there are permission issues with setting the enviroment variable, this needs to be spawned as a separate process, which will then invoke the UAC/sudo and ask permission BEFORE actually changing the db password.

# [Issue #290](https://code.google.com/p/duplicati/issues/detail?id=#290) Proxy support #
There is a module called "http-options", which would be ideal for implementing this. Simply add a few options to the module that allows the user to configure the proxy. There should be a minimum of url, port, username, password.

It should then be possible to modify the function "Configure" in HttpOptions.cs to do something like:
```
if (m_useProxy) 
{
	m_oldproxy = System.Net.WebRequest.DefaultWebProxy;
	System.Net.WebRequest.DefaultWebProxy = new System.Net.WebProxy(....url, port, username, password ...);
}
```
And then simply restoring it in the Dispose callback:
```
if (m_useProxy)
	System.Net.WebRequest.DefaultWebProxy = m_oldproxy;
```

Implementing the options in the "SupportedCommands" property will make the properties appear in the advanced settings grid.

If you wish to add a user interface, you can implement the IGUIControl, but this will only allow application wide setting of the proxy. There is currently no easy way to allow a control like this being shown in the wizard.

But it is possible to let the UI define different proxy settings based on, say hostname, so that the user can configure proxy settings pr. hostname, and thus be able to enable proxies selectively.

# [Issue #373](https://code.google.com/p/duplicati/issues/detail?id=#373) Store log at backend #
Duplicati logs a lot of information using the Logging module. To capture this data to a special file, modify the static "Log" class to support multiple log destinations, and register a new "StreamLog" module on start of the backup operation in "Interface.cs". After the backup has completed, unregister the "StreamLog" and issue a "backend.Put(...)" request, using a ".log" file extension.

# [Issue #380](https://code.google.com/p/duplicati/issues/detail?id=#380) Report orphan origin #
When a number of orphans are found, Duplicati should try to figure out what files are missing. E.g. if the file "duplicati-inc-signature.20110103140000Z.vol5.zip.aes" is found, it is orphan because the file "duplicati-inc-manifestA/B.20110103140000Z.manifest.aes" does not exist. But if that file exists, it is because a previous manifest file does not exist. In this case, load the manifest file and determine what the previous manifest file was, and repeat until a file is missing.

The sum of this should then have duplicates removed, and the list of files that are identified as being the missing files should be output as a warning by issuing a "stat.LogWarning(...message...);"

# [Issue #342](https://code.google.com/p/duplicati/issues/detail?id=#342) Restore files to source location #
This should be possible to do by editing the file "TargetFolder.cs" and adding a radio button that selects "to source folder". When the user selects the radio button, the list of source folders shoud be retrieved by a call to "ListSourceFolders", and the results should replace the target folders, by setting "backupFileList.TargetFolders = sourcefolders;".

# [Issue #236](https://code.google.com/p/duplicati/issues/detail?id=#236) Get commandline from GUI #
The main issue with this is "where to put it" in the UI. My suggestion is to add a button to the "FinishedAdd.cs" dialog. This button would then start a new backuptask through the "DuplicatiRunner", but stop right before the backup actually starts, and then return a commandline string based on the settings found.

# [Issue #222](https://code.google.com/p/duplicati/issues/detail?id=#222) Add profile settings #
This could be implemented by an simple xml file that is read by a new class based on "IGenericModule". It should probably first probe the system-wide configuration file and load the settings from that. After this, it should load the user/instance specific settings, and finally read any configuration file supplied as an option.

By default, this generic module should only be loaded by the commandline and not the GUI.

# [Issue #358](https://code.google.com/p/duplicati/issues/detail?id=#358) detect unexpected shutdown #
To accomplish this, the schedule should be marked as "running" in the database when starting the backup. On startup, all schedules in the database should be checked for the "running" flag, and a log entry should be created for all "running" entries, and the "running" flags should be deaktivated.

This involves changing the database schema and add the extra field in the database. Changes like this are simple to implement by adding an "upgrade script" that simply describes what to change in an existing database. To ensure that new installations also have this, the default database schema must also be changed so the upgrade and clean install have the same database layout. Have a look at the existing upgrades and the full schema in the folder "Database schema". You should also read the comments in beginning of the file "DatabaseUpgrader.cs".

To set the flag, look in the file "DuplicatiRunner.cs" and determine an appropriate place for setting the flag and forcing a "commit" on the schedule. After running the task, simply clear the flag and "commit" again. Look in "Program.cs" for an appropriate place to place the detection of non-completed tasks.

# [Issue #336](https://code.google.com/p/duplicati/issues/detail?id=#336) Do not run backup after restore #
The main idea behind this request is that it should be possible to flag a schedule/task as "inactive". When restoring the setup (see [issue #331](https://code.google.com/p/duplicati/issues/detail?id=#331)), all backups should be marked as "inactive". This involves changing the database schema and add the extra field in the database. Changes like this are simple to implement by adding an "upgrade script" that simply describes what to change in an existing database. To ensure that new installations also have this, the default database schema must also be changed so the upgrade and clean install have the same database layout. Have a look at the existing upgrades and the full schema in the folder "Database schema". You should also read the comments in beginning of the file "DatabaseUpgrader.cs".

Once the field is implemented it is easy to modify the flag in the same way as described above for [issue #331](https://code.google.com/p/duplicati/issues/detail?id=#331). Next step would be to modify the file "Scheduler.cs" and the function "Runner" to ensure that "inactive" items are not scheduled.

Since this has some implications for the user, the file "BackupTreeView.cs" should be modified to show items with the "inactive" flag set as "grey" items (using the standard Duplicati inactive icon).

Finally the user should be able to toggle the "inactive" flag. My suggestion is that the user can do so after editing the backup, on the summary page (FinishedAdd.cs), where there is already a "run backup now" checkbox, but you may have a better suggestion.

# [Issue #231](https://code.google.com/p/duplicati/issues/detail?id=#231) Store real filters along with regexps #
This involves adding an extra field to the database, see above for a description of how to do this. The idea is that the database stores both the original string entered by the user, as well as the regular expression version. This allows all logic to be unchanged because it can use the regular expressions.

When the user edits the filters, the original string should be displayed. After editing the resulting regular expression should be written into the database. Apart from the database modifications, it should be possible to implement simply by editing "FilterDialog.cs" and "FilterEditor.cs".

To allow the UI to differentiate between regexps and globbing expressions, I would propose that regexps are written into the current field in the database, and also sets the new field to null. If the user enters a globbing expression, the new field would not be null. This allows the UI to differentiate between a regexp and a globbing expression, and by simply settings the new field to null on existing entries it should run smoothly, even for upgrades.

# [Issue #335](https://code.google.com/p/duplicati/issues/detail?id=#335) Import/export option #
This issue states taht it would be desirable for the user to select a backup and export it as xml, and then import it on another machine. This would allow the user to import the setup on another machine for easy restore, without having to remember the details (host, password, port, folder, etc.). If [issue #332](https://code.google.com/p/duplicati/issues/detail?id=#332) is implemented first, the imported job should be marked "inactive" after an import.

I am not decided on what the best approach for placing this option in the UI is. Perhaps a right-click menu on the backup list, but that seems somewhat counter intuitive that the user has to select something bogus on the first wizard page, just to get the list.

I think the best approach for exporting the data might be to create a copy of the database, delete everything that is not related to the current backup, and then serialize this to xml. On the receiving end the xml could be made into a database, upgraded to the current version and the objects loaded and copied. There are probably some security issues there that needs handling.

As stated in the issue, there should be a dialog asking the user if the export should be encrypted with a password, as the export may contain the backup passphrase as well as server credentials.

# [Issue #92](https://code.google.com/p/duplicati/issues/detail?id=#92) 7-zip compression #
This is actually not that complicated, it basically consists of choosing an appropriate 7zip library for .net. My current favorite candidate is [SevenZipSharp](http://sevenzipsharp.codeplex.com/). My major problem with this is that it is windows only, and I would like Duplicati to be platform independent.

The major complication for this issue is to get comfortable enough with the third-party library.

Once you have an appropriate library, it is a matter of implementing the "ICompression" interface, which basically allows Duplicati to use a 7zip archive as if it was a regular folder.

# [Issue #293](https://code.google.com/p/duplicati/issues/detail?id=#293) Global manual override settings #
This consists of creating a new class that implements "IGenericControl" and "IGUIControl". The GUI part should just display the "CommandLineOptionGrid" settings control, and keep a local list of all items that were checked. This can be read from "GetConfiguration()" and written with "Setup(...)".

When the backup starts, it will then load the IGenericControl, which should the apply all stored settings to the commandlineOptions dictionary.

There are some complications as the usercontrol "CommandLineOptionGrid" is in another project and the list of supported options is located in "Options.cs", again a different project. There are also options relating to the selected backend/compression/encryption module, but no such module is selected since the settings are global. One solution could be to just display every supported option for all modules, and then filter unsupported options when the backup is executed.

# [Issue #310](https://code.google.com/p/duplicati/issues/detail?id=#310) Add option to disable diff #
You need to modify "Options.cs" and add a new option, say "no-file-diffs".

Then you need to modify the function "AddFileToCompression" in "SharpRSyncDir.cs" to handle this option and ignore the check in the top "m\_modifiedFiles.ContainsKey(s)". This will ensure that files are always stored in full, rather than by diff.

Next step is to examine the function "Patch" to see how it handles the case where an existing file is being replaced with another file, without being patched, and somehow handle this gracefully.

# [Issue #289](https://code.google.com/p/duplicati/issues/detail?id=#289) eCryptFS support #
This would replace the "SelectFiles" dialog with a new custom eCryptFS dialog, and modify the path taken by the wizard to skip the encryption page, and probably also set the encryption level to 0.

# [Issue #376](https://code.google.com/p/duplicati/issues/detail?id=#376) Google Docs backend #
This is a matter of creating a new project and implementing the "IBackend\_V2" and "IBackendGUI" interfaces. It would probably make sense to take the WebDAV interface or similar as a starting point.

The hard work in this one lies in understanding and communicating through the Google Docs API.

# [Issue #188](https://code.google.com/p/duplicati/issues/detail?id=#188) Scheduled bandwidth management #
Duplicati uses the "LiveControl" class to support changing the bandwidth in real-time.
To solve this issue you would have to design a new table in the database with the information needed to store a bandwidth setup.
Then you would need to create a new tab in the "ApplicationSetup.cs" class and construct whatever user interface you need.
Finally you need to create a class that runs a thread at appropriate intervals (like the Scheduler.cs) and updates the "LiveControl" object with the current bandwidth limits.

You also need to consider what to do in case the user has manually changed the settings.