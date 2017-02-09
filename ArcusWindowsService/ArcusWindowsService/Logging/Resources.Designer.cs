﻿/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Logging
{
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
    internal class Resources
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources()
        {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ArcusWindowsService.Logging.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to DEBUG:.
        /// </summary>
        internal static string DebugPrefix
        {
            get
            {
                return ResourceManager.GetString("DebugPrefix", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ERROR:.
        /// </summary>
        internal static string ErrorPrefix
        {
            get
            {
                return ResourceManager.GetString("ErrorPrefix", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EVENT:.
        /// </summary>
        internal static string EventPrefix
        {
            get
            {
                return ResourceManager.GetString("EventPrefix", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to EXCEPTION:.
        /// </summary>
        internal static string ExceptionPrefix
        {
            get
            {
                return ResourceManager.GetString("ExceptionPrefix", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to LoggingConfig.xml.
        /// </summary>
        internal static string LogConfigFile
        {
            get
            {
                return ResourceManager.GetString("LogConfigFile", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to [{0:0000}/{1,-12}] {2}{3}.
        /// </summary>
        internal static string SendTextFormat
        {
            get
            {
                return ResourceManager.GetString("SendTextFormat", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string SendTextPrefixFormatNone
        {
            get
            {
                return ResourceManager.GetString("SendTextPrefixFormatNone", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to WARNING:1.
        /// </summary>
        internal static string WarningPrefix
        {
            get
            {
                return ResourceManager.GetString("WarningPrefix", resourceCulture);
            }
        }
    }
}
