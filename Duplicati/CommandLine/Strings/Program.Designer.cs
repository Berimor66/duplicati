﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5420
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duplicati.CommandLine.Strings {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Program {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Program() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Duplicati.CommandLine.Strings.Program", typeof(Program).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Added folders.
        /// </summary>
        internal static string AddedFoldersHeader {
            get {
                return ResourceManager.GetString("AddedFoldersHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found {0} backup chains on backend
        ///
        ///Type\tTime\t\t\tVolumes\tSize.
        /// </summary>
        internal static string CollectionStatusHeader {
            get {
                return ResourceManager.GetString("CollectionStatusHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Full\t{0}\t{1}\t{2}.
        /// </summary>
        internal static string CollectionStatusLineFull {
            get {
                return ResourceManager.GetString("CollectionStatusLineFull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  Inc\t{0}\t{1}\t{2}.
        /// </summary>
        internal static string CollectionStatusLineInc {
            get {
                return ResourceManager.GetString("CollectionStatusLineInc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Control files.
        /// </summary>
        internal static string ControlFilesHeader {
            get {
                return ResourceManager.GetString("ControlFilesHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleted files.
        /// </summary>
        internal static string DeletedFilesHeader {
            get {
                return ResourceManager.GetString("DeletedFilesHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleted folders.
        /// </summary>
        internal static string DeletedFoldersHeader {
            get {
                return ResourceManager.GetString("DeletedFoldersHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicati options:.
        /// </summary>
        internal static string DuplicatiOptionsHeader {
            get {
                return ResourceManager.GetString("DuplicatiOptionsHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt; not found &gt;.
        /// </summary>
        internal static string FileEntryNotFound {
            get {
                return ResourceManager.GetString("FileEntryNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}\t{1}.
        /// </summary>
        internal static string FindLastVersionEntry {
            get {
                return ResourceManager.GetString("FindLastVersionEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Backuptime\t\tFilename.
        /// </summary>
        internal static string FindLastVersionHeader {
            get {
                return ResourceManager.GetString("FindLastVersionHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The folder {0} was created.
        /// </summary>
        internal static string FolderCreatedMessage {
            get {
                return ResourceManager.GetString("FolderCreatedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Supported generic modules:.
        /// </summary>
        internal static string GenericModulesHeader {
            get {
                return ResourceManager.GetString("GenericModulesHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to parse &quot;{0}&quot; into a number.
        /// </summary>
        internal static string IntegerParseError {
            get {
                return ResourceManager.GetString("IntegerParseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The option --{0} was supplied, but it is reserved for internal use and may not be set on the commandline.
        /// </summary>
        internal static string InternalOptionUsedError {
            get {
                return ResourceManager.GetString("InternalOptionUsedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Modified files.
        /// </summary>
        internal static string ModifiedFilesHeader {
            get {
                return ResourceManager.GetString("ModifiedFilesHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module is loaded atomatically, use --disable-module to prevent this.
        /// </summary>
        internal static string ModuleIsLoadedAutomatically {
            get {
                return ResourceManager.GetString("ModuleIsLoadedAutomatically", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module is not loaded atomatically, use --enable-module to load it.
        /// </summary>
        internal static string ModuleIsNotLoadedAutomatically {
            get {
                return ResourceManager.GetString("ModuleIsNotLoadedAutomatically", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New files.
        /// </summary>
        internal static string NewFilesHeader {
            get {
                return ResourceManager.GetString("NewFilesHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New/Modified files.
        /// </summary>
        internal static string NewOrModifiedFilesHeader {
            get {
                return ResourceManager.GetString("NewOrModifiedFilesHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A &lt;backend&gt; is identified by an url like ftp://host/ or ssh://server/.
        /// Using this system, Duplicati can detect if you want to backup or restore.
        /// The cleanup and delete commands do not delete files, unless the --force option is specified, so you may examine what files are affected, before actually deleting the files.
        /// The cleanup command should not be used unless a backup was interrupted and has left partial files. Duplicati will inform you if this happens.
        /// The delete command can be used to remove bac [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ProgramUsageBackend {
            get {
                return ResourceManager.GetString("ProgramUsageBackend", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Backup (make a full or incremental backup):
        ///  Duplicati.CommandLine [full] [options] &lt;sourcefolder&gt; &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageBackup {
            get {
                return ResourceManager.GetString("ProgramUsageBackup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cleanup (remove partial and unused files):
        ///  Duplicati.CommandLine cleanup [options] &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageCleanup {
            get {
                return ResourceManager.GetString("ProgramUsageCleanup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Create folder (usually done automatically):
        ///  Duplicati.CommandLine create-folder [options] &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageCreateFolders {
            get {
                return ResourceManager.GetString("ProgramUsageCreateFolders", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete old backups:
        ///  Duplicati.CommandLine delete-all-but-n-full &lt;number of full backups to keep&gt; [options] &lt;backend&gt;
        ///  Duplicati.CommandLine delete-older-than &lt;max allowed age&gt; [options] &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageDeleteOld {
            get {
                return ResourceManager.GetString("ProgramUsageDeleteOld", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Filters:
        /// Duplicati uses filters to include and exclude files.
        ///  Duplicati uses a &quot;first-touch&quot; filter where the first rule that matches a file determines if the file is included or excluded. Internally Duplicati uses regular expression filters, but supports filters in the form of filename globbing. The order of the commandline arguments also determine what order they are applied in. An example:
        ///    --include=*.txt --exclude=*\Thumbs.db --include=*
        ///
        ///  Even though the last filter includes everything, no [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ProgramUsageFilters {
            get {
                return ResourceManager.GetString("ProgramUsageFilters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Find the last version of a file:
        ///  Duplicati.CommandLine find-last-version [options] --file-to-restore=&lt;files to find&gt; &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageFindLastVersion {
            get {
                return ResourceManager.GetString("ProgramUsageFindLastVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ********** Duplicati v. {0} **********
        ///
        ///Usage:.
        /// </summary>
        internal static string ProgramUsageHeader {
            get {
                return ResourceManager.GetString("ProgramUsageHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to List content files (files that can be restored):
        ///  Duplicati.CommandLine list-current-files [options] &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageListContentFiles {
            get {
                return ResourceManager.GetString("ProgramUsageListContentFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to List files:
        ///  Duplicati.CommandLine list [options] &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageListFiles {
            get {
                return ResourceManager.GetString("ProgramUsageListFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to List backup sets:
        ///  Duplicati.CommandLine collection-status [options] &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageListSets {
            get {
                return ResourceManager.GetString("ProgramUsageListSets", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to List signature files (files in a single backup volume set):
        ///  Duplicati.CommandLine list-actual-signature-files [options] &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageListSignatureFiles {
            get {
                return ResourceManager.GetString("ProgramUsageListSignatureFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to List source folders (folders backed up from):
        ///  Duplicati.CommandLine list-source-folders [options] &lt;backend&gt;.
        /// </summary>
        internal static string ProgramUsageListSourceFolders {
            get {
                return ResourceManager.GetString("ProgramUsageListSourceFolders", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Option types:
        /// The following option types are available:
        ///  Integer: a numerical value
        ///  Boolean: a truth value, --force and --force=true are equivalent. --force=false is the opposite
        ///  Timespan: a time in the special time format (explained below)
        ///  Size: a size like 5mb or 200kb
        ///  Enumeration: any of the listed values
        ///  Path: the path to a folder or file
        ///  String: any other type.
        /// </summary>
        internal static string ProgramUsageOptionTypes {
            get {
                return ResourceManager.GetString("ProgramUsageOptionTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Purge signature cache:
        ///  Duplicati.CommandLine purge-signature-cache [options].
        /// </summary>
        internal static string ProgramUsagePurgeCache {
            get {
                return ResourceManager.GetString("ProgramUsagePurgeCache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Restore (restore all or some files):
        ///  Duplicati.CommandLine [options] &lt;backend&gt; &lt;destinationfolder&gt;.
        /// </summary>
        internal static string ProgramUsageRestore {
            get {
                return ResourceManager.GetString("ProgramUsageRestore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Times:
        /// Duplicati uses the time system from duplicity, where times may be presented as:
        ///  1: the string &quot;now&quot;, indicating the current time
        ///  2: the number of seconds after epoch, eg: 123456890
        ///  3: a string like &quot;2009-03-26T08:30:00+01:00&quot;
        ///  4: an interval string, using Y, M, W, D, h, m, s for Year, Month, Week, Day, hour, minute or second, eg: &quot;1M4D&quot; for one month and four days, or &quot;5m&quot; for five minutes..
        /// </summary>
        internal static string ProgramUsageTimes {
            get {
                return ResourceManager.GetString("ProgramUsageTimes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verify backup integrity:
        ///  Duplicati.CommandLine verify &lt;backend&gt; --verification-level=manifest|signatures|full.
        /// </summary>
        internal static string ProgramUsageVerify {
            get {
                return ResourceManager.GetString("ProgramUsageVerify", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Supported backends:.
        /// </summary>
        internal static string SupportedBackendsHeader {
            get {
                return ResourceManager.GetString("SupportedBackendsHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Supported compression modules:.
        /// </summary>
        internal static string SupportedCompressionModulesHeader {
            get {
                return ResourceManager.GetString("SupportedCompressionModulesHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Supported encryption modules:.
        /// </summary>
        internal static string SupportedEncryptionModulesHeader {
            get {
                return ResourceManager.GetString("SupportedEncryptionModulesHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Supported options:.
        /// </summary>
        internal static string SupportedOptionsHeader {
            get {
                return ResourceManager.GetString("SupportedOptionsHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to parse &quot;{0}&quot; into a time offset: {1}.
        /// </summary>
        internal static string TimeParseError {
            get {
                return ResourceManager.GetString("TimeParseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occured: {0}.
        /// </summary>
        internal static string UnhandledException {
            get {
                return ResourceManager.GetString("UnhandledException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The inner error message is: {0}.
        /// </summary>
        internal static string UnhandledInnerException {
            get {
                return ResourceManager.GetString("UnhandledInnerException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicati.CommandLine /home/user/ ftp://host/folder --exclude=/file.txt
        ///
        ///  In this example the file &quot;/home/user/file.txt&quot; is excluded..
        /// </summary>
        internal static string UsageExampleLinux {
            get {
                return ResourceManager.GetString("UsageExampleLinux", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicati.CommandLine C:\Documents\Files ftp://host/folder --exclude=\file.txt
        ///
        ///  In this example the file &quot;C:\Documents\Files\file.txt&quot; is excluded..
        /// </summary>
        internal static string UsageExampleWindows {
            get {
                return ResourceManager.GetString("UsageExampleWindows", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Verification completed, summary:
        ///Manifest files verified: {0}
        ///Signature files verified: {1}
        ///Content files verified: {2}
        ///Errors: {3}.
        /// </summary>
        internal static string VerificationCompleted {
            get {
                return ResourceManager.GetString("VerificationCompleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reported error messages:.
        /// </summary>
        internal static string VerificationErrorHeader {
            get {
                return ResourceManager.GetString("VerificationErrorHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wrong number of aguments.
        /// </summary>
        internal static string WrongNumberOfArgumentsError {
            get {
                return ResourceManager.GetString("WrongNumberOfArgumentsError", resourceCulture);
            }
        }
    }
}
