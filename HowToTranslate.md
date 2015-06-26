# Introduction #

This guide will show you how to translate Duplicati to your prefered language.


## Please note that we are currently building a new user interface, and this means different strings. ##
## Even though we would like as many languages supported as possible, it is wasteful to start a translation project based on the current source code. We will make an announcement on Duplicati.com, Facebook and Google+ when the codebase is stable enough to start translating. ##


# The new easy way #
If you simply want to translate all strings, just go to the
[Duplicati Translations page](https://docs.google.com/leaf?id=0B9tKh3OEoqEqOTZmNWI1NWEtOTZkNi00OTk4LTkzMWEtOGI1M2UzMTc3NWYy).

On the page, you can see the existing translations, as well as one called empty. If you want to start a new translation, simply open the [empty translations](https://spreadsheets.google.com/ccc?key=0AttKh3OEoqEqdHhFUV9qa2loSkhQNGNTMi0zejA5WEE) document. You cannot edit it, so save a copy by choosing "Files" "Download as". You can now edit the file in whatever spreadsheet editor you prefer. The "C" column has the translation status, and you will need to translate the items with status "missing" or "not-updated".

When you are done with the translation, please create a ticket, and attach the document to the issue, and I will pick it up from there. If you would like to be the maintainer of a translation, create an issue, and I will grant you edit access to the spreadsheet so you can modify it directly.

If you want to update one of the existing language files, just download the corresponding file instead of the empty one, and all steps are the same as above.

# The hard way #

If you want more that just translating strings, such as adjusting button sizes etc. you need some build tools. Using this method will also allow you to try out the translations directly on your local machine.

## Prerequisites ##

Before you can start building translations to Duplicati, you need to have the following software installed:

  * Duplicati 1.0.1 [r229](https://code.google.com/p/duplicati/source/detail?r=229) or later [Download](http://code.google.com/p/duplicati/wiki/Downloads)
  * Microsoft .Net Framework 2.0 [Download](http://www.microsoft.com/downloads/details.aspx?FamilyID=0856EACB-4362-4B0D-8EDD-AAB15C5E04F5) (Should be installed)
  * Microsoft .Net 2.0 SDK [Download](http://www.microsoft.com/downloads/details.aspx?FamilyID=fe6f2099-b7b4-4f47-a244-c96d69c35dec) (Included in Visual Studio 2005 or newer)
  * A Subversion client, TortoiseSVN is fine for Windows [Download](http://tortoisesvn.net/downloads)

All the mentioned files should install just fine under Wine, except TortoiseSVN.

## Getting the files ##

To translate files for Duplicati, you need to perform a Subversion checkout. <br />
With TortoiseSVN, simply right click on any folder, then choose "TortoiseSVN checkout...".<br />
The repository URL is:
```
http://duplicati.googlecode.com/svn/trunk/
```
You can choose any destination folder you like.<br />
When you click OK, the Duplicati source code is downloaded, which may take a few minutes.

If you have a commandline subversion client, the following command should get the source:
```
svn checkout http://duplicati.googlecode.com/svn/trunk/ duplicati
```

Whenever there is a change in Duplicati, you can update your copy with this command:
```
svn update duplicati
```

With TortoiseSVN, simply right click the folder, and select "TortoiseSVN update".

## Update the configuration file ##
Inside the folder you just created, there should exist a folder called "Duplicati",
and inside this, a folder named "Localization". In this folder, you must edit the file
"configuration.xml". Near the bottom of the document you will find:
```
    <keyfile>..\GUI\Duplicati.snk</keyfile>

    <versionassembly>..\GUI\bin\release\Duplicati.exe</versionassembly>

    <sourcefolder>..</sourcefolder>

    <outputfolder>compiled</outputfolder>

    <productname>Duplicati</productname>
```

Everything should be fine, except the `<versionassembly>` path. Since you have not compiled Duplicati, this
file does not exist. You must update this path to point to the Duplicati.exe file that you whish to translate.
With a standard Duplicati install on an english machine, you can enter:
```
C:\Program Files\Duplicati\Duplicati.exe
```

Remember that localization only works with Duplicati 1.0.1 [r229](https://code.google.com/p/duplicati/source/detail?r=229) or newer.

You can also install Visual Studio 2009 (Express is fine), and build the solution, which will create the file.

## Creating a language ##

To create a new language, you need to be in a command prompt.
To get there, click "Start", then "Run".
In the window, type "cmd" and press enter.

You now need to go to the folder where you checked out Duplicati, eg: D:\Documents\Duplicati.
```
D:
cd "Documents\Duplicati\Duplicati\Localization"
```

Run the LocalizationTool.exe to create the language, eg en-US:
```
LocalizationTool.exe create en-US
```

Wait a little, and there should now be a folder called "en-US".
Inside this folder, you can now translate the .resx files and .txt files.

## Word of warning ##

It can be a tedious task to translate large amounts of text.

I recommend that you do not attempt to translate everything in one stretch.

Try to allocate an hour or two each other day, and stop after that time, and
rest until the next turn.


### Localizing windows (forms) ###

Many resx files will exist twice:
```
ApplicationSetup.resx
ApplicationSetup.en-US.resx
```

You can open the localized version with the "winres.exe" tool avalible in the .Net SDK, which you have installed.

If you open the file `ApplicationSetup.en-US.resx` with "Windows Resource Localization Editor" (the shortcut for winres.exe), you can visually see the window, and modify it interactively. You can move things around to correctly display text, but please try to keep the layout as close to the original as possible.

Once done, save the file.

#### Localizing strings ####
Duplicati contains a large number of messages that are not directly displayed in a window, such as error messages.

Unfortunately the "winres.exe" does not support editing these files, so you have to use a text editor, such as the free [Notepad++](http://notepad-plus.sourceforge.net/uk/download.php), or read below on how to use LocalizationTool to do this.

All the files that are missing a "neutral" resx file (those with only a `*`.en-US.resx file) must be edited with a text editor. They all start with a lot of boilerplate system code, and then multiple entries of the form:
```
  <data name="TimeParseError" xml:space="preserve">
    <value>Unable to parse "{0}" into a time offset: {1}</value>
  </data>
```

The "name" attribute is a system key, and may not be changed.
The text between the `<value>` and `</value>` is the one that needs translating.
The special items {0} and {1} are placeholders for some text that Duplicati will insert. It should be fairly obvious what the fields will be substituted with, but if in doubt, ask on [the Duplicati user group](http://groups.google.com/group/duplicati).

#### Using the LocalizationTool to handle string translation ####

The LocalizationTool.exe can also aid you in editing the files, if you invoke:
```
LocalizationTool.exe GUIUPDATE en-US
```
You will see a GUI with all the strings that are not yet translated. The window will also count down on the number of missing strings to translate. Simply replace the text in the box on the right, and then press ALT+N to go to the next entry.

To skip common stuff, eg. if the "OK" button should also be labelled "OK" in the translated version, create a file called "ignore.en-US.xml" in the same folder as LocalizationTool.exe:
```
<root>
    <ignore>OK</ignore>
    <ignore>Link</ignore>
</root>
```

#### Using the LocalizationTool to import/export CSV files ####

The LocalizationTool.exe can also handle import and export of CSV files, so you may edit strings in your favorite spreadsheet editor. Run LocalizationTool.exe without any arguments to see the supported commands.

The format of the file is:
  * String separator: "
  * Field separator: ,
  * Encoding: Unicode UTF-8

Please note that MS Excel cannot handle linebreaks inside the CSV files, so you will need to convert it first, either through odt or google docs.

## Handling updates ##
From time to time, Duplicati will get new strings that require translation.
If you have already checked out an older version of Duplicati, simply update it as described in the "Getting the files" section above.

To update the language files to have placeholders for the new strings, run:
```
LocalizationTool.exe UPDATE en-US
```

After this, you can use the GUI to only display the missing entries:
```
LocalizationTool.exe GUIUPDATE en-US
```

## Building and testing the dll's ##

When you are ready to test your work, run the following command:
```
LocalizationTool.exe build
```

This will build the resource assemblies, and place them in the folder called "compiled".
Inside the "compiled" folder you will now see a folder called "en-US".
Copy this folder to your duplicati installation, so you get a folder structure like:
```
C:\Program Files\Duplicati\Duplicati.exe
C:\Program Files\Duplicati\Duplicati.CommandLine.exe
C:\Program Files\Duplicati\en-US\Duplicati.resources.dll
C:\Program Files\Duplicati\en-US\Duplicati.CommandLine.resources.dll
```

If your operating system is reporting the "en-US" locale, Duplicati should pick this up,
and use the new language dll's automatically. If not, you can go to settings and select the new language.

## Contributing a translation ##
Once you translate all the files, you can contribute them to the Duplicati project,
so others may benefit from your hard work.

Before you can submit the translation files, you must [create an issue](http://code.google.com/p/duplicati/issues/list). In the issue type the name of the language your are translating, and state that you are accepting that the Duplicati project publishes your files under the LGPL license.

Then simply attach a zip archive with the files to the ticket, and I will create a compiled version for you if you need to test it. Once you are satisfied, I will commit the changes to the Duplicati project, and it will then be avalible in later releases.

## Getting credit ##
There is a "Credits" page in the options dialog, where I will put your name in, unless you ask me not to.

Happy translating!