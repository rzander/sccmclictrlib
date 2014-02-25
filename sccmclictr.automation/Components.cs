//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander
 
//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

#define CM2012
#define CM2007

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sccmclictr.automation;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Web;

namespace sccmclictr.automation.functions
{
    /// <summary>
    /// SCCM Agent Components
    /// </summary>
    public class components : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="components"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">A CCM Client object.</param>
        public components(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// List of Installed SCCM Agent Components
        /// </summary>
        public List<CCM_InstalledComponent> InstalledComponents
        {
            get
            {
                List<CCM_InstalledComponent> lComps = new List<CCM_InstalledComponent>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm", "SELECT * FROM CCM_InstalledComponent");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_InstalledComponent oCIEx = new CCM_InstalledComponent(PSObj, remoteRunspace, pSCode, baseClient);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lComps.Add(oCIEx);
                }
                return lComps; 
            } 
        }

        /// <summary>
        /// Gets a list of component client configuration.
        /// </summary>
        /// <value>A list of component client configuration.</value>
        public List<CCM_ComponentClientConfig> ComponentClientConfig
        {
            get
            {
                List<CCM_ComponentClientConfig> lComps = new List<CCM_ComponentClientConfig>();
                List<PSObject> oObj = GetObjects(@"root\ccm\Policy\Machine\ActualConfig", "SELECT * FROM CCM_ComponentClientConfig");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_ComponentClientConfig oCIEx = new CCM_ComponentClientConfig(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lComps.Add(oCIEx);
                }
                return lComps;
            }
        }

        /// <summary>
        /// Source:ROOT\ccm
        /// </summary>
        public class CCM_InstalledComponent
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_InstalledComponent"/> class.
            /// </summary>
            /// <param name="WMIObject">A WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            /// <param name="oClient">A CCM Client object.</param>
            public CCM_InstalledComponent(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.DisplayName = WMIObject.Properties["DisplayName"].Value as String;
                this.DisplayNameResourceFile = WMIObject.Properties["DisplayNameResourceFile"].Value as String;
                this.DisplayNameResourceID = WMIObject.Properties["DisplayNameResourceID"].Value as UInt32?;
                this.Name = WMIObject.Properties["Name"].Value as String;
                this.Version = WMIObject.Properties["Version"].Value as String;

                try
                {
                    //Disable Tracing
                    PSCode.Switch.Level = SourceLevels.Off;

                    this.Enabled = oClient.Components.ComponentClientConfig.First(t => t.ComponentName == this.Name).Enabled;
                }
                catch
                {
                    this.Enabled = false;
                    PSCode.Switch.Level = SourceLevels.All;
                }

                PSCode.Switch.Level = SourceLevels.All;
            }

            #region Properties
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String DisplayName { get; set; }
            public String DisplayNameResourceFile { get; set; }
            public UInt32? DisplayNameResourceID { get; set; }
            public String Name { get; set; }
            public String Version { get; set; }
            #pragma warning restore 1591 // Enable warnings about missing XML comments

            /// <summary>
            /// Get the Enabled Attribute from root\ccm\Policy\Machine\ActualConfig:CCM_InstalledComponent
            /// </summary>
            public Boolean? Enabled { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\Policy\Machine\ActualConfig
        /// </summary>
        public class CCM_ComponentClientConfig
        {
            //Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="CCM_ComponentClientConfig"/> class.
            /// </summary>
            /// <param name="WMIObject">A WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CCM_ComponentClientConfig(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ComponentName = WMIObject.Properties["ComponentName"].Value as String;
                this.Enabled = WMIObject.Properties["Enabled"].Value as Boolean?;
            }

            #region Properties
            #pragma warning disable 1591 // Disable warnings about missing XML comments

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String ComponentName { get; set; }
            public Boolean? Enabled { get; set; }

            #pragma warning restore 1591 // Enable warnings about missing XML comments
            #endregion

        }
    }


}
