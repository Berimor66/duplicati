# Duplicati components. #

Like most other applications, Duplicati is constructed from a number of components. Great care has been taken to ensure that these components are properly encapsulated and reusable in other projects.

Since Duplicati is released under the open source LGPL license, you can even use these components in closed source applications. This document explains what components are avalible, and how you could use them in other projects. All the components described here are written purely in managed code (C#), to ensure maximum portability. (SQLite is unmanaged code, but avalible for al major platforms).

## LightDataModel (LDM) - A light OR-mapper ##
In most applications you need to store some settings to persistent storag (usually the disk). There are many ways to do so, and usually a config file is used. In newer applications an Xml is typically used. If you have to update data frequently or have complex data, it becomes problematic to ensure that data is always updated correctly. For many years, these problems have been adressed by databases.

When you access a database there are a number of other problems, namely parsing values and updating relations. To solve this, a number of Object-Relation (OR) frameworks have been developed. For .Net, the two largest seems to be NHibernate and NPersist. Both have a huge number of features and are well documented, but IMO difficult to use. LDM is an attempt to create an OR-mapper that is simpl to use and understand. It does not attempt to match other OR-mappers in terms of features, but it does a reasonable job, and supports SQLite. Using SQLite and LDM you have the power of a relational database in .Net, but its portable, cross-platform and easy to use.

Duplicati uses SQLite and LDM to store settings and output from log files.

LightDataModel is developed as a separate project, and avalible for download at:
http://code.google.com/p/lightdatamodel

## SharpRsync - A managed RSync implementation ##
RSync is both an algorithm and an application/library. It makes syncronization of files efficient, by extensive use of hashing. The basic idea is to create a hash-file and a content-file. The hash-file is called a signature, and contains two hash values for each datachunk of a predefined size. There are two hash values, a weak one called a "rolling-cheksum" and a strong one (MD4). The weak checksum is used to support that the same datapiece moves inside a file. The content-file is called the delta file, and contains a series of operations that should be performed on one file to produce another. Put in another way, it stores the differences between two files. The signature files is usually small, and the delta file is usually large, as it contains the changed data.

As explained, with the orignal file, and the delta file, you can reconstruct the updated file. What makes RSync really efficient, is that IF you have an updated file, and the signature file for the previous version, you can generate a delta file containing only the changes from the updated versions. This means that you can store multiple versions of the same file with very little overhead, and that you can generate the delta files with just a signature file and the current file, meaning that the disk space required is very limited.

To restore a file with multiple delta files, simply apply them in order. If you stop the sequence at some point, you can even get a specific version of the file.

SharpRsync is encapsulated in a single dll, and you can use it to efficiently syncronize files. The data produced and consumed by SharpRsync is binary compatible with the official (unmanaged) RSync library and executable.

## Wizard - A wizard framework for .Net ##
Many applications are constructed in way that is optimal for power users, with menus and easy access to all options. If the end user group includes novice computer users, such an interface can be ovewhelming. A wizard based interface can help reduce the confusion by guiding the user through an otherwise complex process.

A wizard based user interface can sometimes greatly simplify an otherwise complex task. Novice users can focus on one or two questions at a time.

Even though a wizard based interface is a well known user interface, there is no built-in support for this in the .Net framework.

The wizard framework in Duplicati is selfcontained, and uses UserControls to display the pages in the wizard. This enables you to provide 100% custom pages. There are enter and exit events so you can setup custom validation. The framework also saves each page setup in a large dictionary, so the user can go back and forth in the pages without loosing information.

The framework supports both a predefined list of pages, as well as dynamic path determination, allowing you to skip pages to get the optimal flow. The wizard framework also contains a RadioButton that can be doubleclicked, as that is standard in wizards, but not avalible in the .Net framework.

## FreshKeeper - An update manager for .Net ##
One of the major issues with system administration is keeping an application up-to-date. Most Linux systems have an easy way to accomplish this, but not all applications are avalible in the package manager.

As a developer it is nice to know that the users have easy access to the latest version of the application.

FreshKeeper is designed to work without server requirements, by being completely file based on the server. To protect users against malicious downloads, all update notifications and update packages are cryptographically signed.

FreshKeeper can work as a standalone executable for use with non .Net applications, and integrate completely with .Net applications. There is just one executable, which serves as both the administrative author tool, as well as the client application.

The client is distributed with a configuration document, that contains the public key part of the encryption, and a number of update Url's. Using this information, the client can downlod and verify the update list. The author part is merely an editor for the xml document that is placed in the location pointed to by the client config urls.

FreshKeeper also conveys optional information about the updates, such as changelogs and update severity, enabling the end user to follow development or just stick to the major releases.