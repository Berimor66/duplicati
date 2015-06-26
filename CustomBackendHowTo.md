# How to write a custom backend #
Duplicati has been designed to allow custom backends to plug in easily. The backends may be open or closed source, and it is not required that backends are known by the Duplicati project. This page describes how to write a new backend from scratch.

## How backends work ##
Each backend is described by a class. This class must be placed in an assembly (dll) located in the same folder as Duplicati. Duplicati will load assemblies from the folder, and search for classes that implement the IBackend interface.

The backend must have a default constructor and a constructor that takes these two arguments:
`constructor(string destination, Dictionary<string, string> options)`
The destination parameter is the url used to setup the backend, and the options is a collection of commandline parameters (without the leading double hyphen). Commandline parameters are usually passed in the form "--option-name=value" and avalible as an entry with the name "option-name" and a value of "value".

A backend must support 4 operations
  * List()
  * Get(remotename, localname)
  * Put(remotename, localname)
  * Delete(remotename)

Since the constructor knows the path, the 4 methods above work on that folder. You can think of this as setting the working directory in the constructor.

### The protocol key ###
When writing a new backend, simply create a "class library" (a dll project). Then add a reference to the Duplicati.Library.Interface.dll.
Your class must implement the IBackend interface, which has the above 4 methods. Besides the 4 methods, there are also the required property ProtocolKey. This is used to select the backend, when encountering an url that starts with the protocolkey, eg. the SSH backend uses the protocol key "ssh" and gets urls of the type "ssh://server/path". It is important to select a unique name for the protocol key, and not use eg. "http", as that will likely clash with another provider at some point.

### User display names ###

There are also two other properties, DisplayName and Description, which is displayed to the user when the selecting the backends. These two strings should be localized, the ProtocolKey should not.

### Reporting the supported options ###

The final property to implement is the SupportedCommands property. This property must return a list with all the supported commandline options that the backend supports. This data is displayed to the user if the user invokes the usage from the commandline, and if the backend does not provide a custom user interface. Strings in this list should be localized, except for the actual switch names.

Although it's a long list, it's really nothing more than just implementing the IBackend interface.
The file based backend is a great starting point, as it is very simple.
The WebDAV backend is a bit better as an example if the new backend uses the http protocol.

## The streaming backend ##

Backends that supports streaming data gets two benefits:
The progress bar in the user interface moves according to transfer progress
The backend can be throttled by the user

To flag a backend as streaming, simply implement the IStreamingBackend interface.
This interface has overloaded the Get and Put methods so they can take a stream.

If at all possible, the backends should implement the IStreamingBackend interface.
The only stock backend that does not support this interface is the SSH backend, because it is fairly difficult to stream data into an external process.

## Implementing a custom userinterface ##
From [r288](https://code.google.com/p/duplicati/source/detail?r=288) and forward backends can now provide a custom userinterface for the wizard. If a backend implements the IBackendGUI interface, Duplicati will display a custom wizard page for the backend. If the backend only implements the IBackend interface Duplicati will display a grid with the options reported by the SupportedCommands property.

### Display strings ###
Implementing the IBackendGUI means implementing two properties called PageTitle and PageDescription. Those two strings should be localized, and are displayed in the top part of the wizard page, and should provide a short description to the user.

### The actual control ###
The control that represents the userinterface must derive from the System.Windows.Forms.Control class, and be returned by the GetControl method. This method gets a collection of application settings, and a collection of backend options. The application settings may vary between Duplicati versions, and contains items such as the temporary folder location. The control should be 506 pixels wide and 242 pixels high.

### Dealing with the option collection ###
The backend options should represent the state of the user interface. Bear in mind that when the user creates the backup, the option collection is empty, and when the user edits the backup, the option collection may be from an older version. Backends should be able to cope with either situation.

The backend options is a collection of key/value strings, and both key and value may be chosen freely. It is recommended that the backend uses descriptive name, such as "Server name", so a user that browses the settings database knows what its for. The backends in the Duplicati interface prefix the string "UI: " to any options that only impacts the userinterface, and not the connection, such as a flag indicating if the user has tested the backend. Backend implementers are encouraged to follow this strategy.

### Saving and validating ###
When the user clicks either "Next" or "Back" the Leave method is invoked, and the backend should save the settings from the user interface into the backend options collection. If the user clicked "Next" the Validate method is invoked afterwards. If the userinterface has acceptable data, the methods should return true. Otherwise the code should notify the user of the problem (eg. by a messagebox) and return false. It is important that the backend saves the settings in the Leave method, and do NOT validate when this method is called, because the user should be able to go "Back" at any time, only the "Next" button should require validation.

### Converting to commandline options ###
The final method required by the IBackendGUI is the GetConfiguration method. This method gets the application settings, the previously save backend settings and the commandline options. Based on the backend settings from the userinterface, this method should set commandline options for the backend, and return the destination string that will invoke the backend. The string returned and the commandline options is then passed on to the backend constructor.

As all backends are invokable from the commandline, this is the interface that Duplicati uses. By translating the userinterface settings into commandline options, it is easy to keep the displaysettings seperated from the actual backend input, as the backend/commandline input is usually more advanced than the userinterface.

## Mono, Linux and Mac ##
As Duplicati is itself 100% portable across the major platforms, please try to make sure the backends are as well.

## LGPL and proprietary backends ##
AFAIK, it is perfectly legal to create a proprietary backend, and have Duplicati load it. If you are a lawyer and see a problem, please let me know.

## Shipping a custom Duplicati ##
If there is just one backend, the backend selection page will disappear.
If you have ideas for branding or otherwise customizing Duplicati, please create a ticket describing the desired changes.

## Improving Duplicati ##
If you need help with implementing a backend, please ask in the groups.
If the current backend system is poor or inefficient, please add a ticket describing the problem.
If you have a great idea or request for improving backend support, please create a ticket.

Happy coding!