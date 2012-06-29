﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duplicati.Library.Utility.Strings {
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
    internal class Utility {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Utility() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Duplicati.Library.Utility.Strings.Utility", typeof(Utility).Assembly);
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
        ///   Looks up a localized string similar to {0} bytes.
        /// </summary>
        internal static string FormatStringB {
            get {
                return ResourceManager.GetString("FormatStringB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:N} GB.
        /// </summary>
        internal static string FormatStringGB {
            get {
                return ResourceManager.GetString("FormatStringGB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:N} KB.
        /// </summary>
        internal static string FormatStringKB {
            get {
                return ResourceManager.GetString("FormatStringKB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:N} MB.
        /// </summary>
        internal static string FormatStringMB {
            get {
                return ResourceManager.GetString("FormatStringMB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:N} TB.
        /// </summary>
        internal static string FormatStringTB {
            get {
                return ResourceManager.GetString("FormatStringTB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The digit &apos;{0}&apos; is not a valid hex digit.
        /// </summary>
        internal static string InvalidHexDigitError {
            get {
                return ResourceManager.GetString("InvalidHexDigitError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The hex string size must be a multiple of two, eg. the length must be even.
        /// </summary>
        internal static string InvalidHexStringLengthError {
            get {
                return ResourceManager.GetString("InvalidHexStringLengthError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The request timed out.
        /// </summary>
        internal static string TimeoutException {
            get {
                return ResourceManager.GetString("TimeoutException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The attempt to open a connection gave an unexpected result. Type: {0}, message: {1}.
        /// </summary>
        internal static string UnexpectedRequestResultError {
            get {
                return ResourceManager.GetString("UnexpectedRequestResultError", resourceCulture);
            }
        }
    }
}
