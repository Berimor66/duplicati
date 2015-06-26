# Introduction #

As of [r430](https://code.google.com/p/duplicati/source/detail?r=430), Duplicati has all interfaces in a single dll, called Duplicati.Library.Interface. This dll contains all the interfaces defined, and should not change often.

Prior to [r430](https://code.google.com/p/duplicati/source/detail?r=430), the interfaces were in other dlls, and these dlls changed too frequently.

The commandline version of Duplicati supports plugable modules for backends, compression, encryption and generic modules.

The GUI version of Duplicati supports GUI interfaces for the modules mentioned, as well as  for applicationwide settings.

When required, Duplicati will scan the application folder for dll's and load the classes that implements the required interfaces.

If you develop a custom class and implement the required interface, Duplicati will detect and load the class at runtime.

The dll Duplicati.Library.DynamicLoader handles this.

All classes are required to implement a default constructor (eg. a constructor taking no arguments) for reading various properties, and a constructor with options relating to the task the class performs.

## IBackend ##
The IBackend interface is the interface for backends. As backends are a central concept in Duplicati, there is a separate page on [how to write backends](CustomBackendHowTo.md).

Backends are invoked using the destination url and a collection of commandline options.

## IStreamingBackend ##
The IStreamingBackend interface is an extension to the IBackend interface. All backends should also implement this interface, if possible. See the page on [how to write backends](CustomBackendHowTo.md) for further details.

## IBackendGUI ##
The IBackend class can register a custom userinterface for setting up the backend in the wizard by implementing the IBackendGUI interface.

Backends that do not implement the IBackendGUI will be presented to the user as a grid with the commandline options.

## ICompression ##
The ICompression interface is for compression modules. By using the ICompression interface, Duplicati can view a compressed file as if it was a simple folder with files.
A class which implements the ICompression interface will be invoked with a path to a file and a collection of commandline options.

If the referenced file is empty or does not exist, the instance is expected to create the archive and prepare for compression, otherwise the existing archive should be opened and prepared for decompression.

## ICompressionGUI ##
If the compression module has settings that should be presented to the user, the ICompression class can also implement the ICompressionGUI interface. Currently ([r430](https://code.google.com/p/duplicati/source/detail?r=430)) the ICompressionGUI interface is not used.

## IEncryption ##
The IEncryption interface is for encryption modules. The IEncryption interface allows Duplicati to view the encryption/decryption process as a simple function call. In the current implementation ([r430](https://code.google.com/p/duplicati/source/detail?r=430)), all operations are done on files, so the encryption/decryption can rely on file seek being available.

## IEncryptionGUI ##
If the encryption module supports commandline options, a grid with options will be displayed in the Duplicati GUI. If the IEncryptionGUI interface is also implemented, Duplicati will use that to display a custom control for configuring the module instead. If the options are few, eg. a single switch, the instance can also implement the IGUIMiniControl interface, which will make Duplicati display the control on the password settings page, rather than on a separate page.

If there should be no GUI displayed, simply return an empty control (eg. `return new System.Windows.Forms.Control();`).

## IGenericModule ##
Generic modules can be deployed to implement custom behavior that is not linked to a particular feature in Duplicati. The [r430](https://code.google.com/p/duplicati/source/detail?r=430) version of Duplicati has no generic modules.

A few examples of generic modules that could be implemented:
  * a passphrase validation module that ensures that the passphrase is strong enough
  * a username/password fetcher that reads these from a remote storage facility
  * a proxy configuration module

## ISettingsControl ##
When implementing one of the above modules, they may have global scope settings. Eg. the SSH module has a global setting that points to the sftp executable. The S3 backend has some default settings and remembers the username/passwords entered for the service.

Such global settings are supported through the use of the ISettingsControl interface. By implementing this interface, it is possible to get a custom tab in the Duplicati Options dialog.

When the user creates or edits a backup, the ISettingsControl is invoked, allowing the module to pass data to another module.

The ISettingsControl classes are also invoked when running a backup, allowing them to alter commandline options and other environment settings.

## IGUIControl ##
To simplify the code, all GUI enabled controls use the IGUIControl interface. There should be no need to use this interface directly.

## IGUIMiniControl ##
The IGUIMiniControl is a marker interface, meaning that it has not methods or properties. The purpose of the interface is to mark the IGUIControl as a "small" control. Duplicati uses this as a hint to compress the screen space used. In the current revision ([r430](https://code.google.com/p/duplicati/source/detail?r=430)) this is only used for the IEncryptionGUI.

In future versions this may also be used to group multiple ISettingsControl instances together on a single page, rather than in a tab each.