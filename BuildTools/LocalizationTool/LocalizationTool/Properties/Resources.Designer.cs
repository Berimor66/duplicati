﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LocalizationTool.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LocalizationTool.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to @echo off
        ///
        ///echo Processing updates, please wait ...
        ///
        ///set FOLDERNAME=
        ///for /f &quot;tokens=*&quot; %%a in (&quot;%CD%&quot;) do set FOLDERNAME=%%~na
        ///
        ///..\LocalizationTool.exe update %FOLDERNAME%
        ///..\LocalizationTool.exe guiupdate %FOLDERNAME%.
        /// </summary>
        internal static string Batchjob_bat {
            get {
                return ResourceManager.GetString("Batchjob.bat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;root&gt;
        ///  &lt;!-- 
        ///    Microsoft ResX Schema 
        ///    
        ///    Version 2.0
        ///    
        ///    The primary goals of this format is to allow a simple XML format 
        ///    that is mostly human readable. The generation and parsing of the 
        ///    various data types are done through the TypeConverter classes 
        ///    associated with the data types.
        ///    
        ///    Example:
        ///    
        ///    ... ado.net/XML headers &amp; schema ...
        ///    &lt;resheader name=&quot;resmimetype&quot;&gt;text/microsoft-resx&lt;/resheader&gt;
        ///    &lt;resheader n [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Empty_resx {
            get {
                return ResourceManager.GetString("Empty_resx", resourceCulture);
            }
        }
    }
}
