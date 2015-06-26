Duplicati offers different log files that help analyzing what went wrong when your backup wasn't successful. This wiki page explains the different log files.



## Basic Log File ##
Whenever a backup job is performed, Duplicati writes a basic log file. This log file is accessible via the user interface. Open the status windo and right-click on the performed backup job. The menu offers to "View log ...". The log file produced here provides some basic information about what Duplicati did and if an error occurred you can find the exception message here as well. Some example for a successfully completed job:

```
BackupType      : Incremental
TypeReason      : An incremental backup is made because the latest full backup is from 4/25/2012 9:44:08 PM and the full backup threshold is 1W
BeginTime       : 05/02/2012 18:14:48
EndTime         : 05/02/2012 18:14:50
Duration        : 00:00:01.6932779
DeletedFiles    : 0
DeletedFolders  : 0
ModifiedFiles   : 0
AddedFiles      : 0
AddedFolders    : 0
ExaminedFiles   : 602
OpenedFiles     : 0
SizeOfModified  : 0
SizeOfAdded     : 0
SizeOfExamined  : 0
Unprocessed     : 0
TooLargeFiles   : 0
FilesWithError  : 0
Executable      : Duplicati, Version=1.3.1.1205, Culture=neutral, PublicKeyToken=8bfe994a39631a7b
Library         : Duplicati.Library.Main, Version=1.3.1.1205, Culture=neutral, PublicKeyToken=8bfe994a39631a7b
OperationName   : Backup
BytesUploaded   : 0
BytesDownloaded : 11013
RemoteCalls     : 5

Cleanup output:

Cleanup output:
Deleting backup at 04/24/2012 19:10:42
Deleting backup at 04/23/2012 20:04:05
Deleting backup at 04/21/2012 13:01:23
Deleting backup at 04/20/2012 20:07:12
Deleting backup at 04/17/2012 22:28:11
```

---


## Basic Log File With Debug Output ##
The basic log file can be extended with some debug output in case an error or an exception occurs. To activate this, please go to advanced options and turn on "debug-output". The basic log file will then contain information like this:
```
Error: System.Exception: Fehler Zeitüberschreitung der Anforderung beim Hochladen der Datei "duplicati-inc-content.20120511T072832Z.vol1.zip.aes" ---> System.Net.WebException: Zeitüberschreitung der Anforderung
   at Duplicati.Library.Utility.Utility.SafeGetRequestOrResponseStream(WebRequest req, Boolean getRequest)
   at Duplicati.Library.Utility.Utility.SafeGetRequestStream(WebRequest req)
   at Duplicati.Library.Backend.SkyDriveSession.UploadFile(String remotename, Stream data)
   at Duplicati.Library.Main.BackendWrapper.PutInternal(BackupEntryBase remote, String filename)
   --- End of inner exception stack trace ---
   at Duplicati.Library.Main.BackendWrapper.Put(BackupEntryBase remote, String filename, Boolean forcesync)
   at Duplicati.Library.Main.Interface.Backup(String[] sources)
   at Duplicati.GUI.DuplicatiRunner.ExecuteTask(IDuplicityTask task)
InnerError: System.Net.WebException: Zeitüberschreitung der Anforderung
   at Duplicati.Library.Utility.Utility.SafeGetRequestOrResponseStream(WebRequest req, Boolean getRequest)
   at Duplicati.Library.Utility.Utility.SafeGetRequestStream(WebRequest req)
   at Duplicati.Library.Backend.SkyDriveSession.UploadFile(String remotename, Stream data)
   at Duplicati.Library.Main.BackendWrapper.PutInternal(BackupEntryBase remote, String filename)
```



---


## Advanced Log File ##
In the case that the basic log file does not reveal the required information, you can configure Duplicati to write an advanced log file. Therefore, edit your backup job, go to advanced settings and turn on the option "log-file". Specify a path like e.g. "D:\my\_logfile.txt". You also have to turn on "log-level" and set it to the right value like e.g. Information, Error, ...

```
Profiling: Reading incremental data took 00:26:402
Profiling: Initiating multipass took 00:00:223
Profiling: Writing delta file 1 took 00:00:599
Profiling: Writing remote signatures took 00:00:230
Profiling: Multipass 1 took 00:01:357
Profiling: Writing manifest 20120507T083154Z took 00:00:300
Profiling: Backup from D:\temp\ to file://D:\test took 00:30:106
```




---

## Communication With Server ##
Duplicati is also able to log the communication to the server. This is useful if you assume that e.g. a specific FTP command does not work on your FTP server. To activate this kind of logging, it is required to store a special file in the program folder of Duplicati. This files contains instructions for Duplicati.

Download the required file from http://code.google.com/p/duplicati/issues/detail?id=389#c3 and do the following steps:
  * Rename the file to Duplicati.exe.config if you are not using the command line version of Duplicati
  * Edit the file and change the log file path to something that you have write access to, e.g. D:\my\_ftpserver.log
  * Move the file into Duplicati's program folder
  * Start the backup

This will create a communication log file that looks like this:

