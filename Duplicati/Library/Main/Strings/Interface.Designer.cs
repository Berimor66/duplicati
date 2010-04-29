﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duplicati.Library.Main.Strings {
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
    internal class Interface {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Interface() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Duplicati.Library.Main.Strings.Interface", typeof(Interface).Assembly);
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
        ///   Looks up a localized string similar to Backend not found: {0}.
        /// </summary>
        internal static string BackendNotFoundError {
            get {
                return ResourceManager.GetString("BackendNotFoundError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bad sorting of backup times detected.
        /// </summary>
        internal static string BadSortingDetectedError {
            get {
                return ResourceManager.GetString("BadSortingDetectedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bad sort order on volumes detected.
        /// </summary>
        internal static string BadVolumeSortOrder {
            get {
                return ResourceManager.GetString("BadVolumeSortOrder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When using cleanup the manifests and hashes must be verified.
        /// </summary>
        internal static string CannotCleanWithoutHashesError {
            get {
                return ResourceManager.GetString("CannotCleanWithoutHashesError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleting backup at {0}.
        /// </summary>
        internal static string DeletingBackupSetMessage {
            get {
                return ResourceManager.GetString("DeletingBackupSetMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to find control files in: {0}.
        /// </summary>
        internal static string FailedToFindControlFilesMessage {
            get {
                return ResourceManager.GetString("FailedToFindControlFilesMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Files are not deleted, use the --force command to actually remove files.
        /// </summary>
        internal static string FilesAreNotForceDeletedMessage {
            get {
                return ResourceManager.GetString("FilesAreNotForceDeletedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An internal error occured, and the operation was aborted to protect the backup sets.
        /// </summary>
        internal static string InternalDeleteCountError {
            get {
                return ResourceManager.GetString("InternalDeleteCountError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to It is not allowed to disable manifest reading when performing a backup. Disabling manifests is only possible on restores, and it should only be used as a last resort..
        /// </summary>
        internal static string ManifestsMustBeReadOnBackups {
            get {
                return ResourceManager.GetString("ManifestsMustBeReadOnBackups", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The signature file for a content volume was missing, date: {0}, volumenumber: {1}, content volume filename: {2}.
        /// </summary>
        internal static string MissingSignatureFile {
            get {
                return ResourceManager.GetString("MissingSignatureFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No source folders specified for backup.
        /// </summary>
        internal static string NoSourceFoldersError {
            get {
                return ResourceManager.GetString("NoSourceFoldersError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not deleting backup at time: {0}, because later backups depend on it.
        /// </summary>
        internal static string NotDeletingBackupSetMessage {
            get {
                return ResourceManager.GetString("NotDeletingBackupSetMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The backup at time {0} is not removed because it is the last full backup, use --allow-full-removal to include this backup in the delete.
        /// </summary>
        internal static string NotDeletingLastFullMessage {
            get {
                return ResourceManager.GetString("NotDeletingLastFullMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The number of source folders in the latest backup is {0}, but the number of specified folders is now {1}. It is not allowed to change source folders for a backup..
        /// </summary>
        internal static string NumberOfSourceFoldersHasChangedError {
            get {
                return ResourceManager.GetString("NumberOfSourceFoldersHasChangedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed after uploading {0} volume(s). Error message: {1}.
        /// </summary>
        internal static string PartialUploadMessage {
            get {
                return ResourceManager.GetString("PartialUploadMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to read the primary manifest {0}, attempting secondary, if avalible. Error message: {1}.
        /// </summary>
        internal static string PrimaryManifestReadErrorLogMessage {
            get {
                return ResourceManager.GetString("PrimaryManifestReadErrorLogMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Secondary manifest file found: {0}, attempting read..
        /// </summary>
        internal static string ReadingSecondaryManifestLogMessage {
            get {
                return ResourceManager.GetString("ReadingSecondaryManifestLogMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Removing partial file: {0}.
        /// </summary>
        internal static string RemovingPartialFilesMessage {
            get {
                return ResourceManager.GetString("RemovingPartialFilesMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to read the secondary manifest {0}. Error message: {1}.
        /// </summary>
        internal static string SecondaryManifestReadErrorLogMessage {
            get {
                return ResourceManager.GetString("SecondaryManifestReadErrorLogMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Signature cache path was not given as an argument.
        /// </summary>
        internal static string SignatureCachePathMissingError {
            get {
                return ResourceManager.GetString("SignatureCachePathMissingError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The content volume {0} is not present in the manifest and is not used.
        /// </summary>
        internal static string SkippedContentVolumeLogMessage {
            get {
                return ResourceManager.GetString("SkippedContentVolumeLogMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The folder {0} is included multiple times.
        /// </summary>
        internal static string SourceDirIsIncludedMultipleTimesError {
            get {
                return ResourceManager.GetString("SourceDirIsIncludedMultipleTimesError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The folder {1} is a subfolder of {0}. It is not allowed to specify the same folder multiple times..
        /// </summary>
        internal static string SourceDirsAreRelatedError {
            get {
                return ResourceManager.GetString("SourceDirsAreRelatedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The folder {0} is no longer in the source folder set. It is not allowed to change source folders for a backup..
        /// </summary>
        internal static string SourceFoldersHasChangedError {
            get {
                return ResourceManager.GetString("SourceFoldersHasChangedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Building filelist ....
        /// </summary>
        internal static string StatusBuildingFilelist {
            get {
                return ResourceManager.GetString("StatusBuildingFilelist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Completed.
        /// </summary>
        internal static string StatusCompleted {
            get {
                return ResourceManager.GetString("StatusCompleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating volume {0}.
        /// </summary>
        internal static string StatusCreatingVolume {
            get {
                return ResourceManager.GetString("StatusCreatingVolume", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloading content, volume {0}.
        /// </summary>
        internal static string StatusDownloadingContentVolume {
            get {
                return ResourceManager.GetString("StatusDownloadingContentVolume", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Downloading signatures, volume {0}.
        /// </summary>
        internal static string StatusDownloadingSignatureVolume {
            get {
                return ResourceManager.GetString("StatusDownloadingSignatureVolume", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading remote filelist.
        /// </summary>
        internal static string StatusLoadingFilelist {
            get {
                return ResourceManager.GetString("StatusLoadingFilelist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Patching restore with #{0}.
        /// </summary>
        internal static string StatusPatching {
            get {
                return ResourceManager.GetString("StatusPatching", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processing: {0}.
        /// </summary>
        internal static string StatusProcessing {
            get {
                return ResourceManager.GetString("StatusProcessing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reading incremental data ....
        /// </summary>
        internal static string StatusReadingIncrementalData {
            get {
                return ResourceManager.GetString("StatusReadingIncrementalData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reading incremental data from: {0}.
        /// </summary>
        internal static string StatusReadingIncrementalFile {
            get {
                return ResourceManager.GetString("StatusReadingIncrementalFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reading incremental data.
        /// </summary>
        internal static string StatusReadingIncrementals {
            get {
                return ResourceManager.GetString("StatusReadingIncrementals", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reading manifest file: {0}.
        /// </summary>
        internal static string StatusReadingManifest {
            get {
                return ResourceManager.GetString("StatusReadingManifest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Reading signatures: {0}, vol {1}.
        /// </summary>
        internal static string StatusReadingSignatureFile {
            get {
                return ResourceManager.GetString("StatusReadingSignatureFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Started.
        /// </summary>
        internal static string StatusStarted {
            get {
                return ResourceManager.GetString("StatusStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Uploading content, volume {0}.
        /// </summary>
        internal static string StatusUploadingContentVolume {
            get {
                return ResourceManager.GetString("StatusUploadingContentVolume", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Uploading manifest, volume {0}.
        /// </summary>
        internal static string StatusUploadingManifestVolume {
            get {
                return ResourceManager.GetString("StatusUploadingManifestVolume", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Uploading signatures, volume {0}.
        /// </summary>
        internal static string StatusUploadingSignatureVolume {
            get {
                return ResourceManager.GetString("StatusUploadingSignatureVolume", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected compression mode: {0}.
        /// </summary>
        internal static string UnexpectedCompressionError {
            get {
                return ResourceManager.GetString("UnexpectedCompressionError", resourceCulture);
            }
        }
    }
}
