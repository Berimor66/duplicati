﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duplicati.Library.Backend.Strings {
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
    internal class SSHUI {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SSHUI() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Duplicati.Library.Backend.Strings.SSHUI", typeof(SSHUI).Assembly);
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
        ///   Looks up a localized string similar to The folder does not exist, do you want to create it?.
        /// </summary>
        internal static string CreateMissingFolderQuestion {
            get {
                return ResourceManager.GetString("CreateMissingFolderQuestion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Duplicati was unable to verify the existence of the sftp program.
        ///sfpt may work regardless, if it is located in the system search path.
        ///
        ///Do you want to continue anyway?.
        /// </summary>
        internal static string MissingSCPWarning {
            get {
                return ResourceManager.GetString("MissingSCPWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to On this page you can select where to store the backup data..
        /// </summary>
        internal static string PageDescription {
            get {
                return ResourceManager.GetString("PageDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Backup storage options.
        /// </summary>
        internal static string PageTitle {
            get {
                return ResourceManager.GetString("PageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A password is required when using the managed SSH option.
        /// </summary>
        internal static string PasswordRequiredError {
            get {
                return ResourceManager.GetString("PasswordRequiredError", resourceCulture);
            }
        }
    }
}
