﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duplicati.GUI.Strings {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ServiceStatus {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ServiceStatus() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Duplicati.GUI.Strings.ServiceStatus", typeof(ServiceStatus).Assembly);
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
        ///   Looks up a localized string similar to Failed backup.
        /// </summary>
        internal static string BackupStatusError {
            get {
                return ResourceManager.GetString("BackupStatusError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Successfull backup.
        /// </summary>
        internal static string BackupStatusOK {
            get {
                return ResourceManager.GetString("BackupStatusOK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Backup only processed some of the files.
        /// </summary>
        internal static string BackupStatusPartial {
            get {
                return ResourceManager.GetString("BackupStatusPartial", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Completed backup with warnings.
        /// </summary>
        internal static string BackupStatusWarning {
            get {
                return ResourceManager.GetString("BackupStatusWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Backup: {0}.
        /// </summary>
        internal static string StatusBackup {
            get {
                return ResourceManager.GetString("StatusBackup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicati is currently paused.
        /// </summary>
        internal static string StatusPaused {
            get {
                return ResourceManager.GetString("StatusPaused", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Restore: {0}.
        /// </summary>
        internal static string StatusRestore {
            get {
                return ResourceManager.GetString("StatusRestore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Waiting for next backup.
        /// </summary>
        internal static string StatusWaiting {
            get {
                return ResourceManager.GetString("StatusWaiting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Advanced &gt;&gt;&gt;.
        /// </summary>
        internal static string SwitchToAdvanced {
            get {
                return ResourceManager.GetString("SwitchToAdvanced", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Simple &lt;&lt;&lt;.
        /// </summary>
        internal static string SwitchToSimple {
            get {
                return ResourceManager.GetString("SwitchToSimple", resourceCulture);
            }
        }
    }
}
