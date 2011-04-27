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
    internal class S3UI {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal S3UI() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Duplicati.Library.Backend.Strings.S3UI", typeof(S3UI).Assembly);
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
        ///   Looks up a localized string similar to The bucket name is not in all lower case.
        ///{0}
        ///Do you want to convert the bucket name to lower case?.
        /// </summary>
        internal static string BucketnameCaseWarning {
            get {
                return ResourceManager.GetString("BucketnameCaseWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The bucket name does not start with your user ID.
        ///To avoid using a bucket owned by another user,
        ///it is recommended that you put your user ID in front of the bucket name.
        ///Do you want to insert the user ID in front of the bucket name?.
        /// </summary>
        internal static string BucketnameNotPrefixedWarning {
            get {
                return ResourceManager.GetString("BucketnameNotPrefixedWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The bucket does not exist, do you want to create it?.
        /// </summary>
        internal static string CreateMissingBucket {
            get {
                return ResourceManager.GetString("CreateMissingBucket", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must enter your AWS Access ID.
        ///You may click the link to the right
        ///to open the AWS login page, and retrieve it..
        /// </summary>
        internal static string EmptyAWSIDError {
            get {
                return ResourceManager.GetString("EmptyAWSIDError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must enter your AWS Secret Key.
        ///You may click the link to the right
        ///to open the AWS login page, and retrieve it..
        /// </summary>
        internal static string EmptyAWSKeyError {
            get {
                return ResourceManager.GetString("EmptyAWSKeyError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must enter a name for the bucket.
        ///You must use a unique name for each backup.
        ///You may enter any name you like..
        /// </summary>
        internal static string EmptyBucketnameError {
            get {
                return ResourceManager.GetString("EmptyBucketnameError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The european buckets require that the bucket name is in lower case..
        /// </summary>
        internal static string EuroBucketsRequireLowerCaseError {
            get {
                return ResourceManager.GetString("EuroBucketsRequireLowerCaseError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The supplied bucket name, &quot;{0}&quot;, does not appear to be a valid hostname label. 
        ///This may still work if the non-subdomain calling style is used, but it is not recommended.
        ///
        ///Do you want to continue anyway?.
        /// </summary>
        internal static string HostnameInvalidWarning {
            get {
                return ResourceManager.GetString("HostnameInvalidWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The EU bucket option requires that the bucket names are valid hostname labels, but the supplied bucket name, &quot;{0}&quot;, is not a valid hostname label. Either change the bucketname or uncheck the EU bucket option..
        /// </summary>
        internal static string HostnameInvalidWithEuBucketOptionError {
            get {
                return ResourceManager.GetString("HostnameInvalidWithEuBucketOptionError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A bug in Mono versions older than 2.4.3,
        /// prevents the backend from detecting and missing buckets.
        ///Due to this, it is recommended that you create the bucket.
        ///Do you want to create the bucket now?.
        /// </summary>
        internal static string MonoRequiresExistingBucket {
            get {
                return ResourceManager.GetString("MonoRequiresExistingBucket", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The new amazon s3 API requires that bucket names are all lower case..
        /// </summary>
        internal static string NewS3RequiresLowerCaseError {
            get {
                return ResourceManager.GetString("NewS3RequiresLowerCaseError", resourceCulture);
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
    }
}
