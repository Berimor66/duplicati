# Introduction #

This document describes Duplicati as it looks from a developer perspective. As with any reasonable size system, it is impossible to describe and understand all details of the system. As with any system there is a constant change, making any detailed description obsolete almost before it was written.

Instead, this document attempts to describe the module division and design choices behind the system. This approach should ensure that this document has a longer lifespan than just the next release.

If you are looking for a way to get started with Duplicati development, look at the [Duplicati Hacking Projects](DuplicatiHackingProjects.md) for a list tasks to get started with.

# Modularity and extensibility #

Duplicati was designed with modularity and extensibility in mind. The 1.2 version of Duplicati supports dynamically loaded modules for backends, encryption and compression. If your primary goal is to develop a new module that falls in one of those categories, it is simply a matter of implementing the correct interface. See the module "Duplicati.Library.Interface" for the supported interfaces.

# Division of responsibilities #

The innermost module in Duplicati is the "Duplicati.Library.Main". This is the module that performs the actual backup operations. Essentially "Duplicati.CommandLine" and "Duplicati" (the GUI) are two different wrappers for activating "Duplicati.Library.Main".

The idea is that both the commandline and the GUI version can have an equally well defined interface to the backup system. This ensures that unittests are easier to execute as they do not rely on the GUI. It also ensures that Duplicati is usable in headless environment, such as when it is automatically executed as a script.

Rather than let the GUI just spawn the commandline version, the GUI hooks directly into "Duplicati.Library.Main" as this ensures a much more stable and flexible communication system than relying on communication through the stdin and stdout console channels.

Having two interfaces also helps enforce the modularity, in that a feature is less likely to rely on a specific GUI component, eg. using a messagebox during a backup, or requesting commandline input.

## Duplicati.CommandLine ##

This module is a simple commandline parser that just activates the requested operation in "Duplicati.Library.Main" and passes all output to the console.

## Duplicati ##

This is the GUI program. It is responsible for running a backup scheduler, as well as the tray icon. It also hosts all the wizard pages and logic for controlling a backup.

All setup and log data is stored in a [SQLite](http://www.sqlite.org/) database. The class "DatabaseUpgrader" is responsible for ensuring that the database format is seamlessly upgraded between versions.

The "Scheduler" class is a thread that monitors the current time and figures out when to run the next backup. When a scheduled backup must be executed, it is transferred to an instance of the WorkerThread class, which maintains a list of backups that should be executed. The class "DuplicatiRunner" is performing the actual backup, which involves invoking the "Duplicati.Library.Main" module.

### Datamodel ###
The data manipulated by the GUI module is abstracted into the classes found in "Datamodel", which are controlled by an [Object-relation-mapper](http://en.wikipedia.org/wiki/Object-relational_mapping) called [LightDataModel](http://code.google.com/p/lightdatamodel). The LDM maintains all relations between objects and transparently loads an stores changes in the database.


## Duplicati.Library.Main ##

Inside "Duplicati.Library.Main", the class "Interface" has a static and instance interface for performing the supported operations. If you wish to implement a new function for Duplicati, the "Interface" class is the place to start.

The "Interface" class is responsible for performing various logic operations, such as validating input, and figuring out which files to transfer.

All the actual work backup work is done inside the "RSyncDir" class. This class was initially intended to serve as a simple wrapper for RSync, so it could support folders as well. It still does that but is now logically more of a wrapper of reading and writing volume files. The "RSyncDir" class uses the ICompression interface as the storage abstraction for storing files in compressed volumes.

The "BackendWrapper" class extends the normal "IBackend" interface with a number of useful functions, such as retry support, bandwidth throttling and asynchronous operation. The "BackendWrapper" class is also responsible for handling encryption and decryption by invoking the correct IEncryption module. Applying encryption at this stage ensures that the remainder of the system can ignore encryption, as everything that goes in and out of the "BackendWrapper" appears to be unencrypted.

Finally the "Options" class encapsulate all supported options (except those from dynamically loaded modules). As options in Duplicati is actually a regular dictionary of strings, the encapsulation ensures that Duplicati has type-safe access to all options.

The classes "RSyncDir" and "Interface" are by far the most complicated classes, and contain a large amount of code. The "RSyncDir" class is difficult to understand because it has a vast number of instance variables which are used to support creating multiple volumes. The basic operation sequence is to call "InitiateMultiPassDiff", then repeatedly call "MakeMultiPassDiff" until there are no more files, and finally call "FinalizeMultiPass". Furthermore the "RSyncDir" class contains logic for both backup and restore operations, making the number of variables larger than necessary.

## Duplicati.Library.Utility ##

The "Duplicati.Library.Utility" module was previously named "Duplicati.Library.Core". The original name was chosen because almost all other modules have a reference to this module. But the name suggested that the module was the main module, which was misleading.

The utility module contains many commonly used functions, such as "TempFile", "TempFolder", "PlugableEnumerable" (LinQ like) that I feel are missing from the .Net 2.0 framework.

It also supports throttling streams, applying file filters, enumerating files and folders, and other operations that could be useful in other projects.

There are also a few Duplicati specific items, such as the "TimeParser".

## Duplicati.Library.Interface ##

This module contains all supported interfaces, and as with the "Duplicati.Library.Utility" it is usually referenced by all other modules.

To support dynamic loading of plugins that are not compiled for the specific version of Duplicati, the standard installation is shipped with an assembly redirect manifest, which enables any module that references any version of "Duplicati.Library.Interface" or "Duplicati.Library.Utility" to be loaded.

This means that any changes made to these two modules should be backwards compatible, which means that no renaming should be done inside these modules.

## Duplicati.Library.DynamicLoader ##

The dynamic loader is responsible for loading instances of the supported dynamic loadable interfaces, IBackend, IEncryption, ICompression, IGenericModule and IGUIControl.

### Duplicati.Library.Encryption ###

This module contains the stock encryption code, which is currently an AESCrypt and a GPG implementation. The AESCrypt module is fully managed, where the GPG module works by invoking the GPG executable on the system, and passes data through the console stdin and stdout.

### Duplicati.Library.Compression ###

This module contains the stock compression modules, which is currently limited to the Zip format.

## Duplicati.Library.Snapshot ##

This module is responsible for providing the snapshot options available in Duplicati. It works by exposing a "ISnapshot" interface which enables the caller to get a list of files and folders found in a certain folder, and also enables the caller to open those files. Internally the normal filenames are mapped to the snapshot paths, and the streams returned are taken from the snapshots, making the snapshot usage transparent for the caller.