```
System.Net Verbose: 0 : [4568] Exiting WebRequest::Create() 	-> FtpWebRequest#10101596
System.Net Verbose: 0 : [4568] FtpWebRequest#10101596::GetResponse()
System.Net Information: 0 : [4568] FtpWebRequest#10101596::GetResponse(Methode = LIST.)
System.Net.Sockets Verbose: 0 : [4568] Socket#51213311::Socket(InterNetwork#2)
System.Net.Sockets Verbose: 0 : [4568] Exiting Socket#51213311::Socket() 
System.Net.Sockets Verbose: 0 : [4568] Socket#5264362::Socket(InterNetworkV6#23)
System.Net.Sockets Verbose: 0 : [4568] Exiting Socket#5264362::Socket() 
System.Net.Sockets Verbose: 0 : [4568] Socket#51213311::Connect(133:21#-2059972398)
System.Net.Sockets Verbose: 0 : [4568] Exiting Socket#51213311::Connect() 
System.Net.Sockets Verbose: 0 : [4568] Socket#5264362::Close()
System.Net.Sockets Verbose: 0 : [4568] Socket#5264362::Dispose()
System.Net.Sockets Verbose: 0 : [4568] Exiting Socket#5264362::Close() 
System.Net Information: 0 : [4568] Associating FtpWebRequest#10101596 with FtpControlStream#64500199
System.Net.Sockets Verbose: 0 : [4568] Socket#1892246::Socket(InterNetwork#2)
System.Net.Sockets Verbose: 0 : [4568] Exiting Socket#1892246::Socket() 
System.Net.Sockets Verbose: 0 : [4568] Socket#1892246::Bind(28:0#481470656)
System.Net.Sockets Verbose: 0 : [4568] Exiting Socket#1892246::Bind() 
System.Net.Sockets Verbose: 0 : [4568] Socket#1892246::Listen(1#1)
System.Net.Sockets Verbose: 0 : [4568] Exiting Socket#1892246::Listen() 
System.Net.Sockets Verbose: 0 : [4568] Socket#51213311::Receive()
System.Net.Sockets Verbose: 0 : [4568] Data from Socket#51213311::Receive
System.Net.Sockets Verbose: 0 : [4568] 00000000 : 32 32 30 2D 2D 2D 2D 2D-2D 2D 2D 2D 2D 20 57 65 : 220---------- We
System.Net.Sockets Verbose: 0 : [4568] 00000010 : 6C 63 6F 6D 65 20 74 6F-20 50 75 72 65 2D 46 54 : lcome to Pure-FT
System.Net.Sockets Verbose: 0 : [4568] 00000020 : 50 64 20 5B 70 72 69 76-73 65 70 5D 20 5B 54 4C : Pd [privsep] [TL
```

---

## Remote Actions Log File ##
In the past a few people submitted bug reports when Duplicati complained that specific files in the backup were missing. Error messages e.g. look like this: "The manifest file indicates that there should be 1 volumes, but the file list indicates 0"

To find out what caused this situation, Duplicati is able to log all activity it causes on the remote server. That way it is possible to see what files have been uploaded or deleted by Duplicati. This kind of logging is turned on the in advanced settings using the option "--backend-log-database" that has to point to a file that you want to write the log file to.

The log file created is an XML file and looks like this:

```
<?xml version="1.0"?>
<duplicati-state-db xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <collection begin="2012-04-11T22:31:36.0692751+02:00" end="2012-04-11T22:33:39.228179+02:00" operation="Backup">
    <list op="List" success="false" timestamp="2012-04-11T22:31:38.1401866+02:00">
      <message>System.Net.WebException: The remote server returned an error: (500) Syntax error, command unrecognized.
   at System.Net.FtpWebRequest.CheckError()
   at System.Net.FtpWebRequest.SyncRequestCallback(Object obj)
   at System.Net.CommandStream.Abort(Exception e)
   at System.Net.FtpWebRequest.FinishRequestStage(RequestStage stage)
   at System.Net.FtpWebRequest.GetResponse()
   at Duplicati.Library.Backend.FTP.List(String filename)
   at Duplicati.Library.Main.BackendWrapper.ListInternal()</message>
    </list>
    <list op="List" success="true" timestamp="2012-04-11T22:31:50.0142623+02:00" />
    <put op="Put" success="false" timestamp="2012-04-11T22:32:00.6854632+02:00">
      <message>System.Net.WebException: Der Remoteserver hat einen Fehler zurückgegeben: (500) Syntaxfehler, Befehl nicht erkannt.
   bei System.Net.FtpWebRequest.CheckError()
   bei System.Net.FtpWebRequest.SyncRequestCallback(Object obj)
   bei System.Net.CommandStream.Abort(Exception e)
   bei System.Net.FtpWebRequest.FinishRequestStage(RequestStage stage)
   bei System.Net.FtpWebRequest.GetRequestStream()
   bei Duplicati.Library.Backend.FTP.Put(String remotename, Stream input)
   bei Duplicati.Library.Main.BackendWrapper.PutInternal(BackupEntryBase remote, String filename)</message>
      <file name="duplicati-full-content.20120411T203150Z.vol1.zip.aes" size="10485536" modified="2012-04-11T22:31:58.6239487+02:00" />
    </put>
    <put op="Put" success="true" timestamp="2012-04-11T22:32:40.8369407+02:00">
      <file name="duplicati-full-content.20120411T203150Z.vol1.zip.aes" size="10485536" modified="2012-04-11T22:31:58.6239487+02:00" />
    </put>
    <put op="Put" success="true" timestamp="2012-04-11T22:32:45.8192256+02:00">
      <file name="duplicati-full-signature.20120411T203150Z.vol1.zip.aes" size="121616" modified="2012-04-11T22:32:40.8389408+02:00" />
    </put>
```